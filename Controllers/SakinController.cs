using FluentValidation;
using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("sakin")]
public class SakinController : TemelApiController
{
    private readonly ISakinServisi _sakinService;
    private readonly IVasiServisi _vasiServisi;
    private readonly IMirasciServisi _mirasciServisi;
    private readonly ISakinMalServisi _malServisi;
    private readonly ISakinGelirServisi _gelirServisi;
    private readonly ISakinSosyalGuvenceServisi _sosyalGuvenceServisi;
    private readonly ISakinGunlukIzinServisi _gunlukIzinServisi;
    private readonly ISakinEmanetServisi _emanetServisi;
    private readonly ISakinBelgeServisi _belgeServisi;
    private readonly IValidator<SakinOlusturIstek> _createValidator;
    private readonly IValidator<SakinGuncelleIstek> _updateValidator;
    private readonly IValidator<YakinOlusturIstek> _yakinOlusturDogrulayici;
    private readonly IValidator<YakinGuncelleIstek> _yakinGuncelleDogrulayici;
    private readonly IValidator<VasiOlusturIstek> _vasiOlusturDogrulayici;
    private readonly IValidator<VasiGuncelleIstek> _vasiGuncelleDogrulayici;
    private readonly IValidator<MirasciOlusturIstek> _mirasciOlusturDogrulayici;
    private readonly IValidator<MirasciGuncelleIstek> _mirasciGuncelleDogrulayici;
    private readonly IValidator<SakinMalIstek> _malDogrulayici;
    private readonly IValidator<SakinGelirIstek> _gelirDogrulayici;
    private readonly IValidator<SakinSosyalGuvenceIstek> _sosyalGuvenceDogrulayici;
    private readonly IValidator<SakinGunlukIzinIstek> _gunlukIzinDogrulayici;
    private readonly IValidator<SakinEmanetIstek> _emanetDogrulayici;
    private readonly IValidator<BelgeYukleIstek> _belgeYukleDogrulayici;
    private readonly IValidator<BelgeGuncelleIstek> _belgeGuncelleDogrulayici;

