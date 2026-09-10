using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Yedekleme;
using Huzurevi.Infrastructure.Pdf;
using Huzurevi.Infrastructure.Persistence;
using Huzurevi.Infrastructure.Security;
using Huzurevi.Infrastructure.Storage;
using Huzurevi.Infrastructure.Yedekleme;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Huzurevi.Infrastructure;

public static class BagimlilikEnjeksiyonu
{
    public static IServiceCollection AltyapiKatmaniniEkle(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("VarsayilanBaglanti");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection yapılandırması bulunamadı. " +
                "PostgreSQL bağlantı dizesini appsettings veya ortam değişkenine ekleyin.");
        }

        services.AddDbContext<UygulamaDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                npgsql.CommandTimeout(30);
            }));

        services.AddScoped<IUygulamaDbContext>(provider => provider.GetRequiredService<UygulamaDbContext>());
        services.AddSingleton<ISifreHasher, SifreHasher>();
        services.AddSingleton<ITokenUretici, JwtTokenUretici>();
        services.AddSingleton<IDosyaDepolama>(sp =>
        {
            var ortam = sp.GetRequiredService<IHostEnvironment>();
            return new YerelDosyaDepolama(Path.Combine(ortam.ContentRootPath, "Uploads"));
        });
        services.AddSingleton<IGunlukIlacPdfUretici, GunlukIlacPdfUretici>();
        services.AddSingleton<IKurumSurecPdfUretici, KurumSurecPdfUretici>();
        services.AddSingleton<IBarkodEtiketUretici, BarkodEtiketUretici>();
        services.AddSingleton<ITabloCiktiUretici, TabloCiktiUretici>();
        services.AddScoped<IYedeklemeServisi, YedeklemeServisi>();
        services.AddHostedService<YedeklemeZamanlayici>();

        return services;
    }
}
