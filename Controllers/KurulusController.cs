using FluentValidation;
using Huzurevi.Application.Features.Kurum;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("kurulus")]
public class KurulusController : TemelApiController
{
    private readonly IKurulusServisi _servis;
    private readonly IValidator<KurulusIstek> _dogrulayici;

    public KurulusController(IKurulusServisi servis, IValidator<KurulusIstek> dogrulayici)
    {
        _servis = servis;
        _dogrulayici = dogrulayici;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await _servis.ListeleAsync(ct));

    [HttpGet("adres-varsayilan")]
    public async Task<IActionResult> Varsayilan([FromQuery] int? kurulusId, CancellationToken ct) =>
        Ok(await _servis.AdresVarsayilanAsync(kurulusId, ct));

    [HttpPost]
    public async Task<IActionResult> Post(KurulusIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OlusturAsync(istek, ct), "Kuruluş eklendi.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, KurulusIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Kuruluş güncellendi.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Kuruluş silindi.");
    }
}
