using FluentValidation;
using Huzurevi.Application.Features.Yemekhane;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("yemekhane")]
public class YemekhaneController : TemelApiController
{
    private readonly IYemekhaneServisi _servis;
    private readonly IValidator<YemekIstek> _yemek;
    private readonly IValidator<BeslenmeProfiliIstek> _profil;
    private readonly IValidator<GunlukMenuIstek> _menu;
    private readonly IValidator<OzelMenuIstek> _ozel;
    private readonly IValidator<YemekTuketimIstek> _tuketim;
    private readonly IValidator<SiviAlimiIstek> _sivi;

    public YemekhaneController(
        IYemekhaneServisi servis,
        IValidator<YemekIstek> yemek,
        IValidator<BeslenmeProfiliIstek> profil,
        IValidator<GunlukMenuIstek> menu,
        IValidator<OzelMenuIstek> ozel,
        IValidator<YemekTuketimIstek> tuketim,
        IValidator<SiviAlimiIstek> sivi)
    {
        _servis = servis;
        _yemek = yemek;
        _profil = profil;
        _menu = menu;
        _ozel = ozel;
        _tuketim = tuketim;
        _sivi = sivi;
    }

    [HttpGet("yemekler")]
    public async Task<IActionResult> GetYemekler([FromQuery] string? kategori, [FromQuery] string? arama, CancellationToken ct) =>
        Ok(await _servis.YemekleriGetirAsync(kategori, arama, ct));

