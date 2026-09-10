using FluentValidation;

namespace Huzurevi.Application.Features.Kullanicilar;

internal static class KullaniciKurallari
{
    public static readonly string[] Roller = ["Yonetici", "Personel"];
}

public class KullaniciOlusturIstekDogrulayici : AbstractValidator<KullaniciOlusturIstek>
{
    public KullaniciOlusturIstekDogrulayici()
    {
        RuleFor(x => x.KullaniciAdi).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Eposta).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Rol).NotEmpty().MaximumLength(40);
        RuleFor(x => x.TcKimlikNo).Length(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.TcKimlikNo));
        RuleFor(x => x.Sifre).NotEmpty().MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
    }
}

public class KullaniciGuncelleIstekDogrulayici : AbstractValidator<KullaniciGuncelleIstek>
{
    public KullaniciGuncelleIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Eposta).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Rol).NotEmpty().MaximumLength(40);
        RuleFor(x => x.TcKimlikNo).Length(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.TcKimlikNo));
    }
}

public class KullaniciSifreIstekDogrulayici : AbstractValidator<KullaniciSifreIstek>
{
    public KullaniciSifreIstekDogrulayici()
    {
        RuleFor(x => x.YeniSifre).NotEmpty().MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
    }
}
