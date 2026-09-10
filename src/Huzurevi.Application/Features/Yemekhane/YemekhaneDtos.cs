using FluentValidation;

namespace Huzurevi.Application.Features.Yemekhane;

public static class YemekhaneSabitleri
{
    public static readonly string[] Kategoriler = ["Çorba", "Ana yemek", "Salata", "Tatlı", "İçecek", "Diğer"];
    public static readonly string[] Ogunler = ["Kahvaltı", "Öğle", "Akşam", "Ara"];
    public static readonly string[] Teksturler = ["Normal", "Püre", "Kıyma", "Sıvı"];
    public static readonly string[] SiviTurleri = ["Su", "Çay", "Süt", "Ayran", "Diğer"];
}

public record YemekDto(int Id, string Ad, string Kategori, decimal? Kalori, decimal? Protein, decimal? Karbonhidrat, decimal? Yag, string? Alerjenler, string? Tekstur, bool AktifMi);
public record YemekIstek(string Ad, string Kategori, decimal? Kalori, decimal? Protein, decimal? Karbonhidrat, decimal? Yag, string? Alerjenler, string? Tekstur, bool AktifMi);

public class YemekIstekDogrulayici : AbstractValidator<YemekIstek>
{
    public YemekIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Kategori).Must(v => YemekhaneSabitleri.Kategoriler.Contains(v));
    }
}

public record BeslenmeProfiliDto(int Id, int SakinId, string SakinAd, string? TeksturTercihi, decimal? HedefKalori, decimal? HedefProtein, decimal? HedefSiviMl, string? Notlar);
public record BeslenmeProfiliIstek(int SakinId, string? TeksturTercihi, decimal? HedefKalori, decimal? HedefProtein, decimal? HedefSiviMl, string? Notlar);

public class BeslenmeProfiliIstekDogrulayici : AbstractValidator<BeslenmeProfiliIstek>
{
    public BeslenmeProfiliIstekDogrulayici()
    {
        RuleFor(x => x.SakinId).GreaterThan(0);
    }
}

public record GunlukMenuDto(int Id, DateTime Tarih, string OgunTipi, int YemekId, string YemekAd);
public record GunlukMenuIstek(DateTime Tarih, string OgunTipi, List<int> YemekIdleri);

public class GunlukMenuIstekDogrulayici : AbstractValidator<GunlukMenuIstek>
{
    public GunlukMenuIstekDogrulayici()
    {
        RuleFor(x => x.Tarih).NotEmpty();
        RuleFor(x => x.OgunTipi).Must(v => YemekhaneSabitleri.Ogunler.Contains(v));
        RuleFor(x => x.YemekIdleri).NotEmpty();
    }
}

public record OzelMenuDto(int Id, int SakinId, string SakinAd, DateTime Tarih, string OgunTipi, int YemekId, string YemekAd, string? TeksturMod, decimal? SiviMl, string? Notlar);
public record OzelMenuIstek(int SakinId, DateTime Tarih, string OgunTipi, int YemekId, string? TeksturMod, decimal? SiviMl, string? Notlar);

public class OzelMenuIstekDogrulayici : AbstractValidator<OzelMenuIstek>
{
    public OzelMenuIstekDogrulayici()
    {
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.YemekId).GreaterThan(0);
        RuleFor(x => x.OgunTipi).Must(v => YemekhaneSabitleri.Ogunler.Contains(v));
    }
}

public record YemekTuketimDto(int Id, int SakinId, string SakinAd, DateTime Tarih, string OgunTipi, int YemekId, string YemekAd, bool TuketildiMi, string? TuketilmemeNedeni);
public record YemekTuketimIstek(int SakinId, DateTime Tarih, string OgunTipi, int YemekId, bool TuketildiMi, string? TuketilmemeNedeni);

public class YemekTuketimIstekDogrulayici : AbstractValidator<YemekTuketimIstek>
{
    public YemekTuketimIstekDogrulayici()
    {
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.YemekId).GreaterThan(0);
        RuleFor(x => x.OgunTipi).Must(v => YemekhaneSabitleri.Ogunler.Contains(v));
    }
}

public record SiviAlimiDto(int Id, int SakinId, string SakinAd, DateTime Tarih, string SiviTuru, decimal MiktarMl);
public record SiviAlimiIstek(int SakinId, DateTime Tarih, string SiviTuru, decimal MiktarMl);

public class SiviAlimiIstekDogrulayici : AbstractValidator<SiviAlimiIstek>
{
    public SiviAlimiIstekDogrulayici()
    {
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.SiviTuru).Must(v => YemekhaneSabitleri.SiviTurleri.Contains(v));
        RuleFor(x => x.MiktarMl).InclusiveBetween(1, 5000);
    }
}

public record BeslenmeOzetKalemDto(string OgunTipi, string YemekAd, bool TuketildiMi, string? TuketilmemeNedeni, decimal? Kalori, decimal? Protein);
public record BeslenmeOzetDto(int SakinId, string SakinAd, decimal ToplamKalori, decimal ToplamProtein, decimal ToplamSivi, List<BeslenmeOzetKalemDto> Detay);
