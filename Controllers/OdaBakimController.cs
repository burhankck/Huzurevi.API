using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.Odalar;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("oda")]
public class OdaBakimController : TemelApiController
{
    private readonly IOdaBakimServisi _servis;
    private readonly IValidator<OdaBakimIstek> _dogrulayici;

    public OdaBakimController(IOdaBakimServisi servis, IValidator<OdaBakimIstek> dogrulayici)
    {
        _servis = servis;
        _dogrulayici = dogrulayici;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiYanit<List<OdaBakimDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listele([FromQuery] int? odaId, CancellationToken ct)
    {
        return Ok(await _servis.ListeleAsync(odaId, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiYanit<OdaBakimDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Ekle(OdaBakimIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _servis.OlusturAsync(istek, ct);
        return Created(sonuc, "Bakım kaydı eklendi.");
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Guncelle(int id, OdaBakimIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Bakım kaydı güncellendi.");
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Sil(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Bakım kaydı silindi.");
    }
}
