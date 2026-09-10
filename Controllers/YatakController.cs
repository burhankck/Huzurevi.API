using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.Yataklar;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("oda")]
public class YatakController : TemelApiController
{
    private readonly IYatakServisi _yatakService;
    private readonly IValidator<YatakOlusturIstek> _createValidator;
    private readonly IValidator<YatakAtamaIstek> _atamaValidator;
    private readonly IValidator<YatakBosaltIstek> _bosaltValidator;

    public YatakController(
        IYatakServisi yatakService,
        IValidator<YatakOlusturIstek> createValidator,
        IValidator<YatakAtamaIstek> atamaValidator,
        IValidator<YatakBosaltIstek> bosaltValidator)
    {
        _yatakService = yatakService;
        _createValidator = createValidator;
        _atamaValidator = atamaValidator;
        _bosaltValidator = bosaltValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiYanit<List<YatakDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYataklar(CancellationToken ct)
    {
        var result = await _yatakService.TumunuGetirAsync(ct);
        return Ok(result);
    }

    [HttpGet("bos-yataklar")]
    [ProducesResponseType(typeof(ApiYanit<List<BosYatakDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBosYataklar(CancellationToken ct)
    {
        var result = await _yatakService.BosYataklariGetirAsync(ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiYanit<YatakDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostYatak(YatakOlusturIstek request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var result = await _yatakService.OlusturAsync(request, ct);
        return Created(result, "Yatak eklendi.");
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteYatak(int id, CancellationToken ct)
    {
        await _yatakService.SilAsync(id, ct);
        return Ok<object?>(null, "Yatak silindi.");
    }

    [HttpPost("ata")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> YatakAta(YatakAtamaIstek request, CancellationToken ct)
    {
        await _atamaValidator.ValidateAndThrowAsync(request, ct);
        await _yatakService.AtaAsync(request, ct);
        return Ok<object?>(null, "Yatak başarıyla sakine tahsis edildi.");
    }

    [HttpPost("bosalt/{sakinId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> YatakBosalt(int sakinId, YatakBosaltIstek istek, CancellationToken ct)
    {
        await _bosaltValidator.ValidateAndThrowAsync(istek, ct);
        await _yatakService.BosaltAsync(sakinId, istek, ct);
        return Ok<object?>(null, "Yatak başarıyla boşaltıldı.");
    }

    [HttpGet("sakin/{sakinId:int}/gecmis")]
    [ProducesResponseType(typeof(ApiYanit<List<YerlesimGecmisDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYerlesimGecmisi(int sakinId, CancellationToken ct)
    {
        return Ok(await _yatakService.GecmisiGetirAsync(sakinId, ct));
    }
}
