using FluentValidation;

namespace Huzurevi.Application.Features.Sakinler;

public record SakinOlcumDto(int Id, int SakinId, DateTime Tarih, decimal? BoyCm, decimal? KiloKg, string? KanGrubu, string? Aciklama, bool AktifMi);
public record SakinOlcumIstek(DateTime Tarih, decimal? BoyCm, decimal? KiloKg, string? KanGrubu, string? Aciklama, bool AktifMi);

public record SakinSaglikDegerlendirmeDto(
    int Id,
    int SakinId,
    string Tur,
    DateTime Tarih,
    string Durum,
    string? Seviye,
    string? Taraf,
    bool YardimciAracMi,
    string? Aciklama,
    bool AktifMi);

public record SakinSaglikDegerlendirmeIstek(
    string Tur,
    DateTime Tarih,
    string Durum,
    string? Seviye,
    string? Taraf,
    bool YardimciAracMi,
    string? Aciklama,
    bool AktifMi);

internal static class SakinSaglikKurallari
{
    public static readonly string[] KanGruplari =
    [
        "0 Rh+", "0 Rh-", "A Rh+", "A Rh-", "B Rh+", "B Rh-", "AB Rh+", "AB Rh-", "Bilinmiyor"
    ];
    public static readonly string[] KayitTurleri =
        ["Hastalik", "Ameliyat", "Aliskanlik", "Cihaz", "Protez", "GelisIlaci", "Alerji"];
    public static readonly string[] Turler = ["GelisSekli", "Konusma", "Isitme", "Gorme"];
    public static readonly string[] GelisSekilleri = ["Yürüyerek", "Yardımla", "Tekerlekli sandalye", "Sedye", "Diğer"];
    public static readonly string[] KonusmaDurumlari = ["Normal", "Kısmi", "Afazi", "Konuşamıyor", "Belirtilmedi"];
    public static readonly string[] IsitmeDurumlari = ["Normal", "Azalmış", "İşitmiyor"];
    public static readonly string[] GormeDurumlari = ["Normal", "Azalmış", "Görmüyor"];
    public static readonly string[] Seviyeler = ["Hafif", "Orta", "İleri"];
    public static readonly string[] Taraflar = ["Sol", "Sağ", "Her iki taraf"];

    public static string[] Durumlar(string tur) => tur switch
    {
        "GelisSekli" => GelisSekilleri,
        "Konusma" => KonusmaDurumlari,
        "Isitme" => IsitmeDurumlari,
        "Gorme" => GormeDurumlari,
        _ => []
    };
}

public class SakinOlcumIstekDogrulayici : AbstractValidator<SakinOlcumIstek>
{
    public SakinOlcumIstekDogrulayici()
    {
        RuleFor(x => x.Tarih).NotEmpty();
        RuleFor(x => x.BoyCm).InclusiveBetween(50, 250).When(x => x.BoyCm is not null);
        RuleFor(x => x.KiloKg).InclusiveBetween(20, 400).When(x => x.KiloKg is not null);
        RuleFor(x => x)
            .Must(x => x.BoyCm is not null || x.KiloKg is not null || !string.IsNullOrWhiteSpace(x.KanGrubu))
            .WithMessage("Boy, kilo veya kan grubundan en az biri girilmelidir.");
        RuleFor(x => x.KanGrubu).Must(v => v is null || SakinSaglikKurallari.KanGruplari.Contains(v));
        RuleFor(x => x.Aciklama).MaximumLength(500);
    }
}

public class SakinSaglikDegerlendirmeIstekDogrulayici : AbstractValidator<SakinSaglikDegerlendirmeIstek>
{
    public SakinSaglikDegerlendirmeIstekDogrulayici()
    {
        RuleFor(x => x.Tur).NotEmpty().Must(v => SakinSaglikKurallari.Turler.Contains(v));
        RuleFor(x => x.Tarih).NotEmpty();
        RuleFor(x => x.Durum)
            .NotEmpty()
            .Must((istek, durum) => SakinSaglikKurallari.Durumlar(istek.Tur).Contains(durum))
            .WithMessage("Geçerli bir durum seçin.");
        RuleFor(x => x.Seviye).Must(v => v is null || SakinSaglikKurallari.Seviyeler.Contains(v));
        RuleFor(x => x.Taraf).Must(v => v is null || SakinSaglikKurallari.Taraflar.Contains(v));
        RuleFor(x => x.Aciklama).MaximumLength(500);
    }
}
