using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;

namespace Huzurevi.Application.Features.Sakinler;

public record SakinGunlukIzinDto(int Id, int SakinId, DateTime Tarih, string Yer, string? CikisSaati, string? DonusSaati, bool AktifMi);
public record SakinGunlukIzinIstek(DateTime Tarih, string Yer, string? CikisSaati, string? DonusSaati, bool AktifMi);

public record SakinEmanetDto(
    int Id,
    int SakinId,
    string IslemTuru,
    string EmanetTuru,
    string Durum,
    int Adet,
    decimal? Deger,
    DateTime Tarih,
    string? Saat,
    string TeslimEden,
    string TeslimAlan,
    string? Aciklama);

public record SakinEmanetIstek(
    string IslemTuru,
    string EmanetTuru,
    string Durum,
    int Adet,
    decimal? Deger,
    DateTime Tarih,
    string? Saat,
    string TeslimEden,
    string TeslimAlan,
    string? Aciklama);

internal static class SakinIzinEmanetKurallari
{
    public static readonly string[] IslemTurleri = ["Teslim alma", "Teslim etme"];
    public static readonly string[] EmanetTurleri = ["Nakit", "Değerli eşya", "Belge", "İlaç", "Diğer"];
    public static readonly string[] Durumlar = ["Emanette", "Teslim edildi", "İade edildi"];
    public static readonly Regex SaatKalibi = new(@"^([01]\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);

    public static string? SaatNormalize(string? deger)
    {
        if (string.IsNullOrWhiteSpace(deger)) return null;
        var metin = deger.Trim();
        if (TimeSpan.TryParseExact(metin, ["hh\\:mm", "h\\:mm", "hh\\:mm\\:ss"], CultureInfo.InvariantCulture, out var sure)
            || TimeSpan.TryParse(metin, CultureInfo.InvariantCulture, out sure))
        {
            return sure.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
        }

        return metin;
    }
}

public class SakinGunlukIzinIstekDogrulayici : AbstractValidator<SakinGunlukIzinIstek>
{
    public SakinGunlukIzinIstekDogrulayici()
    {
        RuleFor(x => x.Tarih).NotEmpty();
        RuleFor(x => x.Yer).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CikisSaati)
            .Must(s => string.IsNullOrWhiteSpace(s) || SakinIzinEmanetKurallari.SaatKalibi.IsMatch(SakinIzinEmanetKurallari.SaatNormalize(s) ?? ""))
            .WithMessage("Çıkış saati SS:DD biçiminde olmalıdır.");
        RuleFor(x => x.DonusSaati)
            .Must(s => string.IsNullOrWhiteSpace(s) || SakinIzinEmanetKurallari.SaatKalibi.IsMatch(SakinIzinEmanetKurallari.SaatNormalize(s) ?? ""))
            .WithMessage("Dönüş saati SS:DD biçiminde olmalıdır.");
        RuleFor(x => x)
            .Must(x =>
            {
                var cikis = SakinIzinEmanetKurallari.SaatNormalize(x.CikisSaati);
                var donus = SakinIzinEmanetKurallari.SaatNormalize(x.DonusSaati);
                if (cikis is null || donus is null) return true;
                return string.CompareOrdinal(donus, cikis) >= 0;
            })
            .WithMessage("Dönüş saati çıkış saatinden önce olamaz.");
    }
}

public class SakinEmanetIstekDogrulayici : AbstractValidator<SakinEmanetIstek>
{
    public SakinEmanetIstekDogrulayici()
    {
        RuleFor(x => x.IslemTuru)
            .NotEmpty()
            .Must(v => SakinIzinEmanetKurallari.IslemTurleri.Contains(v))
            .WithMessage("Geçerli bir işlem türü seçin.");
        RuleFor(x => x.EmanetTuru)
            .NotEmpty()
            .Must(v => SakinIzinEmanetKurallari.EmanetTurleri.Contains(v))
            .WithMessage("Geçerli bir emanet türü seçin.");
        RuleFor(x => x.Durum)
            .NotEmpty()
            .Must(v => SakinIzinEmanetKurallari.Durumlar.Contains(v))
            .WithMessage("Geçerli bir durum seçin.");
        RuleFor(x => x.Adet).GreaterThan(0).LessThanOrEqualTo(100000);
        RuleFor(x => x.Deger).GreaterThanOrEqualTo(0).When(x => x.Deger is not null);
        RuleFor(x => x.Tarih).NotEmpty();
        RuleFor(x => x.Saat)
            .Must(s => string.IsNullOrWhiteSpace(s) || SakinIzinEmanetKurallari.SaatKalibi.IsMatch(SakinIzinEmanetKurallari.SaatNormalize(s) ?? ""))
            .WithMessage("Saat SS:DD biçiminde olmalıdır.");
        RuleFor(x => x.TeslimEden).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TeslimAlan).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Aciklama).MaximumLength(500);
    }
}
