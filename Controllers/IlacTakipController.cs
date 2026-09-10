using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.IlacTakipleri;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("saglik")]
public class IlacTakipController : TemelApiController
{
    private readonly IIlacTakipServisi _ilacTakipService;
    private readonly IValidator<IlacTakipOlusturIstek> _createValidator;

    public IlacTakipController(
        IIlacTakipServisi ilacTakipService,
        IValidator<IlacTakipOlusturIstek> createValidator)
    {
        _ilacTakipService = ilacTakipService;
        _createValidator = createValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiYanit<List<IlacTakipDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _ilacTakipService.TumunuGetirAsync(ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiYanit<IlacTakipDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(IlacTakipOlusturIstek request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var result = await _ilacTakipService.OlusturAsync(request, ct);
        return Created(result, "İlaç planı eklendi.");
    }

    [HttpPost("durum-degistir/{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<IlacTakipDurumDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DurumDegistir(int id, CancellationToken ct)
    {
        var result = await _ilacTakipService.DurumDegistirAsync(id, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _ilacTakipService.SilAsync(id, ct);
        return Ok<object?>(null, "İlaç takip kaydı silindi.");
    }
}
