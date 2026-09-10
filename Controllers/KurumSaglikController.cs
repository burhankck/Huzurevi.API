using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.KurumSaglik;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("saglik")]
public class KurumSaglikController : TemelApiController
{
    private readonly IKurumSaglikKayitServisi _servis;
    private readonly IValidator<KurumSaglikKayitIstek> _dogrulayici;

    public KurumSaglikController(IKurumSaglikKayitServisi servis, IValidator<KurumSaglikKayitIstek> dogrulayici)
    {
        _servis = servis;
        _dogrulayici = dogrulayici;
    }

    private string Imzalayan =>
        $"{User.FindFirstValue(ClaimTypes.GivenName)} {User.FindFirstValue(ClaimTypes.Surname)}".Trim();

    [HttpGet("{tur}")]
    [ProducesResponseType(typeof(ApiYanit<List<KurumSaglikKayitDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(string tur, [FromQuery] int? sakinId, [FromQuery] DateTime? baslangic, [FromQuery] DateTime? bitis, CancellationToken ct) =>
        Ok(await _servis.GetirAsync(tur, sakinId, baslangic, bitis, ct));

    [HttpPost]
    [ProducesResponseType(typeof(ApiYanit<KurumSaglikKayitDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post(KurumSaglikKayitIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OlusturAsync(istek, ct), "Kayıt oluşturuldu.");
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Put(int id, KurumSaglikKayitIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Kayıt güncellendi.");
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Kayıt silindi.");
    }

    [HttpPost("{id:int}/imzala")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Imzala(int id, CancellationToken ct)
    {
        var ad = string.IsNullOrWhiteSpace(Imzalayan) ? User.Identity?.Name ?? "Personel" : Imzalayan;
        await _servis.ImzalaAsync(id, ad, ct);
        return Ok<object?>(null, "Kayıt imzalandı.");
    }
}
