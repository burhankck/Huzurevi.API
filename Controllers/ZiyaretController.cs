using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.Ziyaretler;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("ziyaret")]
public class ZiyaretController : TemelApiController
{
    private readonly IZiyaretServisi _ziyaretServisi;
    private readonly IValidator<ZiyaretGirisIstek> _girisDogrulayici;

    public ZiyaretController(IZiyaretServisi ziyaretServisi, IValidator<ZiyaretGirisIstek> girisDogrulayici)
    {
        _ziyaretServisi = ziyaretServisi;
        _girisDogrulayici = girisDogrulayici;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiYanit<List<ZiyaretDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetZiyaretler([FromQuery] int? sakinId, [FromQuery] bool? iceride, CancellationToken ct)
    {
        return Ok(await _ziyaretServisi.TumunuGetirAsync(sakinId, iceride, ct));
    }

    [HttpGet("iceride")]
    [ProducesResponseType(typeof(ApiYanit<List<ZiyaretDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIceridekiler(CancellationToken ct)
    {
        return Ok(await _ziyaretServisi.IceridekileriGetirAsync(ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiYanit<ZiyaretDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostGiris(ZiyaretGirisIstek istek, CancellationToken ct)
    {
        await _girisDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _ziyaretServisi.GirisYapAsync(istek, ct);
        return Created(sonuc, "Ziyaretçi girişi kaydedildi.");
    }

    [HttpPost("{id:int}/cikis")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PostCikis(int id, CancellationToken ct)
    {
        await _ziyaretServisi.CikisYapAsync(id, ct);
        return Ok<object?>(null, "Ziyaretçi çıkışı kaydedildi.");
    }
}
