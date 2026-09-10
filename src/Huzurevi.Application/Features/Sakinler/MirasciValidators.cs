using FluentValidation;

namespace Huzurevi.Application.Features.Sakinler;

public class MirasciOlusturIstekDogrulayici : AbstractValidator<MirasciOlusturIstek>
{
    public MirasciOlusturIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TcKimlikNo)
            .Length(11)
            .Matches(@"^\d{11}$")
            .When(x => !string.IsNullOrWhiteSpace(x.TcKimlikNo))
            .WithMessage("T.C. Kimlik No 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Yakinlik)
            .NotEmpty()
            .Must(v => SakinKurallari.Yakinliklar.Contains(v))
            .WithMessage("Yakınlık derecesi geçersiz.");
        RuleFor(x => x.Adres).MaximumLength(300);
    }
}

public class MirasciGuncelleIstekDogrulayici : AbstractValidator<MirasciGuncelleIstek>
{
    public MirasciGuncelleIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TcKimlikNo)
            .Length(11)
            .Matches(@"^\d{11}$")
            .When(x => !string.IsNullOrWhiteSpace(x.TcKimlikNo))
            .WithMessage("T.C. Kimlik No 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Yakinlik)
            .NotEmpty()
            .Must(v => SakinKurallari.Yakinliklar.Contains(v))
            .WithMessage("Yakınlık derecesi geçersiz.");
        RuleFor(x => x.Adres).MaximumLength(300);
    }
}
