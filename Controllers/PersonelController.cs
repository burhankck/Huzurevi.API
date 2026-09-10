using FluentValidation;
using Huzurevi.Application.Features.Kurum;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("personel")]
public class PersonelController : TemelApiController
{
    private readonly IPersonelServisi _servis;
    private readonly IValidator<PersonelIstek> _dogrulayici;
    private readonly IValidator<PersonelYakinIstek> _yakin;

    public PersonelController(IPersonelServisi servis, IValidator<PersonelIstek> dogrulayici, IValidator<PersonelYakinIstek> yakin)
    {
        _servis = servis;
        _dogrulayici = dogrulayici;
        _yakin = yakin;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? kurulusId, CancellationToken ct) => Ok(await _servis.ListeleAsync(kurulusId, ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetId(int id, CancellationToken ct) => Ok(await _servis.GetirAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Post(PersonelIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OlusturAsync(istek, ct), "Personel eklendi.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, PersonelIstek istek, CancellationToken ct)
    {
        await _dogrulayici.ValidateAndThrowAsync(istek, ct);
        await _servis.GuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Personel güncellendi.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _servis.SilAsync(id, ct);
        return Ok<object?>(null, "Personel silindi.");
    }

    [HttpPost("{id:int}/foto")]
    public async Task<IActionResult> Foto(int id, IFormFile dosya, CancellationToken ct)
    {
        if (dosya is null || dosya.Length == 0) return BadRequest("Dosya gerekli.");
        await using var akis = dosya.OpenReadStream();
        await _servis.FotoYukleAsync(id, dosya.FileName, dosya.ContentType, akis, ct);
        return Ok<object?>(null, "Fotoğraf yüklendi.");
    }

    [HttpGet("{id:int}/foto")]
    public async Task<IActionResult> FotoGetir(int id, CancellationToken ct)
    {
        var dosya = await _servis.FotoGetirAsync(id, ct);
        return File(dosya.Icerik, dosya.IcerikTipi, dosya.IndirmeAdi);
    }

    [HttpGet("{id:int}/yakinlar")]
    public async Task<IActionResult> Yakinlar(int id, CancellationToken ct) => Ok(await _servis.YakinlarAsync(id, ct));

    [HttpPost("{id:int}/yakinlar")]
    public async Task<IActionResult> YakinPost(int id, PersonelYakinIstek istek, CancellationToken ct)
    {
        await _yakin.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.YakinOlusturAsync(id, istek, ct), "Yakın eklendi.");
    }

    [HttpPut("{id:int}/yakinlar/{yakinId:int}")]
    public async Task<IActionResult> YakinPut(int id, int yakinId, PersonelYakinIstek istek, CancellationToken ct)
    {
        await _yakin.ValidateAndThrowAsync(istek, ct);
        await _servis.YakinGuncelleAsync(id, yakinId, istek, ct);
        return Ok<object?>(null, "Yakın güncellendi.");
    }

    [HttpDelete("{id:int}/yakinlar/{yakinId:int}")]
    public async Task<IActionResult> YakinDel(int id, int yakinId, CancellationToken ct)
    {
        await _servis.YakinSilAsync(id, yakinId, ct);
        return Ok<object?>(null, "Yakın silindi.");
    }

    [HttpGet("{id:int}/belgeler")]
    public async Task<IActionResult> Belgeler(int id, CancellationToken ct) => Ok(await _servis.BelgelerAsync(id, ct));

    [HttpPost("{id:int}/belgeler")]
    public async Task<IActionResult> BelgePost(int id, IFormFile dosya, [FromForm] string belgeTuru, [FromForm] string? aciklama, CancellationToken ct)
    {
        if (dosya is null || dosya.Length == 0) return BadRequest("Dosya gerekli.");
        await using var akis = dosya.OpenReadStream();
        return Created(await _servis.BelgeYukleAsync(id, belgeTuru, aciklama, dosya.FileName, dosya.ContentType, dosya.Length, akis, ct), "Belge yüklendi.");
    }

    [HttpDelete("{id:int}/belgeler/{belgeId:int}")]
    public async Task<IActionResult> BelgeDel(int id, int belgeId, CancellationToken ct)
    {
        await _servis.BelgeSilAsync(id, belgeId, ct);
        return Ok<object?>(null, "Belge silindi.");
    }

    [HttpGet("{id:int}/belgeler/{belgeId:int}/dosya")]
    public async Task<IActionResult> BelgeDosya(int id, int belgeId, CancellationToken ct)
    {
        var dosya = await _servis.BelgeGetirAsync(id, belgeId, ct);
        return File(dosya.Icerik, dosya.IcerikTipi, dosya.IndirmeAdi);
    }
}
