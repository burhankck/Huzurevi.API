using FluentValidation;

namespace Huzurevi.Application.Features.Yataklar;

public class YatakOlusturIstekDogrulayici : AbstractValidator<YatakOlusturIstek>
{
    public YatakOlusturIstekDogrulayici()
    {
        RuleFor(x => x.OdaId).GreaterThan(0);
        RuleFor(x => x.YatakNumarasi).NotEmpty().MaximumLength(50);
    }
}

public class YatakAtamaIstekDogrulayici : AbstractValidator<YatakAtamaIstek>
{
    public YatakAtamaIstekDogrulayici()
    {
        RuleFor(x => x.YatakId).GreaterThan(0);
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.GirisTarihi).NotEmpty();
    }
}

public class YatakBosaltIstekDogrulayici : AbstractValidator<YatakBosaltIstek>
{
    public YatakBosaltIstekDogrulayici()
    {
        RuleFor(x => x.CikisTarihi).NotEmpty();
    }
}
