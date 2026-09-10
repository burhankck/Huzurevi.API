using System.Net;
using System.Security.Claims;
using System.Text.Json;
using FluentValidation;
using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.Denetim;

namespace Huzurevi.API.Middleware;

public class HataYonetimAraKatmani
{
    private readonly RequestDelegate _sonraki;
    private readonly ILogger<HataYonetimAraKatmani> _gunluk;

    public HataYonetimAraKatmani(RequestDelegate sonraki, ILogger<HataYonetimAraKatmani> gunluk)
    {
        _sonraki = sonraki;
        _gunluk = gunluk;
    }

    public async Task InvokeAsync(HttpContext baglam)
    {
        try
        {
            await _sonraki(baglam);
        }
        catch (Exception hata)
        {
            await HataIsleAsync(baglam, hata);
        }
    }

    private async Task HataIsleAsync(HttpContext baglam, Exception hata)
    {
        var (durumKodu, mesaj, hatalar) = hata switch
        {
            ValidationException dogrulamaHatasi => (
                HttpStatusCode.BadRequest,
                "Doğrulama hatası.",
                dogrulamaHatasi.Errors.Select(e => e.ErrorMessage).ToList()),
            KayitBulunamadiHatasi kayitHatasi => (HttpStatusCode.NotFound, kayitHatasi.Message, (List<string>?)null),
            CakismaHatasi cakismaHatasi => (HttpStatusCode.Conflict, cakismaHatasi.Message, (List<string>?)null),
            GecersizIstekHatasi gecersizIstek => (HttpStatusCode.BadRequest, gecersizIstek.Message, (List<string>?)null),
            YetkisizHatasi yetkisiz => (HttpStatusCode.Unauthorized, yetkisiz.Message, (List<string>?)null),
            HesapKilitliHatasi kilitli => (HttpStatusCode.Forbidden, kilitli.Message, (List<string>?)null),
            BakimModuHatasi bakim => (HttpStatusCode.ServiceUnavailable, bakim.Message, (List<string>?)null),
            _ => (HttpStatusCode.InternalServerError, "Beklenmeyen bir hata oluştu.", (List<string>?)null)
        };

        if (durumKodu == HttpStatusCode.InternalServerError)
        {
            _gunluk.LogError(hata, "Beklenmeyen hata: {Mesaj}", hata.Message);
            await HataLoguYaz(baglam, hata, mesaj);
        }
        else
        {
            _gunluk.LogWarning("İşlenen hata: {Mesaj}", hata.Message);
        }

        baglam.Response.ContentType = "application/json";
        baglam.Response.StatusCode = (int)durumKodu;

        var yanit = ApiYanit<object?>.Basarisiz(mesaj, hatalar);
        await baglam.Response.WriteAsync(JsonSerializer.Serialize(yanit, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    private async Task HataLoguYaz(HttpContext baglam, Exception hata, string mesaj)
    {
        try
        {
            var servis = baglam.RequestServices.GetService<IDenetimServisi>();
            if (servis is null) return;
            var idHam = baglam.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? baglam.User.FindFirstValue("sub");
            int? kullaniciId = int.TryParse(idHam, out var id) ? id : null;
            var ip = baglam.Request.Headers.TryGetValue("X-Forwarded-For", out var zincir)
                ? zincir.ToString().Split(',')[0].Trim()
                : baglam.Connection.RemoteIpAddress?.ToString();
            await servis.YazAsync(new DenetimYazIstek(
                "Hata",
                "Beklenmeyen hata",
                mesaj,
                baglam.User.Identity?.Name,
                kullaniciId,
                ip,
                500,
                baglam.Request.Path.Value,
                hata.GetType().Name,
                hata.ToString()));
        }
        catch (Exception logHata)
        {
            _gunluk.LogWarning(logHata, "Hata logu yazılamadı.");
        }
    }
}
