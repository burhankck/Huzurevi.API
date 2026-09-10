using FluentValidation;

namespace Huzurevi.Application.Features.Ayarlar;

public record UygulamaAyariDto(
    bool BakimModu,
    int SifreMinUzunluk,
    int MaksBasarisizGiris,
    int OturumDakika,
    int SifreGecerlilikGun,
    int LogSaklamaGun,
    bool YedeklemeAktifMi,
    int YedekSaat,
    int YedekSaklamaAdet);

public record UygulamaAyariIstek(
    bool BakimModu,
    int SifreMinUzunluk,
    int MaksBasarisizGiris,
    int OturumDakika,
    int SifreGecerlilikGun,
    int LogSaklamaGun,
    bool YedeklemeAktifMi,
    int YedekSaat,
    int YedekSaklamaAdet);

public class UygulamaAyariIstekDogrulayici : AbstractValidator<UygulamaAyariIstek>
{
    public UygulamaAyariIstekDogrulayici()
    {
        RuleFor(x => x.SifreMinUzunluk).InclusiveBetween(6, 32);
        RuleFor(x => x.MaksBasarisizGiris).InclusiveBetween(3, 20);
        RuleFor(x => x.OturumDakika).InclusiveBetween(15, 1440);
        RuleFor(x => x.SifreGecerlilikGun).InclusiveBetween(0, 365);
        RuleFor(x => x.LogSaklamaGun).InclusiveBetween(30, 2555);
        RuleFor(x => x.YedekSaat).InclusiveBetween(0, 23);
        RuleFor(x => x.YedekSaklamaAdet).InclusiveBetween(3, 90);
    }
}