    public SakinController(
        ISakinServisi sakinService,
        IVasiServisi vasiServisi,
        IMirasciServisi mirasciServisi,
        ISakinMalServisi malServisi,
        ISakinGelirServisi gelirServisi,
        ISakinSosyalGuvenceServisi sosyalGuvenceServisi,
        ISakinGunlukIzinServisi gunlukIzinServisi,
        ISakinEmanetServisi emanetServisi,
        ISakinBelgeServisi belgeServisi,
        IValidator<SakinOlusturIstek> createValidator,
        IValidator<SakinGuncelleIstek> updateValidator,
        IValidator<YakinOlusturIstek> yakinOlusturDogrulayici,
        IValidator<YakinGuncelleIstek> yakinGuncelleDogrulayici,
        IValidator<VasiOlusturIstek> vasiOlusturDogrulayici,
        IValidator<VasiGuncelleIstek> vasiGuncelleDogrulayici,
        IValidator<MirasciOlusturIstek> mirasciOlusturDogrulayici,
        IValidator<MirasciGuncelleIstek> mirasciGuncelleDogrulayici,
        IValidator<SakinMalIstek> malDogrulayici,
        IValidator<SakinGelirIstek> gelirDogrulayici,
        IValidator<SakinSosyalGuvenceIstek> sosyalGuvenceDogrulayici,
        IValidator<SakinGunlukIzinIstek> gunlukIzinDogrulayici,
        IValidator<SakinEmanetIstek> emanetDogrulayici,
        IValidator<BelgeYukleIstek> belgeYukleDogrulayici,
        IValidator<BelgeGuncelleIstek> belgeGuncelleDogrulayici)
    {
        _sakinService = sakinService;
        _vasiServisi = vasiServisi;
        _mirasciServisi = mirasciServisi;
        _malServisi = malServisi;
        _gelirServisi = gelirServisi;
        _sosyalGuvenceServisi = sosyalGuvenceServisi;
        _gunlukIzinServisi = gunlukIzinServisi;
        _emanetServisi = emanetServisi;
        _belgeServisi = belgeServisi;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _yakinOlusturDogrulayici = yakinOlusturDogrulayici;
        _yakinGuncelleDogrulayici = yakinGuncelleDogrulayici;
        _vasiOlusturDogrulayici = vasiOlusturDogrulayici;
        _vasiGuncelleDogrulayici = vasiGuncelleDogrulayici;
        _mirasciOlusturDogrulayici = mirasciOlusturDogrulayici;
        _mirasciGuncelleDogrulayici = mirasciGuncelleDogrulayici;
        _malDogrulayici = malDogrulayici;
        _gelirDogrulayici = gelirDogrulayici;
        _sosyalGuvenceDogrulayici = sosyalGuvenceDogrulayici;
        _gunlukIzinDogrulayici = gunlukIzinDogrulayici;
        _emanetDogrulayici = emanetDogrulayici;
        _belgeYukleDogrulayici = belgeYukleDogrulayici;
        _belgeGuncelleDogrulayici = belgeGuncelleDogrulayici;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiYanit<List<SakinListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSakinler(CancellationToken ct)
    {
        var result = await _sakinService.TumunuGetirAsync(ct);
        return Ok(result);
    }

    [HttpGet("kabul-muayeneleri")]
    [ProducesResponseType(typeof(ApiYanit<List<KabulMuayeneDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKabulMuayeneleri([FromQuery] string? arama, CancellationToken ct) =>
        Ok(await _sakinService.KabulMuayeneleriniGetirAsync(arama, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<SakinDetayDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSakin(int id, CancellationToken ct)
    {
        var result = await _sakinService.GetirAsync(id, ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiYanit<SakinListDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostSakin(SakinOlusturIstek request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var result = await _sakinService.OlusturAsync(request, ct);
        return Created(result, "Sakin kaydı oluşturuldu.");
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutSakin(int id, SakinGuncelleIstek request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        await _sakinService.GuncelleAsync(id, request, ct);
        return Ok<object?>(null, "Sakin bilgileri başarıyla güncellendi.");
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSakin(int id, CancellationToken ct)
    {
        await _sakinService.SilAsync(id, ct);
        return Ok<object?>(null, "Sakin başarıyla silindi ve tahsisli yatağı boşaltıldı.");
    }

    [HttpGet("{sakinId:int}/yakinlar")]
    [ProducesResponseType(typeof(ApiYanit<List<YakinDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYakinlar(int sakinId, CancellationToken ct)
    {
        var result = await _sakinService.YakinlariGetirAsync(sakinId, ct);
        return Ok(result);
    }

    [HttpPost("{sakinId:int}/yakinlar")]
    [ProducesResponseType(typeof(ApiYanit<YakinDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostYakin(int sakinId, YakinOlusturIstek request, CancellationToken ct)
    {
        await _yakinOlusturDogrulayici.ValidateAndThrowAsync(request, ct);
        var result = await _sakinService.YakinOlusturAsync(sakinId, request, ct);
        return Created(result, "Yakın kaydı oluşturuldu.");
    }

    [HttpPut("{sakinId:int}/yakinlar/{yakinId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutYakin(int sakinId, int yakinId, YakinGuncelleIstek request, CancellationToken ct)
    {
        await _yakinGuncelleDogrulayici.ValidateAndThrowAsync(request, ct);
        await _sakinService.YakinGuncelleAsync(sakinId, yakinId, request, ct);
        return Ok<object?>(null, "Yakın kaydı güncellendi.");
    }

    [HttpDelete("{sakinId:int}/yakinlar/{yakinId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteYakin(int sakinId, int yakinId, CancellationToken ct)
    {
        await _sakinService.YakinSilAsync(sakinId, yakinId, ct);
        return Ok<object?>(null, "Yakın kaydı silindi.");
    }

    [HttpGet("{sakinId:int}/vasiler")]
    [ProducesResponseType(typeof(ApiYanit<List<VasiDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVasiler(int sakinId, CancellationToken ct)
    {
        return Ok(await _vasiServisi.TumunuGetirAsync(sakinId, ct));
    }

    [HttpPost("{sakinId:int}/vasiler")]
    [ProducesResponseType(typeof(ApiYanit<VasiDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostVasi(int sakinId, VasiOlusturIstek istek, CancellationToken ct)
    {
        await _vasiOlusturDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _vasiServisi.OlusturAsync(sakinId, istek, ct);
        return Created(sonuc, "Vasi kaydı oluşturuldu.");
    }

    [HttpPut("{sakinId:int}/vasiler/{vasiId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutVasi(int sakinId, int vasiId, VasiGuncelleIstek istek, CancellationToken ct)
    {
        await _vasiGuncelleDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _vasiServisi.GuncelleAsync(sakinId, vasiId, istek, ct);
        return Ok<object?>(null, "Vasi kaydı güncellendi.");
    }

    [HttpDelete("{sakinId:int}/vasiler/{vasiId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteVasi(int sakinId, int vasiId, CancellationToken ct)
    {
        await _vasiServisi.SilAsync(sakinId, vasiId, ct);
        return Ok<object?>(null, "Vasi kaydı silindi.");
    }

    [HttpGet("{sakinId:int}/mirascilar")]
    [ProducesResponseType(typeof(ApiYanit<List<MirasciDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMirascilar(int sakinId, CancellationToken ct)
    {
        return Ok(await _mirasciServisi.TumunuGetirAsync(sakinId, ct));
    }

    [HttpPost("{sakinId:int}/mirascilar")]
    [ProducesResponseType(typeof(ApiYanit<MirasciDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostMirasci(int sakinId, MirasciOlusturIstek istek, CancellationToken ct)
    {
        await _mirasciOlusturDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _mirasciServisi.OlusturAsync(sakinId, istek, ct);
        return Created(sonuc, "Mirasçı kaydı oluşturuldu.");
    }

    [HttpPut("{sakinId:int}/mirascilar/{mirasciId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutMirasci(int sakinId, int mirasciId, MirasciGuncelleIstek istek, CancellationToken ct)
    {
        await _mirasciGuncelleDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _mirasciServisi.GuncelleAsync(sakinId, mirasciId, istek, ct);
        return Ok<object?>(null, "Mirasçı kaydı güncellendi.");
    }

    [HttpDelete("{sakinId:int}/mirascilar/{mirasciId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMirasci(int sakinId, int mirasciId, CancellationToken ct)
    {
        await _mirasciServisi.SilAsync(sakinId, mirasciId, ct);
        return Ok<object?>(null, "Mirasçı kaydı silindi.");
    }

    [HttpGet("{sakinId:int}/mallar")]
    [ProducesResponseType(typeof(ApiYanit<List<SakinMalDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMallar(int sakinId, CancellationToken ct)
    {
        return Ok(await _malServisi.TumunuGetirAsync(sakinId, ct));
    }

    [HttpPost("{sakinId:int}/mallar")]
    [ProducesResponseType(typeof(ApiYanit<SakinMalDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostMal(int sakinId, SakinMalIstek istek, CancellationToken ct)
    {
        await _malDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _malServisi.OlusturAsync(sakinId, istek, ct);
        return Created(sonuc, "Mal kaydı oluşturuldu.");
    }

    [HttpPut("{sakinId:int}/mallar/{malId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutMal(int sakinId, int malId, SakinMalIstek istek, CancellationToken ct)
    {
        await _malDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _malServisi.GuncelleAsync(sakinId, malId, istek, ct);
        return Ok<object?>(null, "Mal kaydı güncellendi.");
    }

    [HttpDelete("{sakinId:int}/mallar/{malId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMal(int sakinId, int malId, CancellationToken ct)
    {
        await _malServisi.SilAsync(sakinId, malId, ct);
        return Ok<object?>(null, "Mal kaydı silindi.");
    }

    [HttpGet("{sakinId:int}/gelirler")]
    [ProducesResponseType(typeof(ApiYanit<List<SakinGelirDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGelirler(int sakinId, CancellationToken ct)
    {
        return Ok(await _gelirServisi.TumunuGetirAsync(sakinId, ct));
    }

    [HttpPost("{sakinId:int}/gelirler")]
    [ProducesResponseType(typeof(ApiYanit<SakinGelirDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostGelir(int sakinId, SakinGelirIstek istek, CancellationToken ct)
    {
        await _gelirDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _gelirServisi.OlusturAsync(sakinId, istek, ct);
        return Created(sonuc, "Gelir kaydı oluşturuldu.");
    }

    [HttpPut("{sakinId:int}/gelirler/{gelirId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutGelir(int sakinId, int gelirId, SakinGelirIstek istek, CancellationToken ct)
    {
        await _gelirDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _gelirServisi.GuncelleAsync(sakinId, gelirId, istek, ct);
        return Ok<object?>(null, "Gelir kaydı güncellendi.");
    }

    [HttpDelete("{sakinId:int}/gelirler/{gelirId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteGelir(int sakinId, int gelirId, CancellationToken ct)
    {
        await _gelirServisi.SilAsync(sakinId, gelirId, ct);
        return Ok<object?>(null, "Gelir kaydı silindi.");
    }

    [HttpGet("{sakinId:int}/sosyal-guvenceler")]
    [ProducesResponseType(typeof(ApiYanit<List<SakinSosyalGuvenceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSosyalGuvenceler(int sakinId, CancellationToken ct)
    {
        return Ok(await _sosyalGuvenceServisi.TumunuGetirAsync(sakinId, ct));
    }

    [HttpPost("{sakinId:int}/sosyal-guvenceler")]
    [ProducesResponseType(typeof(ApiYanit<SakinSosyalGuvenceDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostSosyalGuvence(int sakinId, SakinSosyalGuvenceIstek istek, CancellationToken ct)
    {
        await _sosyalGuvenceDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _sosyalGuvenceServisi.OlusturAsync(sakinId, istek, ct);
        return Created(sonuc, "Sosyal güvence kaydı oluşturuldu.");
    }

    [HttpPut("{sakinId:int}/sosyal-guvenceler/{guvenceId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutSosyalGuvence(int sakinId, int guvenceId, SakinSosyalGuvenceIstek istek, CancellationToken ct)
    {
        await _sosyalGuvenceDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _sosyalGuvenceServisi.GuncelleAsync(sakinId, guvenceId, istek, ct);
        return Ok<object?>(null, "Sosyal güvence kaydı güncellendi.");
    }

    [HttpDelete("{sakinId:int}/sosyal-guvenceler/{guvenceId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSosyalGuvence(int sakinId, int guvenceId, CancellationToken ct)
    {
        await _sosyalGuvenceServisi.SilAsync(sakinId, guvenceId, ct);
        return Ok<object?>(null, "Sosyal güvence kaydı silindi.");
    }

    [HttpGet("{sakinId:int}/gunluk-izinler")]
    [ProducesResponseType(typeof(ApiYanit<List<SakinGunlukIzinDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGunlukIzinler(int sakinId, CancellationToken ct)
    {
        return Ok(await _gunlukIzinServisi.TumunuGetirAsync(sakinId, ct));
    }

    [HttpPost("{sakinId:int}/gunluk-izinler")]
    [ProducesResponseType(typeof(ApiYanit<SakinGunlukIzinDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostGunlukIzin(int sakinId, SakinGunlukIzinIstek istek, CancellationToken ct)
    {
        await _gunlukIzinDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _gunlukIzinServisi.OlusturAsync(sakinId, istek, ct);
        return Created(sonuc, "Günlük izin kaydı oluşturuldu.");
    }

    [HttpPut("{sakinId:int}/gunluk-izinler/{izinId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutGunlukIzin(int sakinId, int izinId, SakinGunlukIzinIstek istek, CancellationToken ct)
    {
        await _gunlukIzinDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _gunlukIzinServisi.GuncelleAsync(sakinId, izinId, istek, ct);
        return Ok<object?>(null, "Günlük izin kaydı güncellendi.");
    }

    [HttpDelete("{sakinId:int}/gunluk-izinler/{izinId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteGunlukIzin(int sakinId, int izinId, CancellationToken ct)
    {
        await _gunlukIzinServisi.SilAsync(sakinId, izinId, ct);
        return Ok<object?>(null, "Günlük izin kaydı silindi.");
    }

    [HttpGet("{sakinId:int}/emanetler")]
    [ProducesResponseType(typeof(ApiYanit<List<SakinEmanetDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmanetler(int sakinId, CancellationToken ct)
    {
        return Ok(await _emanetServisi.TumunuGetirAsync(sakinId, ct));
    }

    [HttpPost("{sakinId:int}/emanetler")]
    [ProducesResponseType(typeof(ApiYanit<SakinEmanetDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostEmanet(int sakinId, SakinEmanetIstek istek, CancellationToken ct)
    {
        await _emanetDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _emanetServisi.OlusturAsync(sakinId, istek, ct);
        return Created(sonuc, "Emanet kaydı oluşturuldu.");
    }

    [HttpPut("{sakinId:int}/emanetler/{emanetId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutEmanet(int sakinId, int emanetId, SakinEmanetIstek istek, CancellationToken ct)
    {
        await _emanetDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _emanetServisi.GuncelleAsync(sakinId, emanetId, istek, ct);
        return Ok<object?>(null, "Emanet kaydı güncellendi.");
    }

    [HttpDelete("{sakinId:int}/emanetler/{emanetId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteEmanet(int sakinId, int emanetId, CancellationToken ct)
    {
        await _emanetServisi.SilAsync(sakinId, emanetId, ct);
        return Ok<object?>(null, "Emanet kaydı silindi.");
    }

    [HttpGet("{sakinId:int}/foto")]
    public async Task<IActionResult> GetFoto(int sakinId, CancellationToken ct)
    {
        var dosya = await _belgeServisi.FotoGetirAsync(sakinId, ct);
        return File(dosya.Icerik, dosya.IcerikTipi, dosya.IndirmeAdi);
    }

    [HttpPost("{sakinId:int}/foto")]
    [RequestSizeLimit(3_000_000)]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PostFoto(int sakinId, IFormFile dosya, CancellationToken ct)
    {
        if (dosya is null || dosya.Length == 0)
        {
            throw new GecersizIstekHatasi("Fotoğraf dosyası seçilmedi.");
        }

        await using var akis = dosya.OpenReadStream();
        await _belgeServisi.FotoYukleAsync(sakinId, dosya.FileName, dosya.ContentType, dosya.Length, akis, ct);
        return Ok<object?>(null, "Fotoğraf yüklendi.");
    }

    [HttpDelete("{sakinId:int}/foto")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteFoto(int sakinId, CancellationToken ct)
    {
        await _belgeServisi.FotoSilAsync(sakinId, ct);
        return Ok<object?>(null, "Fotoğraf silindi.");
    }

    [HttpGet("{sakinId:int}/belgeler")]
    [ProducesResponseType(typeof(ApiYanit<List<SakinBelgeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBelgeler(int sakinId, [FromQuery] string? grup, CancellationToken ct)
    {
        return Ok(await _belgeServisi.BelgeleriGetirAsync(sakinId, grup, ct));
    }

    [HttpPost("{sakinId:int}/belgeler")]
    [RequestSizeLimit(12_000_000)]
    [ProducesResponseType(typeof(ApiYanit<SakinBelgeDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostBelge(int sakinId, [FromForm] BelgeYukleFormu form, CancellationToken ct)
    {
        if (form.Dosya is null || form.Dosya.Length == 0)
        {
            throw new GecersizIstekHatasi("Yüklenecek dosya seçilmedi.");
        }

        var istek = new BelgeYukleIstek(form.Grup, form.BelgeTuru, form.BelgeTarihi, form.GecerlilikTarihi, form.Aciklama, form.AktifMi);
        await _belgeYukleDogrulayici.ValidateAndThrowAsync(istek, ct);
        await using var akis = form.Dosya.OpenReadStream();
        var sonuc = await _belgeServisi.BelgeYukleAsync(
            sakinId,
            istek,
            form.Dosya.FileName,
            form.Dosya.ContentType,
            form.Dosya.Length,
            akis,
            ct);
        return Created(sonuc, "Belge yüklendi.");
    }

    [HttpPut("{sakinId:int}/belgeler/{belgeId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutBelge(int sakinId, int belgeId, BelgeGuncelleIstek istek, CancellationToken ct)
    {
        await _belgeGuncelleDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _belgeServisi.BelgeGuncelleAsync(sakinId, belgeId, istek, ct);
        return Ok<object?>(null, "Belge kaydı güncellendi.");
    }

    [HttpDelete("{sakinId:int}/belgeler/{belgeId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteBelge(int sakinId, int belgeId, CancellationToken ct)
    {
        await _belgeServisi.BelgeSilAsync(sakinId, belgeId, ct);
        return Ok<object?>(null, "Belge silindi.");
    }

    [HttpGet("{sakinId:int}/belgeler/{belgeId:int}/dosya")]
    public async Task<IActionResult> GetBelgeDosyasi(int sakinId, int belgeId, CancellationToken ct)
    {
        var dosya = await _belgeServisi.BelgeDosyasiGetirAsync(sakinId, belgeId, ct);
        return File(dosya.Icerik, dosya.IcerikTipi, dosya.IndirmeAdi);
    }
}

public class BelgeYukleFormu
{
    public string Grup { get; set; } = "Genel";
    public string BelgeTuru { get; set; } = string.Empty;
    public DateTime? BelgeTarihi { get; set; }
    public DateTime? GecerlilikTarihi { get; set; }
    public string? Aciklama { get; set; }
    public bool AktifMi { get; set; } = true;
    public IFormFile? Dosya { get; set; }
}
