using Huzurevi.Application.Features.Yedekleme;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("yedek")]
public class YedekController : TemelApiController
{
    private readonly IYedeklemeServisi _servis;
    public YedekController(IYedeklemeServisi servis) => _servis = servis;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(await _servis.ListeleAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Post(CancellationToken ct) =>
        Created(await _servis.OlusturAsync(User.Identity?.Name ?? "kullanici", "Manuel", ct), "Yedek alındı.");

    [HttpPost("{id:int}/geri-yukle")]
    public async Task<IActionResult> GeriYukle(int id, CancellationToken ct)
    {
        await _servis.GeriYukleAsync(id, User.Identity?.Name ?? "kullanici", ct);
        return Ok<object?>(null, "Geri yükleme tamamlandı. Sayfayı yenileyin.");
    }

    [HttpGet("{id:int}/indir")]
    public async Task<IActionResult> Indir(int id, [FromQuery] string parca = "veritabani", CancellationToken ct = default)
    {
        var dosya = await _servis.IndirAsync(id, parca, ct);
        return File(dosya.Icerik, dosya.IcerikTipi, dosya.IndirmeAdi);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Yedek silindi.");
    }
}
