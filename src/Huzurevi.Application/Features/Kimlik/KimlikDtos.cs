using FluentValidation;
using Huzurevi.Application.Features.Kurum;

namespace Huzurevi.Application.Features.Kimlik;

public record GirisIstek(string KullaniciAdi, string Sifre);

public record GirisSonuc(
    string Token,
    DateTime GecerlilikBitis,
    KullaniciOzetDto Kullanici);

public record KullaniciOzetDto(
    int Id,
    string KullaniciAdi,
    string Ad,
    string Soyad,
    string Eposta,
    string? Telefon,
    string Rol,
    bool AktifMi,
    string? TcKimlikNo,
    int? PersonelId,
    int? AktifKurulusId,
    string? AktifKurulusAd,
    List<KurulusSecimDto> Kuruluslar,
    string? RolAd,
    List<string> Izinler);

public record KurulusSecIstek(int KurulusId);

public record ProfilGuncelleIstek(string Eposta, string? Telefon, string? MevcutSifre, string? YeniSifre);

public record SifremiUnuttumIstek(string KullaniciAdi);
public record SifremiUnuttumSonuc(string Mesaj, string? GelistirmeKodu);

public record SifreSifirlaIstek(string KullaniciAdi, string Kod, string YeniSifre);

public class GirisIstekDogrulayici : AbstractValidator<GirisIstek>
{
    public GirisIstekDogrulayici()
    {
        RuleFor(x => x.KullaniciAdi).NotEmpty().MaximumLength(50).WithMessage("Kullanıcı adı zorunludur.");
        RuleFor(x => x.Sifre).NotEmpty().WithMessage("Şifre zorunludur.");
    }
}

public class ProfilGuncelleIstekDogrulayici : AbstractValidator<ProfilGuncelleIstek>
{
    public ProfilGuncelleIstekDogrulayici()
    {
        RuleFor(x => x.Eposta).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon))
            .WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.YeniSifre).MinimumLength(6).When(x => !string.IsNullOrWhiteSpace(x.YeniSifre));
        RuleFor(x => x.MevcutSifre).NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.YeniSifre))
            .WithMessage("Yeni şifre için mevcut şifre gereklidir.");
    }
}

public class SifremiUnuttumIstekDogrulayici : AbstractValidator<SifremiUnuttumIstek>
{
    public SifremiUnuttumIstekDogrulayici()
    {
        RuleFor(x => x.KullaniciAdi).NotEmpty().MaximumLength(50);
    }
}

public class KurulusSecIstekDogrulayici : AbstractValidator<KurulusSecIstek>
{
    public KurulusSecIstekDogrulayici()
    {
        RuleFor(x => x.KurulusId).GreaterThan(0);
    }
}

public class SifreSifirlaIstekDogrulayici : AbstractValidator<SifreSifirlaIstek>
{
    public SifreSifirlaIstekDogrulayici()
    {
        RuleFor(x => x.KullaniciAdi).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Kod).NotEmpty().MaximumLength(80);
        RuleFor(x => x.YeniSifre).NotEmpty().MinimumLength(6);
    }
}