    [HttpPost("yemekler")]
    public async Task<IActionResult> PostYemek(YemekIstek istek, CancellationToken ct)
    {
        await _yemek.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.YemekOlusturAsync(istek, ct), "Yemek eklendi.");
    }

    [HttpPut("yemekler/{id:int}")]
    public async Task<IActionResult> PutYemek(int id, YemekIstek istek, CancellationToken ct)
    {
        await _yemek.ValidateAndThrowAsync(istek, ct);
        await _servis.YemekGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Yemek güncellendi.");
    }

    [HttpDelete("yemekler/{id:int}")]
    public async Task<IActionResult> DeleteYemek(int id, CancellationToken ct)
    {
        await _servis.YemekSilAsync(id, ct);
        return Ok<object?>(null, "Yemek silindi.");
    }

    [HttpGet("profiller")]
    public async Task<IActionResult> GetProfiller(CancellationToken ct) => Ok(await _servis.ProfilleriGetirAsync(ct));

    [HttpPost("profiller")]
    public async Task<IActionResult> PostProfil(BeslenmeProfiliIstek istek, CancellationToken ct)
    {
        await _profil.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.ProfilOlusturAsync(istek, ct), "Profil eklendi.");
    }

    [HttpPut("profiller/{id:int}")]
    public async Task<IActionResult> PutProfil(int id, BeslenmeProfiliIstek istek, CancellationToken ct)
    {
        await _profil.ValidateAndThrowAsync(istek, ct);
        await _servis.ProfilGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Profil güncellendi.");
    }

    [HttpDelete("profiller/{id:int}")]
    public async Task<IActionResult> DeleteProfil(int id, CancellationToken ct)
    {
        await _servis.ProfilSilAsync(id, ct);
        return Ok<object?>(null, "Profil silindi.");
    }

    [HttpGet("menuler")]
    public async Task<IActionResult> GetMenuler([FromQuery] DateTime? tarih, CancellationToken ct) =>
        Ok(await _servis.MenuleriGetirAsync(tarih, ct));

    [HttpPost("menuler")]
    public async Task<IActionResult> PostMenu(GunlukMenuIstek istek, CancellationToken ct)
    {
        await _menu.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.MenuOlusturAsync(istek, ct), "Menü eklendi.");
    }

    [HttpDelete("menuler/{id:int}")]
    public async Task<IActionResult> DeleteMenu(int id, CancellationToken ct)
    {
        await _servis.MenuSilAsync(id, ct);
        return Ok<object?>(null, "Menü kaydı silindi.");
    }

    [HttpPost("menuler/gun-sil")]
    public async Task<IActionResult> GunSil([FromQuery] DateTime tarih, CancellationToken ct)
    {
        await _servis.GunSilAsync(tarih, ct);
        return Ok<object?>(null, "Günün menüsü silindi.");
    }

    [HttpGet("ozel-menuler")]
    public async Task<IActionResult> GetOzel([FromQuery] int? sakinId, [FromQuery] DateTime? tarih, CancellationToken ct) =>
        Ok(await _servis.OzelMenuleriGetirAsync(sakinId, tarih, ct));

    [HttpPost("ozel-menuler")]
    public async Task<IActionResult> PostOzel(OzelMenuIstek istek, CancellationToken ct)
    {
        await _ozel.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.OzelMenuOlusturAsync(istek, ct), "Özel menü eklendi.");
    }

    [HttpPut("ozel-menuler/{id:int}")]
    public async Task<IActionResult> PutOzel(int id, OzelMenuIstek istek, CancellationToken ct)
    {
        await _ozel.ValidateAndThrowAsync(istek, ct);
        await _servis.OzelMenuGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Özel menü güncellendi.");
    }

    [HttpDelete("ozel-menuler/{id:int}")]
    public async Task<IActionResult> DeleteOzel(int id, CancellationToken ct)
    {
        await _servis.OzelMenuSilAsync(id, ct);
        return Ok<object?>(null, "Özel menü silindi.");
    }

    [HttpGet("tuketimler")]
    public async Task<IActionResult> GetTuketim([FromQuery] DateTime? tarih, [FromQuery] string? ogunTipi, [FromQuery] int? sakinId, CancellationToken ct) =>
        Ok(await _servis.TuketimleriGetirAsync(tarih, ogunTipi, sakinId, ct));

    [HttpPost("tuketimler")]
    public async Task<IActionResult> PostTuketim(YemekTuketimIstek istek, CancellationToken ct)
    {
        await _tuketim.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.TuketimOlusturAsync(istek, ct), "Tüketim kaydı eklendi.");
    }

    [HttpPut("tuketimler/{id:int}")]
    public async Task<IActionResult> PutTuketim(int id, YemekTuketimIstek istek, CancellationToken ct)
    {
        await _tuketim.ValidateAndThrowAsync(istek, ct);
        await _servis.TuketimGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Tüketim kaydı güncellendi.");
    }

    [HttpDelete("tuketimler/{id:int}")]
    public async Task<IActionResult> DeleteTuketim(int id, CancellationToken ct)
    {
        await _servis.TuketimSilAsync(id, ct);
        return Ok<object?>(null, "Tüketim kaydı silindi.");
    }

    [HttpPost("servis-doldur")]
    public async Task<IActionResult> ServisDoldur([FromQuery] DateTime tarih, [FromQuery] string ogunTipi, CancellationToken ct)
    {
        await _servis.ServisDoldurAsync(tarih, ogunTipi, ct);
        return Ok<object?>(null, "Servis satırları oluşturuldu.");
    }

    [HttpGet("sivilar")]
    public async Task<IActionResult> GetSivi([FromQuery] int? sakinId, [FromQuery] DateTime? tarih, CancellationToken ct) =>
        Ok(await _servis.SivlariGetirAsync(sakinId, tarih, ct));

    [HttpPost("sivilar")]
    public async Task<IActionResult> PostSivi(SiviAlimiIstek istek, CancellationToken ct)
    {
        await _sivi.ValidateAndThrowAsync(istek, ct);
        return Created(await _servis.SiviOlusturAsync(istek, ct), "Sıvı kaydı eklendi.");
    }

    [HttpPut("sivilar/{id:int}")]
    public async Task<IActionResult> PutSivi(int id, SiviAlimiIstek istek, CancellationToken ct)
    {
        await _sivi.ValidateAndThrowAsync(istek, ct);
        await _servis.SiviGuncelleAsync(id, istek, ct);
        return Ok<object?>(null, "Sıvı kaydı güncellendi.");
    }

    [HttpDelete("sivilar/{id:int}")]
    public async Task<IActionResult> DeleteSivi(int id, CancellationToken ct)
    {
        await _servis.SiviSilAsync(id, ct);
        return Ok<object?>(null, "Sıvı kaydı silindi.");
    }

    [HttpGet("ozet")]
    public async Task<IActionResult> Ozet([FromQuery] DateTime tarih, CancellationToken ct) =>
        Ok(await _servis.OzetGetirAsync(tarih, ct));
}
