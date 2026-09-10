using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.KurumSurec;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Huzurevi.API.Controllers;

public abstract class OnayliApiController : TemelApiController
{
    protected string OnaylayanAd =>
        string.IsNullOrWhiteSpace($"{User.FindFirstValue(ClaimTypes.GivenName)} {User.FindFirstValue(ClaimTypes.Surname)}".Trim())
            ? User.Identity?.Name ?? "Personel"
            : $"{User.FindFirstValue(ClaimTypes.GivenName)} {User.FindFirstValue(ClaimTypes.Surname)}".Trim();

    protected bool YoneticiMi => User.IsInRole("Yonetici");
}

[YetkiKaynak("surec")]
public class IzinSureciController : OnayliApiController
{
    private readonly IIzinSureciServisi _servis;
    private readonly IOnayYetkiServisi _yetki;
    private readonly IValidator<IzinSureciIstek> _dogrulayici;
    private readonly IValidator<OnayIstek> _onayDogrulayici;

    public IzinSureciController(IIzinSureciServisi servis, IOnayYetkiServisi yetki, IValidator<IzinSureciIstek> dogrulayici, IValidator<OnayIstek> onayDogrulayici)
    {
        _servis = servis;
        _yetki = yetki;
        _dogrulayici = dogrulayici;
        _onayDogrulayici = onayDogrulayici;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? sakinId, [FromQuery] string? onayDurumu, [FromQuery] DateTime? baslangic, [FromQuery] DateTime? bitis, CancellationToken ct) =>
        Ok(await _servis.GetirAsync(sakinId, onayDurumu, baslangic, bitis, ct));

    [HttpPost]
    public async Task<IActionResult> Post(IzinSureciIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OlusturAsync(istek, ct), "İzin kaydı oluşturuldu.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, IzinSureciIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "İzin kaydı güncellendi.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "İzin kaydı silindi.");
    }

    [HttpPost("{id:int}/onayla")]
    public async Task<IActionResult> Onayla(int id, OnayIstek istek, CancellationToken ct)
    {
        await _onayDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _yetki.YetkiDogrulaAsync(OturumKullaniciId ?? 0, YoneticiMi, OnayAlanlari.Izin, ct);
        await _servis.OnaylaAsync(id, istek, OnaylayanAd, ct);
        return Ok<object?>(null, "Onay işlemi kaydedildi.");
    }
}

[YetkiKaynak("surec")]
public class EsyaTespitController : OnayliApiController
{
    private readonly IEsyaTespitServisi _servis;
    private readonly IOnayYetkiServisi _yetki;
    private readonly IValidator<EsyaTespitIstek> _dogrulayici;
    private readonly IValidator<OnayIstek> _onayDogrulayici;

    public EsyaTespitController(IEsyaTespitServisi servis, IOnayYetkiServisi yetki, IValidator<EsyaTespitIstek> dogrulayici, IValidator<OnayIstek> onayDogrulayici)
    {
        _servis = servis;
        _yetki = yetki;
        _dogrulayici = dogrulayici;
        _onayDogrulayici = onayDogrulayici;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? sakinId, [FromQuery] string? onayDurumu, CancellationToken ct) =>
        Ok(await _servis.GetirAsync(sakinId, onayDurumu, ct));

    [HttpPost]
    public async Task<IActionResult> Post(EsyaTespitIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OlusturAsync(istek, ct), "Eşya tespiti oluşturuldu.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, EsyaTespitIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Eşya tespiti güncellendi.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Eşya tespiti silindi.");
    }

    [HttpPost("{id:int}/onayla")]
    public async Task<IActionResult> Onayla(int id, OnayIstek istek, CancellationToken ct)
    {
        await _onayDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _yetki.YetkiDogrulaAsync(OturumKullaniciId ?? 0, YoneticiMi, OnayAlanlari.Esya, ct);
        await _servis.OnaylaAsync(id, istek, OnaylayanAd, ct);
        return Ok<object?>(null, "Onay işlemi kaydedildi.");
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> Pdf([FromQuery] int sakinId, CancellationToken ct)
    {
        var pdf = await _servis.PdfUretAsync(sakinId, ct);
        return File(pdf, "application/pdf", $"esya-tespit-{sakinId}.pdf");
    }
}

[YetkiKaynak("surec")]
public class MirasciTeslimController : OnayliApiController
{
    private readonly IMirasciTeslimServisi _servis;
    private readonly IOnayYetkiServisi _yetki;
    private readonly IValidator<MirasciTeslimIstek> _dogrulayici;
    private readonly IValidator<OnayIstek> _onayDogrulayici;

    public MirasciTeslimController(IMirasciTeslimServisi servis, IOnayYetkiServisi yetki, IValidator<MirasciTeslimIstek> dogrulayici, IValidator<OnayIstek> onayDogrulayici)
    {
        _servis = servis;
        _yetki = yetki;
        _dogrulayici = dogrulayici;
        _onayDogrulayici = onayDogrulayici;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? sakinId, CancellationToken ct) => Ok(await _servis.GetirAsync(sakinId, ct));

