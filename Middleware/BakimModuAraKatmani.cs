using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.API.Middleware;

public class BakimModuAraKatmani
{
    private readonly RequestDelegate _sonraki;

    public BakimModuAraKatmani(RequestDelegate sonraki) => _sonraki = sonraki;

    public async Task InvokeAsync(HttpContext baglam, IUygulamaDbContext db)
    {
        var yol = baglam.Request.Path.Value ?? "";
        if (Istisna(yol))
        {
            await _sonraki(baglam);
            return;
        }

        var bakim = await db.UygulamaAyarlari.AsNoTracking().Select(x => x.BakimModu).FirstOrDefaultAsync();
        if (!bakim)
        {
            await _sonraki(baglam);
            return;
        }

        var rol = baglam.User.FindFirstValue(ClaimTypes.Role) ?? baglam.User.FindFirstValue("role");
        if (rol == "Yonetici")
        {
            await _sonraki(baglam);
            return;
        }

        baglam.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
        baglam.Response.ContentType = "application/json";
        var yanit = ApiYanit<object?>.Basarisiz("Sistem bakımda. Yalnızca yöneticiler işlem yapabilir.");
        await baglam.Response.WriteAsync(JsonSerializer.Serialize(yanit, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    private static bool Istisna(string yol)
    {
        if (yol.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)) return true;
        if (yol.Equals("/health", StringComparison.OrdinalIgnoreCase)) return true;
        if (yol.Equals("/api/Kimlik/giris", StringComparison.OrdinalIgnoreCase)) return true;
        if (yol.Equals("/api/Kimlik/sifremi-unuttum", StringComparison.OrdinalIgnoreCase)) return true;
        if (yol.Equals("/api/Kimlik/sifre-sifirla", StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }
}
