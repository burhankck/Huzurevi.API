using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.Kullanicilar;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("kullanici")]
public class KullaniciController : TemelApiController
{
    private readonly IKullaniciServisi _kullaniciServisi;
    private readonly IValidator<KullaniciOlusturIstek> _olusturDogrulayici;
    private readonly IValidator<KullaniciGuncelleIstek> _guncelleDogrulayici;
    private readonly IValidator<KullaniciSifreIstek> _sifreDogrulayici;

    public KullaniciController(
        IKullaniciServisi kullaniciServisi,
        IValidator<KullaniciOlusturIstek> olusturDogrulayici,
        IValidator<KullaniciGuncelleIstek> guncelleDogrulayici,
        IValidator<KullaniciSifreIstek> sifreDogrulayici)
    {
        _kullaniciServisi = kullaniciServisi;
        _olusturDogrulayici = olusturDogrulayici;
        _guncelleDogrulayici = guncelleDogrulayici;
        _sifreDogrulayici = sifreDogrulayici;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiYanit<List<KullaniciListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKullanicilar(CancellationToken ct)
    {
        return Ok(await _kullaniciServisi.TumunuGetirAsync(ct));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<KullaniciListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKullanici(int id, CancellationToken ct)
    {
        return Ok(await _kullaniciServisi.GetirAsync(id, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiYanit<KullaniciListDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostKullanici(KullaniciOlusturIstek istek, CancellationToken ct)
    {
        await _olusturDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _kullaniciServisi.OlusturAsync(istek, ct);
        return Created(sonuc, "Kullanıcı oluşturuldu.");
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutKullanici(int id, KullaniciGuncelleIstek istek, CancellationToken ct)
    {
        if (OturumKullaniciId is not int islemYapan)
        {
            return Unauthorized();
        }

        await _guncelleDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _kullaniciServisi.GuncelleAsync(id, istek, islemYapan, ct);
        return Ok<object?>(null, "Kullanıcı güncellendi.");
    }

    [HttpPost("{id:int}/sifre")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PostSifre(int id, KullaniciSifreIstek istek, CancellationToken ct)
    {
        await _sifreDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _kullaniciServisi.SifreAtaAsync(id, istek, ct);
        return Ok<object?>(null, "Şifre güncellendi.");
    }

    [HttpPost("{id:int}/kilit-ac")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PostKilitAc(int id, CancellationToken ct)
    {
        await _kullaniciServisi.KilidiAcAsync(id, ct);
        return Ok<object?>(null, "Hesap kilidi açıldı.");
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteKullanici(int id, CancellationToken ct)
    {
        if (OturumKullaniciId is not int islemYapan)
        {
            return Unauthorized();
        }

        await _kullaniciServisi.SilAsync(id, islemYapan, ct);
        return Ok<object?>(null, "Kullanıcı silindi.");
    }
}
