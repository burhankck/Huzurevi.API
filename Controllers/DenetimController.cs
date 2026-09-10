using Huzurevi.Application.Features.Denetim;
using Huzurevi.API.Yetkilendirme;
using Huzurevi.Application.Features.Yetkiler;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("log")]
public class DenetimController : TemelApiController
{
    private readonly IDenetimServisi _servis;
    public DenetimController(IDenetimServisi servis) => _servis = servis;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? tur, [FromQuery] DateTime? baslangic, [FromQuery] DateTime? bitis, [FromQuery] string? q, CancellationToken ct) =>
        Ok(await _servis.ListeleAsync(tur, baslangic, bitis, q, ct));

    [HttpPost("tasfiye")]
    public async Task<IActionResult> Tasfiye(CancellationToken ct)
    {
        var rol = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");
        if (!string.Equals(rol, IzinKatalogu.Yonetici, StringComparison.OrdinalIgnoreCase))
            return StatusCode(403, Huzurevi.Application.Common.Models.ApiYanit<object?>.Basarisiz("Tasfiye yalnızca yönetici yapabilir."));
        var adet = await _servis.TasfiyeAsync(ct);
        return Ok(new { adet }, $"{adet} kayıt tasfiye edildi.");
    }
}
