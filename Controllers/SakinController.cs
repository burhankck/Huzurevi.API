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
    public async Task<ActionResult<IEnumerable<Sakin>>> GetSakinler()
    {
        return await _context.Sakinler.Include(s => s.Yatak).ThenInclude(y => y!.Oda).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Sakin>> PostSakin(Sakin sakin)
    {
        // Şartname Madde 4.5.40: TC Kimlik No mükerrer kayıt engeli
        bool tcVarMi = await _context.Sakinler.AnyAsync(s => s.TcKimlikNo == sakin.TcKimlikNo);
        if (tcVarMi)
            return BadRequest("Bu T.C. Kimlik Numarası ile kayıtlı bir sakin zaten mevcut.");

        // Yatak atanmışsa yatağı dolu olarak işaretle
        if (sakin.YatakId.HasValue)
        {
            var yatak = await _context.Yataklar.FindAsync(sakin.YatakId.Value);
            if (yatak != null)
            {
                if (yatak.DoluMu) return BadRequest("Seçilen yatak şu anda doludur.");
                yatak.DoluMu = true;
            }
        }

        _context.Sakinler.Add(sakin);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSakinler), new { id = sakin.Id }, sakin);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSakin(int id)
    {
        var sakin = await _context.Sakinler.FindAsync(id);
        if (sakin == null) return NotFound("Sakin bulunamadı.");

        // Şartname Madde 4.5.17 & 4.5.37: Soft Delete
        sakin.SilindiMi = true;
        sakin.SilinmeTarihi = DateTime.UtcNow;

        // Sakin silindiğinde yatağı boşalt
        if (sakin.YatakId.HasValue)
        {
            var yatak = await _context.Yataklar.FindAsync(sakin.YatakId.Value);
            if (yatak != null) yatak.DoluMu = false;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
