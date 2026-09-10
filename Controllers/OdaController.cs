using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.Odalar;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("oda")]
public class OdaController : TemelApiController
{
    private readonly IOdaServisi _odaService;
    private readonly IValidator<OdaOlusturIstek> _createValidator;
    private readonly IValidator<OdaGuncelleIstek> _updateValidator;

    public OdaController(
        IOdaServisi odaService,
        IValidator<OdaOlusturIstek> createValidator,
        IValidator<OdaGuncelleIstek> updateValidator)
    {
        _odaService = odaService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiYanit<List<OdaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOdalar(CancellationToken ct)
    {
        var result = await _odaService.TumunuGetirAsync(ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<OdaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOda(int id, CancellationToken ct)
    {
        var result = await _odaService.GetirAsync(id, ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiYanit<OdaDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostOda(OdaOlusturIstek request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var result = await _odaService.OlusturAsync(request, ct);
        return Created(result, "Oda eklendi.");
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutOda(int id, OdaGuncelleIstek request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        await _odaService.GuncelleAsync(id, request, ct);
        return Ok<object?>(null, "Oda bilgileri güncellendi.");
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteOda(int id, CancellationToken ct)
    {
        await _odaService.SilAsync(id, ct);
        return Ok<object?>(null, "Oda silindi.");
    }
}
