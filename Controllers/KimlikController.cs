using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.Kimlik;
using Huzurevi.API.Guvenlik;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Huzurevi.API.Controllers;

public class KimlikController : TemelApiController
{
    private readonly IKimlikServisi _kimlikServisi;
    private readonly IValidator<GirisIstek> _girisDogrulayici;
    private readonly IValidator<ProfilGuncelleIstek> _profilDogrulayici;
    private readonly IValidator<SifremiUnuttumIstek> _unuttumDogrulayici;
    private readonly IValidator<SifreSifirlaIstek> _sifirlaDogrulayici;
    private readonly IValidator<KurulusSecIstek> _kurulusDogrulayici;
    private readonly IHostEnvironment _ortam;

    public KimlikController(
        IKimlikServisi kimlikServisi,
        IValidator<GirisIstek> girisDogrulayici,
        IValidator<ProfilGuncelleIstek> profilDogrulayici,
        IValidator<SifremiUnuttumIstek> unuttumDogrulayici,
        IValidator<SifreSifirlaIstek> sifirlaDogrulayici,
        IValidator<KurulusSecIstek> kurulusDogrulayici,
        IHostEnvironment ortam)
    {
        _kimlikServisi = kimlikServisi;
        _girisDogrulayici = girisDogrulayici;
        _profilDogrulayici = profilDogrulayici;
        _unuttumDogrulayici = unuttumDogrulayici;
        _sifirlaDogrulayici = sifirlaDogrulayici;
        _kurulusDogrulayici = kurulusDogrulayici;
        _ortam = ortam;
    }

    [AllowAnonymous]
    [EnableRateLimiting(IstekSinirlama.KimlikPolitika)]
    [HttpPost("giris")]
    [ProducesResponseType(typeof(ApiYanit<GirisSonuc>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Giris(GirisIstek istek, CancellationToken ct)
    {
        await _girisDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _kimlikServisi.GirisYapAsync(istek, ct);
        return Ok(sonuc, "Giriş başarılı.");
    }

    [AllowAnonymous]
    [EnableRateLimiting(IstekSinirlama.KimlikPolitika)]
    [HttpPost("sifremi-unuttum")]
    public async Task<IActionResult> SifremiUnuttum(SifremiUnuttumIstek istek, CancellationToken ct)
    {
        await _unuttumDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _kimlikServisi.SifremiUnuttumAsync(istek, ct);
        if (!_ortam.IsDevelopment())
        {
            sonuc = sonuc with { GelistirmeKodu = null };
        }
        return Ok(sonuc, sonuc.Mesaj);
    }

    [AllowAnonymous]
    [EnableRateLimiting(IstekSinirlama.KimlikPolitika)]
    [HttpPost("sifre-sifirla")]
    public async Task<IActionResult> SifreSifirla(SifreSifirlaIstek istek, CancellationToken ct)
    {
        await _sifirlaDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _kimlikServisi.SifreSifirlaAsync(istek, ct);
        return Ok<object?>(null, "Şifre güncellendi. Giriş yapabilirsiniz.");
    }

    [Authorize]
    [HttpGet("ben")]
    [ProducesResponseType(typeof(ApiYanit<KullaniciOzetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Ben(CancellationToken ct)
    {
        var idDegeri = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!int.TryParse(idDegeri, out var kullaniciId))
        {
            return Unauthorized();
        }

        int? kurulusId = int.TryParse(User.FindFirstValue("kurulusId"), out var kid) ? kid : null;
        var sonuc = await _kimlikServisi.BeniGetirAsync(kullaniciId, kurulusId, ct);
        return Ok(sonuc);
    }

    [Authorize]
    [HttpGet("profil-yetkileri")]
    [ProducesResponseType(typeof(ApiYanit<Huzurevi.Application.Features.Yetkiler.ProfilYetkileriDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ProfilYetkileri(CancellationToken ct)
    {
        if (OturumKullaniciId is not int kullaniciId)
        {
            return Unauthorized();
        }

        int? kurulusId = int.TryParse(User.FindFirstValue("kurulusId"), out var kid) ? kid : null;
        var sonuc = await _kimlikServisi.ProfilYetkileriGetirAsync(kullaniciId, kurulusId, ct);
        return Ok(sonuc);
    }

    [Authorize]
    [HttpPost("kurulus-sec")]
    public async Task<IActionResult> KurulusSec(KurulusSecIstek istek, CancellationToken ct)
    {
        if (OturumKullaniciId is not int kullaniciId)
        {
            return Unauthorized();
        }

        await _kurulusDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _kimlikServisi.KurulusSecAsync(kullaniciId, istek.KurulusId, ct);
        return Ok(sonuc, "Kuruluş değiştirildi.");
    }

    [Authorize]
    [HttpPut("profil")]
    public async Task<IActionResult> Profil(ProfilGuncelleIstek istek, CancellationToken ct)
    {
        if (OturumKullaniciId is not int kullaniciId)
        {
            return Unauthorized();
        }

        await _profilDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _kimlikServisi.ProfilGuncelleAsync(kullaniciId, istek, ct);
        return Ok(sonuc, "Profil güncellendi.");
    }
}
