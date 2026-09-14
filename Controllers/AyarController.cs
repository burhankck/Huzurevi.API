using FluentValidation;
using Huzurevi.Application.Features.Ayarlar;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("ayar")]
public class AyarController : TemelApiController
{
    private readonly IAyarServisi _servis;
    private readonly IValidator<UygulamaAyariIstek> _dogrulayici;

    public AyarController(IAyarServisi servis, IValidator<UygulamaAyariIstek> dogrulayici)
    {
        _servis = servis;
        _dogrulayici = dogrulayici;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await _servis.GetirAsync(ct));

    [HttpPut]
    [Yetki("ayar.duzenle")]
    public async Task<IActionResult> Put(UygulamaAyariIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(istek, ct);
        return Ok<object?>(null, "Ayarlar kaydedildi.");
    }
}
