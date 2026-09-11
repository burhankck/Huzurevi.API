using FluentValidation;

namespace Huzurevi.Application.Features.Yataklar;

internal static class YatakKurallari
{
    public static readonly string[] Tipler = ["Standart", "Hasta yatağı", "Havalı", "Elektrikli"];
    public static readonly string[] Ozellikler = ["Korkuluk", "Tekerlekli", "Pozisyon", "Oksijen", "Basınç yatak"];
    public static readonly string[] Durumlar = ["Aktif", "Bakımda", "Arızalı"];
}

public class YatakOlusturIstekDogrulayici : AbstractValidator<YatakOlusturIstek>
{
    public YatakOlusturIstekDogrulayici()
    {
        RuleFor(x => x.OdaId).GreaterThan(0);
        RuleFor(x => x.YatakNumarasi).NotEmpty().MaximumLength(50);
        RuleFor(x => x.YatakTipi)
            .Must(v => v is null || YatakKurallari.Tipler.Contains(v))
            .WithMessage("Yatak tipi geçersiz.");
        RuleFor(x => x.Durum)
            .Must(v => v is null || YatakKurallari.Durumlar.Contains(v))
            .WithMessage("Yatak durumu Aktif, Bakımda veya Arızalı olmalıdır.");
        RuleFor(x => x.Ozellikler)
            .Must(v => v is null || v.All(o => YatakKurallari.Ozellikler.Contains(o)))
            .WithMessage("Yatak özelliği geçersiz.");
    }
}

public class YatakGuncelleIstekDogrulayici : AbstractValidator<YatakGuncelleIstek>
{
    public YatakGuncelleIstekDogrulayici()
    {
        RuleFor(x => x.YatakNumarasi).NotEmpty().MaximumLength(50);
        RuleFor(x => x.YatakTipi)
            .Must(v => v is null || YatakKurallari.Tipler.Contains(v))
            .WithMessage("Yatak tipi geçersiz.");
        RuleFor(x => x.Durum)
            .Must(v => v is null || YatakKurallari.Durumlar.Contains(v))
            .WithMessage("Yatak durumu Aktif, Bakımda veya Arızalı olmalıdır.");
        RuleFor(x => x.Ozellikler)
            .Must(v => v is null || v.All(o => YatakKurallari.Ozellikler.Contains(o)))
            .WithMessage("Yatak özelliği geçersiz.");
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