    [HttpPost]
    public async Task<IActionResult> Post(MirasciTeslimIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OlusturAsync(istek, ct), "Teslim kaydı oluşturuldu.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, MirasciTeslimIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Teslim kaydı güncellendi.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Teslim kaydı silindi.");
    }

    [HttpPost("{id:int}/onayla")]
    public async Task<IActionResult> Onayla(int id, OnayIstek istek, CancellationToken ct)
    {
        await _onayDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _yetki.YetkiDogrulaAsync(OturumKullaniciId ?? 0, YoneticiMi, OnayAlanlari.MirasciTeslim, ct);
        await _servis.OnaylaAsync(id, istek, OnaylayanAd, ct);
        return Ok<object?>(null, "Onay işlemi kaydedildi.");
    }

    [HttpGet("{id:int}/pdf")]
    public async Task<IActionResult> Pdf(int id, CancellationToken ct)
    {
        var pdf = await _servis.PdfUretAsync(id, ct);
        return File(pdf, "application/pdf", $"teslim-tutanagi-{id}.pdf");
    }

    [HttpPost("{id:int}/evrak")]
    public async Task<IActionResult> Evrak(int id, IFormFile dosya, CancellationToken ct)
    {
        if (dosya is null || dosya.Length == 0) return BadRequest("Dosya gerekli.");
        await using var akis = dosya.OpenReadStream();
        await _servis.EvrakYukleAsync(id, dosya.FileName, dosya.ContentType, akis, ct);
        return Ok<object?>(null, "İmzalı evrak arşivlendi.");
    }

    [HttpGet("{id:int}/evrak")]
    public async Task<IActionResult> EvrakGetir(int id, CancellationToken ct)
    {
        var dosya = await _servis.EvrakGetirAsync(id, ct);
        return File(dosya.Icerik, dosya.IcerikTipi, dosya.IndirmeAdi);
    }
}

[YetkiKaynak("surec")]
public class SosyalIncelemeController : OnayliApiController
{
    private readonly ISosyalIncelemeServisi _servis;
    private readonly IOnayYetkiServisi _yetki;
    private readonly IValidator<SosyalIncelemeIstek> _dogrulayici;
    private readonly IValidator<OnayIstek> _onayDogrulayici;

    public SosyalIncelemeController(ISosyalIncelemeServisi servis, IOnayYetkiServisi yetki, IValidator<SosyalIncelemeIstek> dogrulayici, IValidator<OnayIstek> onayDogrulayici)
    {
        _servis = servis;
        _yetki = yetki;
        _dogrulayici = dogrulayici;
        _onayDogrulayici = onayDogrulayici;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? onayDurumu, [FromQuery] DateTime? baslangic, [FromQuery] DateTime? bitis, [FromQuery] string? arama, CancellationToken ct) =>
        Ok(await _servis.GetirAsync(onayDurumu, baslangic, bitis, arama, ct));

    [HttpPost]
    public async Task<IActionResult> Post(SosyalIncelemeIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OlusturAsync(istek, ct), "Sosyal inceleme oluşturuldu.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, SosyalIncelemeIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Sosyal inceleme güncellendi.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Sosyal inceleme silindi.");
    }

    [HttpPost("{id:int}/onayla")]
    public async Task<IActionResult> Onayla(int id, OnayIstek istek, CancellationToken ct)
    {
        await _onayDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _yetki.YetkiDogrulaAsync(OturumKullaniciId ?? 0, YoneticiMi, OnayAlanlari.SosyalInceleme, ct);
        await _servis.OnaylaAsync(id, istek, OnaylayanAd, ct);
        return Ok<object?>(null, "Onay işlemi kaydedildi.");
    }
}

[YetkiKaynak("surec")]
public class PsikolojikDegerlendirmeController : OnayliApiController
{
    private readonly IPsikolojikDegerlendirmeServisi _servis;
    private readonly IOnayYetkiServisi _yetki;
    private readonly IValidator<PsikolojikDegerlendirmeIstek> _dogrulayici;
    private readonly IValidator<OnayIstek> _onayDogrulayici;

    public PsikolojikDegerlendirmeController(IPsikolojikDegerlendirmeServisi servis, IOnayYetkiServisi yetki, IValidator<PsikolojikDegerlendirmeIstek> dogrulayici, IValidator<OnayIstek> onayDogrulayici)
    {
        _servis = servis;
        _yetki = yetki;
        _dogrulayici = dogrulayici;
        _onayDogrulayici = onayDogrulayici;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? sakinId, [FromQuery] string? tur, CancellationToken ct) =>
        Ok(await _servis.GetirAsync(sakinId, tur, ct));

    [HttpPost]
    public async Task<IActionResult> Post(PsikolojikDegerlendirmeIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OlusturAsync(istek, ct), "Değerlendirme oluşturuldu.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, PsikolojikDegerlendirmeIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Değerlendirme güncellendi.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Değerlendirme silindi.");
    }

    [HttpPost("{id:int}/onayla")]
    public async Task<IActionResult> Onayla(int id, OnayIstek istek, CancellationToken ct)
    {
        await _onayDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _yetki.YetkiDogrulaAsync(OturumKullaniciId ?? 0, YoneticiMi, OnayAlanlari.Psikolojik, ct);
        await _servis.OnaylaAsync(id, istek, OnaylayanAd, ct);
        return Ok<object?>(null, "Onay işlemi kaydedildi.");
    }
}

[YetkiKaynak("onay")]
public class OnayYetkisiController : OnayliApiController
{
    private readonly IOnayYetkiServisi _servis;
    private readonly IValidator<OnayYetkisiIstek> _dogrulayici;

    public OnayYetkisiController(IOnayYetkiServisi servis, IValidator<OnayYetkisiIstek> dogrulayici)
    {
        _servis = servis;
        _dogrulayici = dogrulayici;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await _servis.GetirAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Post(OnayYetkisiIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OlusturAsync(istek, ct), "Onay yetkisi eklendi.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, OnayYetkisiIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Onay yetkisi güncellendi.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Onay yetkisi silindi.");
    }
}
