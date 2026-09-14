using System.Security.Claims;
using System.Text.Json;
using System.Threading.RateLimiting;
using Huzurevi.Application.Common.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace Huzurevi.API.Guvenlik;

public static class IstekSinirlama
{
    public const string KimlikPolitika = "kimlik";

    public static IServiceCollection IstekSinirlamasiEkle(this IServiceCollection servisler, IConfiguration yapilandirma)
    {
        var genelIstek = yapilandirma.GetValue("RateLimit:GenelIstek", 180);
        var genelDakika = yapilandirma.GetValue("RateLimit:GenelDakika", 1);
        var kimlikIstek = yapilandirma.GetValue("RateLimit:KimlikIstek", 8);
        var kimlikDakika = yapilandirma.GetValue("RateLimit:KimlikDakika", 1);

        servisler.AddRateLimiter(secenek =>
        {
            secenek.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            secenek.OnRejected = async (baglam, iptal) =>
            {
                baglam.HttpContext.Response.ContentType = "application/json";
                if (baglam.Lease.TryGetMetadata(MetadataName.RetryAfter, out var bekleme))
                {
                    baglam.HttpContext.Response.Headers.RetryAfter = ((int)Math.Ceiling(bekleme.TotalSeconds)).ToString();
                }

                var yanit = ApiYanit<object?>.Basarisiz("Çok fazla istek gönderildi. Lütfen kısa süre sonra tekrar deneyin.");
                await baglam.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(yanit), iptal);
            };

            secenek.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(http =>
            {
                if (http.Request.Path.StartsWithSegments("/health"))
                {
                    return RateLimitPartition.GetNoLimiter("health");
                }

                return RateLimitPartition.GetFixedWindowLimiter(Anahtar(http), _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = genelIstek,
                    Window = TimeSpan.FromMinutes(genelDakika),
                    QueueLimit = 0
                });
            });

            secenek.AddPolicy(KimlikPolitika, http =>
                RateLimitPartition.GetFixedWindowLimiter(Anahtar(http), _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = kimlikIstek,
                    Window = TimeSpan.FromMinutes(kimlikDakika),
                    QueueLimit = 0
                }));
        });

        return servisler;
    }

    private static string Anahtar(HttpContext http)
    {
        var kullaniciId = http.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? http.User.FindFirstValue("sub");
        if (!string.IsNullOrWhiteSpace(kullaniciId))
        {
            return $"k:{kullaniciId}";
        }

        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "bilinmeyen";
        return $"ip:{ip}";
    }
}
