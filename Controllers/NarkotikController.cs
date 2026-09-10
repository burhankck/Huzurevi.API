using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.KurumSaglik;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("saglik")]
public class NarkotikController : TemelApiController
{
    private readonly INarkotikServisi _servis;
    private readonly IValidator<NarkotikIlacIstek> _ilacDogrulayici;
    private readonly IValidator<NarkotikHareketIstek> _hareketDogrulayici;

    public NarkotikController(INarkotikServisi servis, IValidator<NarkotikIlacIstek> ilacDogrulayici, IValidator<NarkotikHareketIstek> hareketDogrulayici)
    {
        _servis = servis;
        _ilacDogrulayici = ilacDogrulayici;
        _hareketDogrulayici = hareketDogrulayici;
    }

    [HttpGet("ilaclar")]
    [ProducesResponseType(typeof(ApiYanit<List<NarkotikIlacDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIlaclar(CancellationToken ct) => Ok(await _servis.IlaclariGetirAsync(ct));

    [HttpPost("ilaclar")]
    public async Task<IActionResult> PostIlac(NarkotikIlacIstek istek, CancellationToken ct)
    {
        await _ilacDogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.IlacOlusturAsync(istek, ct), "İlaç tanımı eklendi.");
    }

    [HttpPut("ilaclar/{id:int}")]
    public async Task<IActionResult> PutIlac(int id, NarkotikIlacIstek istek, CancellationToken ct)
    {
        await _ilacDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.IlacGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "İlaç tanımı güncellendi.");
    }

    [HttpDelete("ilaclar/{id:int}")]
    public async Task<IActionResult> DeleteIlac(int id, CancellationToken ct)
    {
        await _servis.IlacSilAsync(id, ct);
        return Ok<object?>(null, "İlaç tanımı silindi.");
    }

    [HttpGet("hareketler")]
    public async Task<IActionResult> GetHareketler([FromQuery] int? ilacId, [FromQuery] DateTime? baslangic, [FromQuery] DateTime? bitis, CancellationToken ct) =>
        Ok(await _servis.HareketleriGetirAsync(ilacId, baslangic, bitis, ct));

    [HttpPost("hareketler")]
    public async Task<IActionResult> PostHareket(NarkotikHareketIstek istek, CancellationToken ct)
    {
        await _hareketDogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.HareketOlusturAsync(istek, ct), "Hareket kaydedildi.");
    }
}
