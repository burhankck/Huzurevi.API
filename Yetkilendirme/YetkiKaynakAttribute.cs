using Huzurevi.Application.Common.Models;
using Huzurevi.Application.Features.Yetkiler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Huzurevi.API.Yetkilendirme;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class YetkiKaynakAttribute : TypeFilterAttribute
{
    public YetkiKaynakAttribute(string kaynak) : base(typeof(YetkiFiltresi))
    {
        Arguments = [kaynak];
    }
}

public sealed class YetkiFiltresi : IAsyncAuthorizationFilter
{
    private readonly string _kaynak;
    private readonly IYetkiServisi _yetki;

    public YetkiFiltresi(string kaynak, IYetkiServisi yetki)
    {
        _kaynak = kaynak;
        _yetki = yetki;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
            return;

        if (YetkiKaynakYardimcisi.MetoddaAcikYetkiVar(context))
            return;

        var rol = context.HttpContext.User.FindFirstValue(ClaimTypes.Role)
            ?? context.HttpContext.User.FindFirstValue("role");
        var metot = context.HttpContext.Request.Method.ToUpperInvariant();
        var islemler = metot switch
        {
            "GET" => new[] { "goruntule" },
            "DELETE" => new[] { "sil" },
            "PUT" or "PATCH" => new[] { "duzenle" },
            "POST" => new[] { "ekle", "duzenle" },
            _ => new[] { "goruntule" }
        };

        foreach (var islem in islemler)
        {
            if (await _yetki.IzinVarMiAsync(rol, $"{_kaynak}.{islem}", context.HttpContext.RequestAborted))
                return;
        }

        context.Result = new ObjectResult(ApiYanit<object?>.Basarisiz("Bu işlem için yetkiniz yok."))
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
}
