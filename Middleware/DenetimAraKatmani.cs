using System.Security.Claims;
using Huzurevi.Application.Features.Denetim;

namespace Huzurevi.API.Middleware;

public class DenetimAraKatmani
{
    private static readonly HashSet<string> Atlanan = new(StringComparer.OrdinalIgnoreCase)
    {
        "/health", "/swagger", "/favicon.ico"
    };

    private readonly RequestDelegate _sonraki;
    private readonly ILogger<DenetimAraKatmani> _gunluk;

    public DenetimAraKatmani(RequestDelegate sonraki, ILogger<DenetimAraKatmani> gunluk)
    {
        _sonraki = sonraki;
        _gunluk = gunluk;
    }

    public async Task InvokeAsync(HttpContext baglam)
    {
        var yol = baglam.Request.Path.Value ?? "";
        if (!yol.StartsWith("/api", StringComparison.OrdinalIgnoreCase)
            || yol.StartsWith("/api/Denetim", StringComparison.OrdinalIgnoreCase)
            || Atlanan.Any(a => yol.StartsWith(a, StringComparison.OrdinalIgnoreCase)))
        {
            await _sonraki(baglam);
            return;
        }

        await _sonraki(baglam);
        await Kaydet(baglam, yol);
    }

    private async Task Kaydet(HttpContext baglam, string yol)
    {
        try
        {
            var metot = baglam.Request.Method.ToUpperInvariant();
            if (metot == "OPTIONS") return;

            var durum = baglam.Response.StatusCode;
            var kvkk = SaglikYolu(yol);
            var degisen = metot is "POST" or "PUT" or "PATCH" or "DELETE";
            var tur = kvkk ? "Kvkk" : degisen ? "Islem" : "Erisim";
            var kullanici = baglam.User.Identity?.Name;
            var idHam = baglam.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? baglam.User.FindFirstValue("sub");
            int? kullaniciId = int.TryParse(idHam, out var id) ? id : null;

            var servis = baglam.RequestServices.GetRequiredService<IDenetimServisi>();
            await servis.YazAsync(new DenetimYazIstek(
                tur,
                $"{metot} {yol}",
                $"HTTP {durum}",
                kullanici,
                kullaniciId,
                Ip(baglam),
                durum,
                yol,
                null,
                null),
                baglam.RequestAborted);
        }
        catch (Exception ex)
        {
            _gunluk.LogWarning(ex, "Denetim kaydı yazılamadı.");
        }
    }

    private static bool SaglikYolu(string yol)
    {
        var p = yol.ToLowerInvariant();
        return p.Contains("/ilac")
            || p.Contains("/narkotik")
            || p.Contains("/kurumsaglik")
            || p.Contains("/sakinsaglik")
            || p.Contains("/olcum")
            || p.Contains("fizyo")
            || p.Contains("revir")
            || p.Contains("nobet")
            || p.Contains("periyodik");
    }

    private static string? Ip(HttpContext baglam)
    {
        if (baglam.Request.Headers.TryGetValue("X-Forwarded-For", out var zincir))
        {
            var ilk = zincir.ToString().Split(',')[0].Trim();
            if (!string.IsNullOrWhiteSpace(ilk)) return ilk;
        }
        return baglam.Connection.RemoteIpAddress?.ToString();
    }
}
