using Huzurevi.API.Data;
using Huzurevi.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SakinController : ControllerBase
{
    private readonly AppDbContext _context;

    public SakinController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
public async Task<ActionResult<IEnumerable<object>>> GetSakinler()
{
    // Yatakları sakinleriyle ve odalarıyla birlikte çekiyoruz
    var yataklar = await _context.Yataklar
        .Include(y => y.Oda)
        .Where(y => y.SakinId != null)
        .ToListAsync();

    var sakinler = await _context.Sakinler.ToListAsync();

    var sonuc = sakinler.Select(s =>
    {
        var atananYatak = yataklar.FirstOrDefault(y => y.SakinId == s.Id);
        return new
        {
            s.Id,
            s.Ad,
            s.Soyad,
            s.TcKimlikNo,
            s.Telefon,
            s.Durum,
            AktifMi = s.Durum == "Aktif",
            YatakId = atananYatak?.Id,
            YatakBilgisi = atananYatak != null && atananYatak.Oda != null
                ? $"Oda {atananYatak.Oda.OdaNumarasi} - Yatak {atananYatak.YatakNumarasi}"
                : null
        };
    });

    return Ok(sonuc);
}

    [HttpPost]
    public async Task<ActionResult<Sakin>> PostSakin(Sakin sakin)
    {
        bool tcVarMi = await _context.Sakinler.AnyAsync(s => s.TcKimlikNo == sakin.TcKimlikNo);
        if (tcVarMi)
            return BadRequest("Bu T.C. Kimlik Numarası ile kayıtlı bir sakin zaten mevcut.");

        _context.Sakinler.Add(sakin);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSakinler), new { id = sakin.Id }, sakin);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSakin(int id)
    {
        var sakin = await _context.Sakinler.Include(s => s.Yatak).FirstOrDefaultAsync(s => s.Id == id);
        if (sakin == null) return NotFound("Sakin bulunamadı.");

        sakin.SilindiMi = true;
        sakin.SilinmeTarihi = DateTime.UtcNow;

        if (sakin.Yatak != null)
        {
            sakin.Yatak.SakinId = null;
            sakin.Yatak.DoluMu = false;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
}