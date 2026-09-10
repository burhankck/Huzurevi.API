using Huzurevi.API.Data;
using Huzurevi.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OdaController : ControllerBase
{
    private readonly AppDbContext _context;

    public OdaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Oda>>> GetOdalar()
    {
        return await _context.Odalar.Include(o => o.Yataklar).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Oda>> PostOda(Oda oda)
    {
        _context.Odalar.Add(oda);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOdalar), new { id = oda.Id }, oda);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOda(int id)
    {
        var oda = await _context.Odalar.FindAsync(id);
        if (oda == null) return NotFound("Oda bulunamadı.");

        // Şartname Madde 4.5.17: Soft Delete
        oda.SilindiMi = true;
        oda.SilinmeTarihi = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
