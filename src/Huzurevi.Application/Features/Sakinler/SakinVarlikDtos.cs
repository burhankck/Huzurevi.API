using FluentValidation;

namespace Huzurevi.Application.Features.Sakinler;

public record SakinMalDto(int Id, int SakinId, string MalTuru, int Adet, decimal? Deger, string? Adres, string? Aciklama, bool AktifMi);
public record SakinMalIstek(string MalTuru, int Adet, decimal? Deger, string? Adres, string? Aciklama, bool AktifMi);

public record SakinGelirDto(int Id, int SakinId, string GelirTuru, string Periyot, decimal? Deger, string? Aciklama, bool AktifMi);
public record SakinGelirIstek(string GelirTuru, string Periyot, decimal? Deger, string? Aciklama, bool AktifMi);

public record SakinSosyalGuvenceDto(int Id, int SakinId, string GuvenceTuru, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string? Aciklama, bool AktifMi);
public record SakinSosyalGuvenceIstek(string GuvenceTuru, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string? Aciklama, bool AktifMi);

internal static class SakinVarlikKurallari
{
    public static readonly string[] MalTurleri = ["Taşınmaz", "Taşınır", "Araç", "Banka hesabı", "Diğer"];
    public static readonly string[] GelirTurleri = ["Emekli maaşı", "Maaş", "Kira", "Nafaka", "Diğer"];
    public static readonly string[] Periyotlar = ["Aylık", "Yıllık", "Tek seferlik"];
    public static readonly string[] GuvenceTurleri = ["SGK", "Bağkur", "Emekli Sandığı", "Yeşil Kart", "Özel sigorta", "Diğer"];
}

public class SakinMalIstekDogrulayici : AbstractValidator<SakinMalIstek>
{
    public SakinMalIstekDogrulayici()
    {
        RuleFor(x => x.MalTuru)
            .NotEmpty()
            .Must(v => SakinVarlikKurallari.MalTurleri.Contains(v))
            .WithMessage("Geçerli bir mal türü seçin.");
        RuleFor(x => x.Adet).GreaterThan(0).LessThanOrEqualTo(100000);
        RuleFor(x => x.Deger).GreaterThanOrEqualTo(0).When(x => x.Deger is not null);
        RuleFor(x => x.Adres).MaximumLength(300);
        RuleFor(x => x.Aciklama).MaximumLength(500);
    }
}

public class SakinGelirIstekDogrulayici : AbstractValidator<SakinGelirIstek>
{
    public SakinGelirIstekDogrulayici()
    {
        RuleFor(x => x.GelirTuru)
            .NotEmpty()
            .Must(v => SakinVarlikKurallari.GelirTurleri.Contains(v))
            .WithMessage("Geçerli bir gelir türü seçin.");
        RuleFor(x => x.Periyot)
            .NotEmpty()
            .Must(v => SakinVarlikKurallari.Periyotlar.Contains(v))
            .WithMessage("Geçerli bir periyot seçin.");
        RuleFor(x => x.Deger).GreaterThanOrEqualTo(0).When(x => x.Deger is not null);
        RuleFor(x => x.Aciklama).MaximumLength(500);
    }
}

public class SakinSosyalGuvenceIstekDogrulayici : AbstractValidator<SakinSosyalGuvenceIstek>
{
    public SakinSosyalGuvenceIstekDogrulayici()
    {
        RuleFor(x => x.GuvenceTuru)
            .NotEmpty()
            .Must(v => SakinVarlikKurallari.GuvenceTurleri.Contains(v))
            .WithMessage("Geçerli bir sosyal güvence türü seçin.");
        RuleFor(x => x.Aciklama).MaximumLength(500);
        RuleFor(x => x)
            .Must(x => x.BitisTarihi is null || x.BaslangicTarihi is null || x.BitisTarihi >= x.BaslangicTarihi)
            .WithMessage("Bitiş tarihi başlangıçtan önce olamaz.");
    }
}
