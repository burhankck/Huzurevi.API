using FluentValidation;

namespace Huzurevi.Application.Features.IlacTakipleri;

public class IlacTakipOlusturIstekDogrulayici : AbstractValidator<IlacTakipOlusturIstek>
{
    public IlacTakipOlusturIstekDogrulayici()
    {
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.IlacAdi).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Dozaj).MaximumLength(100);
        RuleFor(x => x.Zaman).MaximumLength(100);
        RuleFor(x => x.Notlar).MaximumLength(300);
    }
}
