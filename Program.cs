using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Huzurevi.API.Guvenlik;
using Huzurevi.API.Json;
using Huzurevi.API.Middleware;
using Huzurevi.Application;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Common.Models;
using Huzurevi.Infrastructure;
using Huzurevi.Infrastructure.Persistence;
using Huzurevi.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/huzurevi-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
            .WriteTo.Console()
            .WriteTo.File("Logs/huzurevi-.log", rollingInterval: RollingInterval.Day));

    builder.Services.UygulamaKatmaniniEkle();
    builder.Services.AltyapiKatmaniniEkle(builder.Configuration);

    var jwtAnahtar = builder.Configuration["Jwt:Anahtar"]
        ?? throw new InvalidOperationException("Jwt:Anahtar yapılandırması eksik.");
    var jwtYayinci = builder.Configuration["Jwt:Yayinci"] ?? "Huzurevi.API";
    var jwtDinleyici = builder.Configuration["Jwt:Dinleyici"] ?? "Huzurevi.Web";

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtYayinci,
                ValidAudience = jwtDinleyici,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAnahtar)),
                ClockSkew = TimeSpan.FromMinutes(1),
                RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                NameClaimType = System.Security.Claims.ClaimTypes.Name
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var yanit = ApiYanit<object?>.Basarisiz("Oturum açmanız gerekiyor.");
                    await context.Response.WriteAsync(JsonSerializer.Serialize(yanit));
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    var yanit = ApiYanit<object?>.Basarisiz("Bu işlem için yetkiniz yok.");
                    await context.Response.WriteAsync(JsonSerializer.Serialize(yanit));
                }
            };
        });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<Huzurevi.Application.Common.Interfaces.IOturumBaglami, Huzurevi.API.Yetkilendirme.OturumBaglami>();
    builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider, Huzurevi.API.Yetkilendirme.YetkiPolitikaSaglayici>();
    builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, Huzurevi.API.Yetkilendirme.IzinYetkiIsleyici>();

    builder.Services.AddAuthorization();
    builder.Services.IstekSinirlamasiEkle(builder.Configuration);
    builder.Services.Configure<FormOptions>(secenek =>
    {
        secenek.MultipartBodyLengthLimit = 12 * 1024 * 1024;
    });

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new EsnekTarihDonusturucu());
            options.JsonSerializerOptions.Converters.Add(new EsnekBosOlabilirTarihDonusturucu());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });

    builder.Services.Configure<ApiBehaviorOptions>(secenek =>
    {
        secenek.InvalidModelStateResponseFactory = baglam =>
        {
            var hatalar = baglam.ModelState
                .SelectMany(k => k.Value?.Errors ?? [])
                .Select(e =>
                {
                    if (!string.IsNullOrWhiteSpace(e.ErrorMessage) && !e.ErrorMessage.Contains("JSON value", StringComparison.OrdinalIgnoreCase))
                    {
                        return e.ErrorMessage;
                    }

                    var ham = e.ErrorMessage + e.Exception?.Message;
                    if (ham.Contains("DateTime", StringComparison.OrdinalIgnoreCase))
                    {
                        return "Tarih formatı geçersiz.";
                    }

                    return string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Gönderilen bilgiler doğrulanamadı." : e.ErrorMessage;
                })
                .Distinct()
                .ToList();

            var mesaj = hatalar.FirstOrDefault() ?? "Gönderilen bilgiler doğrulanamadı.";
            return new BadRequestObjectResult(ApiYanit<object?>.Basarisiz(mesaj, hatalar));
        };
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Huzurevi API", Version = "v1" });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT: Bearer {token}"
        });
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ReactIzin", policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                .WithExposedHeaders("Content-Disposition");
        });
    });

    var app = builder.Build();

    using (var kapsam = app.Services.CreateScope())
    {
        var db = kapsam.ServiceProvider.GetRequiredService<UygulamaDbContext>();
        var hasher = kapsam.ServiceProvider.GetRequiredService<ISifreHasher>();
        await db.Database.MigrateAsync();
        await KullaniciTohumu.UygulaAsync(db, hasher);
        await KurumTohumu.UygulaAsync(db);
    }

    app.UseMiddleware<HataYonetimAraKatmani>();

    var enableSwagger = app.Environment.IsDevelopment()
        || app.Configuration.GetValue<bool>("Swagger:Enabled");
    if (enableSwagger)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Huzurevi API v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseCors("ReactIzin");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRateLimiter();
    app.UseMiddleware<BakimModuAraKatmani>();
    app.UseMiddleware<DenetimAraKatmani>();
    app.MapGet("/health", () => Results.Ok(new { status = "ok" })).DisableRateLimiting();
    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Huzurevi API başlatılamadı.");
}
finally
{
    Log.CloseAndFlush();
}
