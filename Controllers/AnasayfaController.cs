using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.Anasayfa;
using Huzurevi.API.Yetkilendirme;
using Microsoft.AspNetCore.Mvc;

namespace Huzurevi.API.Controllers;

[YetkiKaynak("anasayfa")]
public class AnaSayfaController : TemelApiController
{
    private readonly IAnasayfaServisi _anasayfaService;

    public AnaSayfaController(IAnasayfaServisi anasayfaService)
    {
        _anasayfaService = anasayfaService;
    }

    [HttpGet("istatistikler")]
    [ProducesResponseType(typeof(ApiYanit<AnasayfaIstatistikDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIstatistikler(CancellationToken ct)
    {
        var result = await _anasayfaService.IstatistikleriGetirAsync(ct);
        return Ok(result);
    }
}
