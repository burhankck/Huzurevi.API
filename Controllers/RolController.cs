using FluentValidation;
using Huzurevi.Application.Features.Yetkiler;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("rol")]
public class RolController : TemelApiController
{
    private readonly IRolServisi _servis;
    private readonly IValidator<RolIstek> _dogrulayici;

    public RolController(IRolServisi servis, IValidator<RolIstek> dogrulayici)
    {
        _servis = servis;
        _dogrulayici = dogrulayici;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await _servis.ListeleAsync(ct));

    [HttpGet("katalog")]
    public async Task<IActionResult> Katalog(CancellationToken ct) => Ok(await _servis.KatalogAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetId(int id, CancellationToken ct) => Ok(await _servis.GetirAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Post(RolIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OlusturAsync(istek, ct), "Rol eklendi.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, RolIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Rol güncellendi.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Rol silindi.");
    }
}
