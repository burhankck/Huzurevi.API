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

    [HttpPost]
    public async Task<ActionResult<Yatak>> PostYatak(Yatak yatak)
    {
        var oda = await _context.Odalar.Include(o => o.Yataklar).FirstOrDefaultAsync(o => o.Id == yatak.OdaId);
        if (oda == null) return NotFound("Bağlanmak istenen oda bulunamadı.");

        // Şartname kuralı: Oda kapasitesi dolmuşsa yeni yatak ekletme
        if (oda.Yataklar.Count >= oda.Kapasite)
            return BadRequest("Odanın yatak kapasitesi dolmuştur.");

        _context.Yataklar.Add(yatak);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetYataklar), new { id = yatak.Id }, yatak);
    }
}
