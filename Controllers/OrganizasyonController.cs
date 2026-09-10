using FluentValidation;
using Huzurevi.Application.Features.Kurum;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("organizasyon")]
public class OrganizasyonController : TemelApiController
{
    private readonly IOrganizasyonServisi _servis;
    private readonly IValidator<OrganizasyonBirimIstek> _birim;
    private readonly IValidator<PersonelAtamaIstek> _atama;

    public OrganizasyonController(IOrganizasyonServisi servis, IValidator<OrganizasyonBirimIstek> birim, IValidator<PersonelAtamaIstek> atama)
    {
        _servis = servis;
        _birim = birim;
        _atama = atama;
    }

    [HttpGet("birimler")]
    public async Task<IActionResult> Birimler([FromQuery] int? kurulusId, CancellationToken ct) => Ok(await _servis.BirimlerAsync(kurulusId, ct));

    [HttpPost("birimler")]
    public async Task<IActionResult> BirimPost(OrganizasyonBirimIstek istek, CancellationToken ct)
    {
        await _birim.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.BirimOlusturAsync(istek, ct), "Birim eklendi.");
    }

    [HttpPut("birimler/{id:int}")]
    public async Task<IActionResult> BirimPut(int id, OrganizasyonBirimIstek istek, CancellationToken ct)
    {
        await _birim.ValidateAndThrowAsync(istek, ct);
        await _servis.BirimGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Birim güncellendi.");
    }

    [HttpDelete("birimler/{id:int}")]
    public async Task<IActionResult> BirimDel(int id, CancellationToken ct)
    {
        await _servis.BirimSilAsync(id, ct);
        return Ok<object?>(null, "Birim silindi.");
    }

    [HttpGet("atamalar")]
    public async Task<IActionResult> Atamalar([FromQuery] int? birimId, [FromQuery] int? personelId, CancellationToken ct) =>
        Ok(await _servis.AtamalarAsync(birimId, personelId, ct));

    [HttpPost("atamalar")]
    public async Task<IActionResult> AtamaPost(PersonelAtamaIstek istek, CancellationToken ct)
    {
        await _atama.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.AtamaOlusturAsync(istek, ct), "Atama eklendi.");
    }

    [HttpPut("atamalar/{id:int}")]
    public async Task<IActionResult> AtamaPut(int id, PersonelAtamaIstek istek, CancellationToken ct)
    {
        await _atama.ValidateAndThrowAsync(istek, ct);
        await _servis.AtamaGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Atama güncellendi.");
    }

    [HttpDelete("atamalar/{id:int}")]
    public async Task<IActionResult> AtamaDel(int id, CancellationToken ct)
    {
        await _servis.AtamaSilAsync(id, ct);
        return Ok<object?>(null, "Atama kaldırıldı.");
    }
}
