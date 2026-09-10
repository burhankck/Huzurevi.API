using Huzurevi.API.Data;
using Huzurevi.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class YatakController : ControllerBase
{
    private readonly AppDbContext _context;

    public YatakController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Yatak>>> GetYataklar()
    {
        return await _context.Yataklar.Include(y => y.Oda).ToListAsync();
    }

    [HttpGet("bos-yataklar")]
    public async Task<IActionResult> GetBosYataklar()
    {
        var bosYataklar = await _context.Yataklar
            .Include(y => y.Oda)
            .Where(y => !y.DoluMu)
            .Select(y => new
            {
                y.Id,
                y.YatakNumarasi,
                y.OdaId,
                OdaNumarasi = y.Oda != null ? y.Oda.OdaNumarasi : ""
            })
            .ToListAsync();

        return Ok(bosYataklar);
    }

    [HttpPost]
    public async Task<ActionResult<Yatak>> PostYatak(Yatak yatak)
    {
        var oda = await _context.Odalar.Include(o => o.Yataklar).FirstOrDefaultAsync(o => o.Id == yatak.OdaId);
        if (oda == null) return NotFound("Bağlanmak istenen oda bulunamadı.");

        if (oda.Yataklar.Count >= oda.Kapasite)
            return BadRequest("Odanın yatak kapasitesi dolmuştur.");

        _context.Yataklar.Add(yatak);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetYataklar), new { id = yatak.Id }, yatak);
    }

    [HttpPost("ata")]
    public async Task<IActionResult> YatakAta([FromBody] YatakAtamaDto dto)
    {
        var yatak = await _context.Yataklar.FindAsync(dto.YatakId);
        if (yatak == null)
            return NotFound("Yatak bulunamadı.");

        if (yatak.DoluMu)
            return BadRequest("Bu yatak zaten dolu.");

        var sakin = await _context.Sakinler.FindAsync(dto.SakinId);
        if (sakin == null)
            return NotFound("Sakin bulunamadı.");

        yatak.SakinId = dto.SakinId;
        yatak.DoluMu = true;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Yatak başarıyla sakine tahsis edildi." });
    }

    [HttpPost("bosalt/{sakinId}")]
    public async Task<IActionResult> YatakBosalt(int sakinId)
    {
        var yatak = await _context.Yataklar.FirstOrDefaultAsync(y => y.SakinId == sakinId);
        var sakin = await _context.Sakinler.FindAsync(sakinId);

        if (yatak == null && (sakin == null || sakin.YatakId == null))
            return NotFound("Bu sakine ait atanmış bir yatak bulunamadı.");

        if (yatak != null)
        {
            yatak.SakinId = null;
            yatak.DoluMu = false;
        }

        if (sakin != null)
        {
            sakin.YatakId = null;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Yatak başarıyla boşaltıldı." });
    }
}

public class YatakAtamaDto
{
    public int YatakId { get; set; }
    public int SakinId { get; set; }
}