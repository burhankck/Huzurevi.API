using FluentValidation;

namespace Huzurevi.Application.Features.Ziyaretler;

public class ZiyaretGirisIstekDogrulayici : AbstractValidator<ZiyaretGirisIstek>
{
    public ZiyaretGirisIstekDogrulayici()
    {
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.ZiyaretciAd).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ZiyaretciSoyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TcKimlikNo)
            .Length(11)
            .Matches(@"^\d{11}$")
            .When(x => !string.IsNullOrWhiteSpace(x.TcKimlikNo))
            .WithMessage("T.C. Kimlik No 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Yakinlik).MaximumLength(50);
        RuleFor(x => x.Notlar).MaximumLength(300);
    }
}
