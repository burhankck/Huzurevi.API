using FluentValidation;
using Huzurevi.Application.Features.Kurum;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("tanim")]
public class TanimController : TemelApiController
{
    private readonly IAdresTanimServisi _servis;
    private readonly IValidator<UlkeIstek> _ulke;
    private readonly IValidator<IlIstek> _il;
    private readonly IValidator<IlceIstek> _ilce;
    private readonly IValidator<MahalleIstek> _mahalle;
    private readonly IValidator<GlobalTanimIstek> _tanim;

    public TanimController(
        IAdresTanimServisi servis,
        IValidator<UlkeIstek> ulke,
        IValidator<IlIstek> il,
        IValidator<IlceIstek> ilce,
        IValidator<MahalleIstek> mahalle,
        IValidator<GlobalTanimIstek> tanim)
    {
        _servis = servis;
        _ulke = ulke;
        _il = il;
        _ilce = ilce;
        _mahalle = mahalle;
        _tanim = tanim;
    }

    [HttpGet("ulkeler")]
    public async Task<IActionResult> Ulkeler(CancellationToken ct) => Ok(await _servis.UlkelerAsync(ct));
    [HttpPost("ulkeler")]
    public async Task<IActionResult> UlkePost(UlkeIstek istek, CancellationToken ct)
    {
        await _ulke.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.UlkeOlusturAsync(istek, ct), "Ülke eklendi.");
    }
    [HttpPut("ulkeler/{id:int}")]
    public async Task<IActionResult> UlkePut(int id, UlkeIstek istek, CancellationToken ct)
    {
        await _ulke.ValidateAndThrowAsync(istek, ct);
        await _servis.UlkeGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Ülke güncellendi.");
    }
    [HttpDelete("ulkeler/{id:int}")]
    public async Task<IActionResult> UlkeDel(int id, CancellationToken ct)
    {
        await _servis.UlkeSilAsync(id, ct);
        return Ok<object?>(null, "Ülke silindi.");
    }

    [HttpGet("iller")]
    public async Task<IActionResult> Iller([FromQuery] int? ulkeId, CancellationToken ct) => Ok(await _servis.IllerAsync(ulkeId, ct));
    [HttpPost("iller")]
    public async Task<IActionResult> IlPost(IlIstek istek, CancellationToken ct)
    {
        await _il.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.IlOlusturAsync(istek, ct), "İl eklendi.");
    }
    [HttpPut("iller/{id:int}")]
    public async Task<IActionResult> IlPut(int id, IlIstek istek, CancellationToken ct)
    {
        await _il.ValidateAndThrowAsync(istek, ct);
        await _servis.IlGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "İl güncellendi.");
    }
    [HttpDelete("iller/{id:int}")]
    public async Task<IActionResult> IlDel(int id, CancellationToken ct)
    {
        await _servis.IlSilAsync(id, ct);
        return Ok<object?>(null, "İl silindi.");
    }

    [HttpGet("ilceler")]
    public async Task<IActionResult> Ilceler([FromQuery] int? ilId, CancellationToken ct) => Ok(await _servis.IlcelerAsync(ilId, ct));
    [HttpPost("ilceler")]
    public async Task<IActionResult> IlcePost(IlceIstek istek, CancellationToken ct)
    {
        await _ilce.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.IlceOlusturAsync(istek, ct), "İlçe eklendi.");
    }
    [HttpPut("ilceler/{id:int}")]
    public async Task<IActionResult> IlcePut(int id, IlceIstek istek, CancellationToken ct)
    {
        await _ilce.ValidateAndThrowAsync(istek, ct);
        await _servis.IlceGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "İlçe güncellendi.");
    }
    [HttpDelete("ilceler/{id:int}")]
    public async Task<IActionResult> IlceDel(int id, CancellationToken ct)
    {
        await _servis.IlceSilAsync(id, ct);
        return Ok<object?>(null, "İlçe silindi.");
    }

    [HttpGet("mahalleler")]
    public async Task<IActionResult> Mahalleler([FromQuery] int? ilceId, CancellationToken ct) => Ok(await _servis.MahallelerAsync(ilceId, ct));
    [HttpPost("mahalleler")]
    public async Task<IActionResult> MahallePost(MahalleIstek istek, CancellationToken ct)
    {
        await _mahalle.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.MahalleOlusturAsync(istek, ct), "Mahalle eklendi.");
    }
    [HttpPut("mahalleler/{id:int}")]
    public async Task<IActionResult> MahallePut(int id, MahalleIstek istek, CancellationToken ct)
    {
        await _mahalle.ValidateAndThrowAsync(istek, ct);
        await _servis.MahalleGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Mahalle güncellendi.");
    }
    [HttpDelete("mahalleler/{id:int}")]
    public async Task<IActionResult> MahalleDel(int id, CancellationToken ct)
    {
        await _servis.MahalleSilAsync(id, ct);
        return Ok<object?>(null, "Mahalle silindi.");
    }

    [HttpGet("global")]
    public async Task<IActionResult> Tanimlar([FromQuery] string? kategori, CancellationToken ct) => Ok(await _servis.TanimlarAsync(kategori, ct));
    [HttpPost("global")]
    public async Task<IActionResult> TanimPost(GlobalTanimIstek istek, CancellationToken ct)
    {
        await _tanim.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.TanimOlusturAsync(istek, ct), "Tanım eklendi.");
    }
    [HttpPut("global/{id:int}")]
    public async Task<IActionResult> TanimPut(int id, GlobalTanimIstek istek, CancellationToken ct)
    {
        await _tanim.ValidateAndThrowAsync(istek, ct);
        await _servis.TanimGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Tanım güncellendi.");
    }
    [HttpDelete("global/{id:int}")]
    public async Task<IActionResult> TanimDel(int id, CancellationToken ct)
    {
        await _servis.TanimSilAsync(id, ct);
        return Ok<object?>(null, "Tanım silindi.");
    }
}
