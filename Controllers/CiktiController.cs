using Huzurevi.Application.Features.Cikti;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

public class CiktiController : TemelApiController
{
    private readonly ICiktiServisi _servis;
    public CiktiController(ICiktiServisi servis) => _servis = servis;

    [HttpGet("sakin")]
    [Yetki("sakin.aktar")]
    public async Task<IActionResult> Sakin([FromQuery] string format = "xlsx", [FromQuery] string? q = null, CancellationToken ct = default) =>
        Dosya(await _servis.SakinAsync(format, q, ct));

    [HttpGet("oda")]
    [Yetki("oda.aktar")]
    public async Task<IActionResult> Oda([FromQuery] string format = "xlsx", [FromQuery] string? blok = null, [FromQuery] int? kat = null, [FromQuery] string? durum = null, CancellationToken ct = default) =>
        Dosya(await _servis.OdaAsync(format, blok, kat, durum, ct));

    [HttpGet("ziyaret")]
    [Yetki("ziyaret.aktar")]
    public async Task<IActionResult> Ziyaret([FromQuery] string format = "xlsx", [FromQuery] bool? iceride = null, CancellationToken ct = default) =>
        Dosya(await _servis.ZiyaretAsync(format, iceride, ct));

    [HttpGet("personel")]
    [Yetki("personel.aktar")]
    public async Task<IActionResult> Personel([FromQuery] string format = "xlsx", CancellationToken ct = default) =>
        Dosya(await _servis.PersonelAsync(format, ct));

    private static FileContentResult Dosya(Huzurevi.Application.Common.Interfaces.TabloCikti dosya) =>
        new(dosya.Icerik, dosya.IcerikTipi) { FileDownloadName = dosya.DosyaAdi };
}

[YetkiKaynak("cikti")]
public class PdfSablonController : TemelApiController
{
    private readonly IPdfSablonServisi _servis;
    public PdfSablonController(IPdfSablonServisi servis) => _servis = servis;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await _servis.ListeleAsync(ct));

    [HttpGet("arsiv")]
    public async Task<IActionResult> Arsiv(CancellationToken ct) => Ok(await _servis.ArsivAsync(ct));

    [HttpGet("arsiv/{id:int}/indir")]
    public async Task<IActionResult> ArsivIndir(int id, CancellationToken ct)
    {
        var dosya = await _servis.ArsivIndirAsync(id, ct);
        return File(dosya.Icerik, dosya.IcerikTipi, dosya.IndirmeAdi);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetId(int id, CancellationToken ct) => Ok(await _servis.GetirAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Post(PdfSablonIstek istek, CancellationToken ct) =>
        Created(await _servis.OlusturAsync(istek, ct), "Şablon eklendi.");

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, PdfSablonIstek istek, CancellationToken ct)
    {
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Şablon güncellendi.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Şablon silindi.");
    }

    [HttpPost("{id:int}/uret")]
    public async Task<IActionResult> Uret(int id, PdfUretIstek istek, CancellationToken ct)
    {
        var ad = User.Identity?.Name ?? "kullanici";
        var dosya = await _servis.UretVeArsivleAsync(id, istek.SakinId, ad, ct);
        return File(dosya.Icerik, dosya.IcerikTipi, dosya.DosyaAdi);
    }
}
