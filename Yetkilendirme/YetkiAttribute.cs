using Huzurevi.Application.Features.Yetkiler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Huzurevi.API.Yetkilendirme;

/// <summary>
/// Endpoint iznini kesin doğrular. Token olsa bile rolde izin yoksa 403 döner.
/// Örnek: [Yetki("sakin.sil")]
/// </summary>
public sealed class YetkiAttribute : AuthorizeAttribute
{
    public const string PolitikaOnEki = "Yetki:";

    public YetkiAttribute(string izinKodu) : base(PolitikaOnEki + izinKodu)
    {
        IzinKodu = izinKodu;
    }

    public string IzinKodu { get; }
}

public sealed class IzinGereksinimi : IAuthorizationRequirement
{
    public IzinGereksinimi(string izinKodu) => IzinKodu = izinKodu;
    public string IzinKodu { get; }
}

public sealed class YetkiPolitikaSaglayici : DefaultAuthorizationPolicyProvider
{
    public YetkiPolitikaSaglayici(Microsoft.Extensions.Options.IOptions<AuthorizationOptions> options) : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(YetkiAttribute.PolitikaOnEki, StringComparison.OrdinalIgnoreCase))
        {
            var kod = policyName[YetkiAttribute.PolitikaOnEki.Length..];
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new IzinGereksinimi(kod))
                .Build();
        }

        return await base.GetPolicyAsync(policyName);
    }
}

public sealed class IzinYetkiIsleyici : AuthorizationHandler<IzinGereksinimi>
{
    private readonly IYetkiServisi _yetki;

    public IzinYetkiIsleyici(IYetkiServisi yetki) => _yetki = yetki;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IzinGereksinimi requirement)
    {
        var rol = context.User.FindFirstValue(ClaimTypes.Role)
            ?? context.User.FindFirstValue("role");
        if (await _yetki.IzinVarMiAsync(rol, requirement.IzinKodu))
            context.Succeed(requirement);
    }
}

public static class YetkiKaynakYardimcisi
{
    public static bool MetoddaAcikYetkiVar(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor cad)
            return false;
        return cad.MethodInfo.GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Any(a => a.Policy?.StartsWith(YetkiAttribute.PolitikaOnEki, StringComparison.OrdinalIgnoreCase) == true);
    }
}
