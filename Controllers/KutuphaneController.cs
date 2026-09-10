using FluentValidation;
using Huzurevi.Application.Features.Kutuphane;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("kutuphane")]
public class KutuphaneController : TemelApiController
{
    private readonly IKutuphaneServisi _servis;
    private readonly IValidator<DolapIstek> _dolap;
    private readonly IValidator<RafIstek> _raf;
    private readonly IValidator<KitapIstek> _kitap;
    private readonly IValidator<KitapKopyaIstek> _kopya;
    private readonly IValidator<KitapOduncIstek> _odunc;
    private readonly IValidator<KitapIadeIstek> _iade;

    public KutuphaneController(
        IKutuphaneServisi servis,
        IValidator<DolapIstek> dolap,
        IValidator<RafIstek> raf,
        IValidator<KitapIstek> kitap,
        IValidator<KitapKopyaIstek> kopya,
        IValidator<KitapOduncIstek> odunc,
        IValidator<KitapIadeIstek> iade)
    {
        _servis = servis;
        _dolap = dolap;
        _raf = raf;
        _kitap = kitap;
        _kopya = kopya;
        _odunc = odunc;
        _iade = iade;
    }

    [HttpGet("dolaplar")]
    public async Task<IActionResult> GetDolap(CancellationToken ct) => Ok(await _servis.DolaplariGetirAsync(ct));

    [HttpPost("dolaplar")]
    public async Task<IActionResult> PostDolap(DolapIstek istek, CancellationToken ct)
    {
        await _dolap.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.DolapOlusturAsync(istek, ct), "Dolap eklendi.");
    }

    [HttpPut("dolaplar/{id:int}")]
    public async Task<IActionResult> PutDolap(int id, DolapIstek istek, CancellationToken ct)
    {
        await _dolap.ValidateAndThrowAsync(istek, ct);
        await _servis.DolapGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Dolap güncellendi.");
    }

    [HttpDelete("dolaplar/{id:int}")]
    public async Task<IActionResult> DeleteDolap(int id, CancellationToken ct)
    {
        await _servis.DolapSilAsync(id, ct);
        return Ok<object?>(null, "Dolap silindi.");
    }

    [HttpGet("raflar")]
    public async Task<IActionResult> GetRaf([FromQuery] int? dolapId, CancellationToken ct) => Ok(await _servis.RaflariGetirAsync(dolapId, ct));

    [HttpPost("raflar")]
    public async Task<IActionResult> PostRaf(RafIstek istek, CancellationToken ct)
    {
        await _raf.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.RafOlusturAsync(istek, ct), "Raf eklendi.");
    }

    [HttpPut("raflar/{id:int}")]
    public async Task<IActionResult> PutRaf(int id, RafIstek istek, CancellationToken ct)
    {
        await _raf.ValidateAndThrowAsync(istek, ct);
        await _servis.RafGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Raf güncellendi.");
    }

    [HttpDelete("raflar/{id:int}")]
    public async Task<IActionResult> DeleteRaf(int id, CancellationToken ct)
    {
        await _servis.RafSilAsync(id, ct);
        return Ok<object?>(null, "Raf silindi.");
    }

    [HttpGet("kitaplar")]
    public async Task<IActionResult> GetKitap([FromQuery] string? arama, CancellationToken ct) => Ok(await _servis.KitaplariGetirAsync(arama, ct));

    [HttpPost("kitaplar")]
    public async Task<IActionResult> PostKitap(KitapIstek istek, CancellationToken ct)
    {
        await _kitap.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.KitapOlusturAsync(istek, ct), "Kitap eklendi.");
    }

    [HttpPut("kitaplar/{id:int}")]
    public async Task<IActionResult> PutKitap(int id, KitapIstek istek, CancellationToken ct)
    {
        await _kitap.ValidateAndThrowAsync(istek, ct);
        await _servis.KitapGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Kitap güncellendi.");
    }

    [HttpDelete("kitaplar/{id:int}")]
    public async Task<IActionResult> DeleteKitap(int id, CancellationToken ct)
    {
        await _servis.KitapSilAsync(id, ct);
        return Ok<object?>(null, "Kitap silindi.");
    }

    [HttpPost("kitaplar/{id:int}/kapak")]
    public async Task<IActionResult> Kapak(int id, IFormFile dosya, CancellationToken ct)
    {
        if (dosya is null || dosya.Length == 0) return BadRequest("Dosya gerekli.");
        await using var akis = dosya.OpenReadStream();
        await _servis.KapakYukleAsync(id, dosya.FileName, dosya.ContentType, akis, ct);
        return Ok<object?>(null, "Kapak yüklendi.");
    }

    [HttpGet("kitaplar/{id:int}/kapak")]
    public async Task<IActionResult> KapakGetir(int id, CancellationToken ct)
    {
        var dosya = await _servis.KapakGetirAsync(id, ct);
        return File(dosya.Icerik, dosya.IcerikTipi, dosya.IndirmeAdi);
    }

    [HttpGet("kopyalar")]
    public async Task<IActionResult> GetKopya([FromQuery] int? kitapId, [FromQuery] string? durum, CancellationToken ct) =>
        Ok(await _servis.KopyalariGetirAsync(kitapId, durum, ct));

    [HttpPost("kopyalar")]
    public async Task<IActionResult> PostKopya(KitapKopyaIstek istek, CancellationToken ct)
    {
        await _kopya.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.KopyaOlusturAsync(istek, ct), "Kopya eklendi.");
    }

    [HttpPut("kopyalar/{id:int}")]
    public async Task<IActionResult> PutKopya(int id, KitapKopyaIstek istek, CancellationToken ct)
    {
        await _kopya.ValidateAndThrowAsync(istek, ct);
        await _servis.KopyaGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Kopya güncellendi.");
    }

    [HttpDelete("kopyalar/{id:int}")]
    public async Task<IActionResult> DeleteKopya(int id, CancellationToken ct)
    {
        await _servis.KopyaSilAsync(id, ct);
        return Ok<object?>(null, "Kopya silindi.");
    }

    [HttpGet("kopyalar/{id:int}/etiket")]
    public async Task<IActionResult> Etiket(int id, CancellationToken ct)
    {
        var png = await _servis.EtiketUretAsync(id, ct);
        return File(png, "image/png", $"etiket-{id}.png");
    }

    [HttpGet("oduncler")]
    public async Task<IActionResult> GetOdunc([FromQuery] bool? gecikenler, CancellationToken ct) =>
        Ok(await _servis.OduncleriGetirAsync(gecikenler, ct));

    [HttpPost("oduncler")]
    public async Task<IActionResult> PostOdunc(KitapOduncIstek istek, CancellationToken ct)
    {
        await _odunc.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OduncOlusturAsync(istek, ct), "Ödünç verildi.");
    }

    [HttpPost("oduncler/{id:int}/iade")]
    public async Task<IActionResult> Iade(int id, KitapIadeIstek istek, CancellationToken ct)
    {
        await _iade.ValidateAndThrowAsync(istek, ct);
        await _servis.IadeEtAsync(id, istek, ct);
        return Ok<object?>(null, "İade kaydedildi.");
    }
}
