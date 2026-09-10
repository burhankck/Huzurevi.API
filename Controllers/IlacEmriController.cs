using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.KurumSaglik;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("saglik")]
public class IlacEmriController : TemelApiController
{
    private readonly IIlacEmriServisi _servis;
    private readonly IValidator<IlacEmriIstek> _emirDogrulayici;
    private readonly IValidator<IlacUygulamaIstek> _uygulamaDogrulayici;

    public IlacEmriController(IIlacEmriServisi servis, IValidator<IlacEmriIstek> emirDogrulayici, IValidator<IlacUygulamaIstek> uygulamaDogrulayici)
    {
        _servis = servis;
        _emirDogrulayici = emirDogrulayici;
        _uygulamaDogrulayici = uygulamaDogrulayici;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiYanit<List<IlacEmriDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmirler([FromQuery] int? sakinId, [FromQuery] bool? aktifMi, CancellationToken ct) =>
        Ok(await _servis.EmirleriGetirAsync(sakinId, aktifMi, ct));

    [HttpPost]
    public async Task<IActionResult> PostEmir(IlacEmriIstek istek, CancellationToken ct)
    {
        await _emirDogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.EmirOlusturAsync(istek, ct), "İlaç emri oluşturuldu.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutEmir(int id, IlacEmriIstek istek, CancellationToken ct)
    {
        await _emirDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.EmirGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "İlaç emri güncellendi.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmir(int id, CancellationToken ct)
    {
        await _servis.EmirSilAsync(id, ct);
        return Ok<object?>(null, "İlaç emri silindi.");
    }

    [HttpGet("uygulamalar")]
    public async Task<IActionResult> GetUygulamalar([FromQuery] DateTime? tarih, [FromQuery] int? sakinId, CancellationToken ct) =>
        Ok(await _servis.UygulamalariGetirAsync(tarih, sakinId, ct));

    [HttpPost("uygulamalar")]
    public async Task<IActionResult> PostUygulama(IlacUygulamaIstek istek, CancellationToken ct)
    {
        await _uygulamaDogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.UygulamaOlusturAsync(istek, ct), "Uygulama kaydı oluşturuldu.");
    }

    [HttpPut("uygulamalar/{id:int}")]
    public async Task<IActionResult> PutUygulama(int id, IlacUygulamaIstek istek, CancellationToken ct)
    {
        await _uygulamaDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.UygulamaGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Uygulama kaydı güncellendi.");
    }

    [HttpDelete("uygulamalar/{id:int}")]
    public async Task<IActionResult> DeleteUygulama(int id, CancellationToken ct)
    {
        await _servis.UygulamaSilAsync(id, ct);
        return Ok<object?>(null, "Uygulama kaydı silindi.");
    }

    [HttpGet("gunluk-pdf")]
    public async Task<IActionResult> GunlukPdf([FromQuery] DateTime tarih, CancellationToken ct)
    {
        var pdf = await _servis.GunlukPdfUretAsync(tarih, ct);
        return File(pdf, "application/pdf", $"ilac-uygulama-{tarih:yyyy-MM-dd}.pdf");
    }
}
