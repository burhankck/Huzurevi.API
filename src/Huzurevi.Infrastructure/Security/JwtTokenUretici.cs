using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Huzurevi.Infrastructure.Security;

public class JwtTokenUretici : ITokenUretici
{
    private readonly IConfiguration _configuration;

    public JwtTokenUretici(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime GecerlilikBitis) TokenUret(Kullanici kullanici, int oturumDakika, int? kurulusId, string? rol)
    {
        var anahtar = _configuration["Jwt:Anahtar"]
            ?? throw new InvalidOperationException("Jwt:Anahtar yapılandırması eksik.");
        var yayinci = _configuration["Jwt:Yayinci"] ?? "Huzurevi.API";
        var dinleyici = _configuration["Jwt:Dinleyici"] ?? "Huzurevi.Web";
        var dakika = oturumDakika > 0 ? oturumDakika : _configuration.GetValue("Jwt:GecerlilikDakika", 480);
        var bitis = DateTime.UtcNow.AddMinutes(dakika);
        var etkinRol = string.IsNullOrWhiteSpace(rol) ? kullanici.Rol : rol;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, kullanici.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, kullanici.KullaniciAdi),
            new(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
            new(ClaimTypes.Name, kullanici.KullaniciAdi),
            new(ClaimTypes.GivenName, kullanici.Ad),
            new(ClaimTypes.Surname, kullanici.Soyad),
            new(ClaimTypes.Role, etkinRol),
            new("role", etkinRol),
            new(JwtRegisteredClaimNames.Email, kullanici.Eposta)
        };
        if (kurulusId is not null)
        {
            claims.Add(new Claim("kurulusId", kurulusId.Value.ToString()));
        }

        var imza = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(anahtar)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: yayinci,
            audience: dinleyici,
            claims: claims,
            expires: bitis,
            signingCredentials: imza);

        return (new JwtSecurityTokenHandler().WriteToken(token), bitis);
    }
}
