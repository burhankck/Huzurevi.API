using FluentValidation;
using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/Sakin/{sakinId:int}")]
[YetkiKaynak("saglik")]
public class SakinSaglikController : ControllerBase
{
    private readonly ISakinOlcumServisi _olcumServisi;
    private readonly ISakinSaglikDegerlendirmeServisi _degerlendirmeServisi;
    private readonly ISakinSaglikKayitServisi _kayitServisi;
    private readonly IValidator<SakinOlcumIstek> _olcumDogrulayici;
    private readonly IValidator<SakinSaglikDegerlendirmeIstek> _degerlendirmeDogrulayici;
    private readonly IValidator<SakinSaglikKayitIstek> _kayitDogrulayici;

    public SakinSaglikController(
        ISakinOlcumServisi olcumServisi,
        ISakinSaglikDegerlendirmeServisi degerlendirmeServisi,
        ISakinSaglikKayitServisi kayitServisi,
        IValidator<SakinOlcumIstek> olcumDogrulayici,
        IValidator<SakinSaglikDegerlendirmeIstek> degerlendirmeDogrulayici,
        IValidator<SakinSaglikKayitIstek> kayitDogrulayici)
    {
        _olcumServisi = olcumServisi;
        _degerlendirmeServisi = degerlendirmeServisi;
        _kayitServisi = kayitServisi;
        _olcumDogrulayici = olcumDogrulayici;
        _degerlendirmeDogrulayici = degerlendirmeDogrulayici;
        _kayitDogrulayici = kayitDogrulayici;
    }

    private static ObjectResult Basarili<T>(T veri, int kod = 200, string? mesaj = null) =>
        new(ApiYanit<T>.BasariliSonuc(veri, mesaj)) { StatusCode = kod };

    [HttpGet("olcumler")]
    [ProducesResponseType(typeof(ApiYanit<List<SakinOlcumDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOlcumler(int sakinId, CancellationToken ct) =>
        Basarili(await _olcumServisi.TumunuGetirAsync(sakinId, ct));

    [HttpPost("olcumler")]
    [ProducesResponseType(typeof(ApiYanit<SakinOlcumDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostOlcum(int sakinId, SakinOlcumIstek istek, CancellationToken ct)
    {
        await _olcumDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _olcumServisi.OlusturAsync(sakinId, istek, ct);
        return Basarili(sonuc, 201, "Ölçüm kaydı oluşturuldu.");
    }

    [HttpPut("olcumler/{olcumId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutOlcum(int sakinId, int olcumId, SakinOlcumIstek istek, CancellationToken ct)
    {
        await _olcumDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _olcumServisi.GuncelleAsync(sakinId, olcumId, istek, ct);
        return Basarili<object?>(null, 200, "Ölçüm kaydı güncellendi.");
    }

    [HttpDelete("olcumler/{olcumId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteOlcum(int sakinId, int olcumId, CancellationToken ct)
    {
        await _olcumServisi.SilAsync(sakinId, olcumId, ct);
        return Basarili<object?>(null, 200, "Ölçüm kaydı silindi.");
    }

    [HttpGet("degerlendirmeler/{tur}")]
    [ProducesResponseType(typeof(ApiYanit<List<SakinSaglikDegerlendirmeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDegerlendirmeler(int sakinId, string tur, CancellationToken ct) =>
        Basarili(await _degerlendirmeServisi.TumunuGetirAsync(sakinId, tur, ct));

    [HttpPost("degerlendirmeler")]
    [ProducesResponseType(typeof(ApiYanit<SakinSaglikDegerlendirmeDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostDegerlendirme(int sakinId, SakinSaglikDegerlendirmeIstek istek, CancellationToken ct)
    {
        await _degerlendirmeDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _degerlendirmeServisi.OlusturAsync(sakinId, istek, ct);
        return Basarili(sonuc, 201, "Değerlendirme kaydı oluşturuldu.");
    }

    [HttpPut("degerlendirmeler/{kayitId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutDegerlendirme(int sakinId, int kayitId, SakinSaglikDegerlendirmeIstek istek, CancellationToken ct)
    {
        await _degerlendirmeDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _degerlendirmeServisi.GuncelleAsync(sakinId, kayitId, istek, ct);
        return Basarili<object?>(null, 200, "Değerlendirme kaydı güncellendi.");
    }

    [HttpDelete("degerlendirmeler/{kayitId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteDegerlendirme(int sakinId, int kayitId, CancellationToken ct)
    {
        await _degerlendirmeServisi.SilAsync(sakinId, kayitId, ct);
        return Basarili<object?>(null, 200, "Değerlendirme kaydı silindi.");
    }

    [HttpGet("saglik-kayitlari/{tur}")]
    [ProducesResponseType(typeof(ApiYanit<List<SakinSaglikKayitDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSaglikKayitlari(int sakinId, string tur, CancellationToken ct) =>
        Basarili(await _kayitServisi.TumunuGetirAsync(sakinId, tur, ct));

    [HttpPost("saglik-kayitlari")]
    [ProducesResponseType(typeof(ApiYanit<SakinSaglikKayitDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostSaglikKayit(int sakinId, SakinSaglikKayitIstek istek, CancellationToken ct)
    {
        await _kayitDogrulayici.ValidateAndThrowAsync(istek, ct);
        var sonuc = await _kayitServisi.OlusturAsync(sakinId, istek, ct);
        return Basarili(sonuc, 201, "Sağlık kaydı oluşturuldu.");
    }

    [HttpPut("saglik-kayitlari/{kayitId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutSaglikKayit(int sakinId, int kayitId, SakinSaglikKayitIstek istek, CancellationToken ct)
    {
        await _kayitDogrulayici.ValidateAndThrowAsync(istek, ct);
        await _kayitServisi.GuncelleAsync(sakinId, kayitId, istek, ct);
        return Basarili<object?>(null, 200, "Sağlık kaydı güncellendi.");
    }

    [HttpDelete("saglik-kayitlari/{kayitId:int}")]
    [ProducesResponseType(typeof(ApiYanit<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSaglikKayit(int sakinId, int kayitId, CancellationToken ct)
    {
        await _kayitServisi.SilAsync(sakinId, kayitId, ct);
        return Basarili<object?>(null, 200, "Sağlık kaydı silindi.");
    }
}
