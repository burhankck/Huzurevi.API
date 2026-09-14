using Huzurevi.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Huzurevi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class TemelApiController : ControllerBase
{
    protected int? OturumKullaniciId
    {
        get
        {
            var deger = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            return int.TryParse(deger, out var id) ? id : null;
        }
    }

    protected int? OturumKurulusId
    {
        get
        {
            var deger = User.FindFirstValue("kurulusId");
            return int.TryParse(deger, out var id) ? id : null;
        }
    }

    protected string? OturumRol =>
        User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");

    protected static ObjectResult Ok<T>(T veri, string? mesaj = null) =>
        new(ApiYanit<T>.BasariliSonuc(veri, mesaj)) { StatusCode = 200 };

    protected static ObjectResult Created<T>(T veri, string? mesaj = null) =>
        new(ApiYanit<T>.BasariliSonuc(veri, mesaj)) { StatusCode = 201 };
}
