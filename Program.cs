using Huzurevi.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantı dizesi (appsettings okunamazsa doğrudan localhost dizesini kullanır)
var baglantiDizesi = builder.Configuration.GetConnectionString("VarsayilanBaglanti") 
    ?? "Host=localhost;Port=5432;Database=HuzureviDb;Username=burhan;Password=";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(baglantiDizesi));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // İlişkili tablolar (Oda -> Yatak) döngüye girmesin diye
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// React Frontend için CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactIzin", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Huzurevi API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("ReactIzin");
app.UseAuthorization();
app.MapControllers();

app.Run();