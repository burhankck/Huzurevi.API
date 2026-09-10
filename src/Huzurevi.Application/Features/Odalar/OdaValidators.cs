using FluentValidation;

namespace Huzurevi.Application.Features.Odalar;

internal static class OdaKurallari
{
    public static readonly string[] Durumlar = ["Aktif", "Bakımda", "Kapalı"];
    public static readonly string[] Tipler = ["Standart", "Özel bakım", "İzole", "Misafir"];
}

public class OdaOlusturIstekDogrulayici : AbstractValidator<OdaOlusturIstek>
{
    public OdaOlusturIstekDogrulayici()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.OdaNumarasi) || !string.IsNullOrWhiteSpace(x.OdaNo))
            .WithMessage("Oda numarası zorunludur.");

        RuleFor(x => x.Kat).GreaterThanOrEqualTo(0).WithMessage("Kat 0 veya daha büyük olmalıdır.");
        RuleFor(x => x.Kapasite).InclusiveBetween(1, 12).WithMessage("Kapasite 1 ile 12 arasında olmalıdır.");
        RuleFor(x => x.Blok).MaximumLength(50);
        RuleFor(x => x.OdaTipi)
            .Must(v => v is null || OdaKurallari.Tipler.Contains(v))
            .WithMessage("Oda tipi geçersiz.");
    }
}

public class OdaGuncelleIstekDogrulayici : AbstractValidator<OdaGuncelleIstek>
{
    public OdaGuncelleIstekDogrulayici()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.OdaNumarasi) || !string.IsNullOrWhiteSpace(x.OdaNo))
            .WithMessage("Oda numarası zorunludur.");

        RuleFor(x => x.Kat).GreaterThanOrEqualTo(0).WithMessage("Kat 0 veya daha büyük olmalıdır.");
        RuleFor(x => x.Kapasite).InclusiveBetween(1, 12).WithMessage("Kapasite 1 ile 12 arasında olmalıdır.");
        RuleFor(x => x.Blok).MaximumLength(50);
        RuleFor(x => x.Durum)
            .Must(v => v is null || OdaKurallari.Durumlar.Contains(v))
            .WithMessage("Oda durumu Aktif, Bakımda veya Kapalı olmalıdır.");
        RuleFor(x => x.OdaTipi)
            .Must(v => v is null || OdaKurallari.Tipler.Contains(v))
            .WithMessage("Oda tipi geçersiz.");
    }
}
