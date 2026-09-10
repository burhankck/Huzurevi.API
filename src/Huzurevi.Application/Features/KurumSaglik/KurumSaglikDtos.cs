using FluentValidation;

namespace Huzurevi.Application.Features.KurumSaglik;

public static class KurumSaglikTurleri
{
    public const string FizyoSeans = "FizyoSeans";
    public const string FizyoDegerlendirme = "FizyoDegerlendirme";
    public const string KanSekeri = "KanSekeri";
    public const string Tansiyon = "Tansiyon";
    public const string Revir = "Revir";
    public const string Nobet = "Nobet";
    public const string PeriyodikMuayene = "PeriyodikMuayene";

    public static readonly string[] Tum =
    [
        FizyoSeans, FizyoDegerlendirme, KanSekeri, Tansiyon, Revir, Nobet, PeriyodikMuayene
    ];
}

public record KurumSaglikKayitDto(
    int Id,
    string Tur,
    int? SakinId,
    string? SakinAd,
    DateTime Tarih,
    string? Personel,
    string? Notlar,
    bool ImzalandiMi,
    string? Imzalayan,
    DateTime? ImzaTarihi,
    decimal? KanSekeri,
    string? OlcumZamani,
    int? Sistolik,
    int? Diastolik,
    int? Nabiz,
    string? Tedaviler,
    string? DurumDegerlendirme,
    string? HareketKabiliyeti,
    string? GucDenge,
    string? KayitTuru,
    string? YapilanIslemler,
    string? Malzeme,
    string? Nobetci,
    string? GenelDurum,
    string? OnemliOlaylar,
    string? DevirTeslim,
    string? Doktor,
    string? Bulgular,
    string? FizikMuayene,
    string? LabSonuclari,
    string? Oneriler);

public record KurumSaglikKayitIstek(
    string Tur,
    int? SakinId,
    DateTime Tarih,
    string? Personel,
    string? Notlar,
    decimal? KanSekeri,
    string? OlcumZamani,
    int? Sistolik,
    int? Diastolik,
    int? Nabiz,
    string? Tedaviler,
    string? DurumDegerlendirme,
    string? HareketKabiliyeti,
    string? GucDenge,
    string? KayitTuru,
    string? YapilanIslemler,
    string? Malzeme,
    string? Nobetci,
    string? GenelDurum,
    string? OnemliOlaylar,
    string? DevirTeslim,
    string? Doktor,
    string? Bulgular,
    string? FizikMuayene,
    string? LabSonuclari,
    string? Oneriler);

public class KurumSaglikKayitIstekDogrulayici : AbstractValidator<KurumSaglikKayitIstek>
{
    public KurumSaglikKayitIstekDogrulayici()
    {
        RuleFor(x => x.Tur).NotEmpty().Must(v => KurumSaglikTurleri.Tum.Contains(v));
        RuleFor(x => x.Tarih).NotEmpty();
        RuleFor(x => x.SakinId).NotEmpty()
            .When(x => x.Tur != KurumSaglikTurleri.Nobet)
            .WithMessage("Sakin seçiniz.");
        RuleFor(x => x.KanSekeri).InclusiveBetween(20, 800).When(x => x.KanSekeri is not null);
        RuleFor(x => x.Sistolik).InclusiveBetween(50, 250).When(x => x.Sistolik is not null);
        RuleFor(x => x.Diastolik).InclusiveBetween(30, 180).When(x => x.Diastolik is not null);
        RuleFor(x => x.Nabiz).InclusiveBetween(30, 220).When(x => x.Nabiz is not null);
    }
}

public record NarkotikIlacDto(int Id, string Ad, decimal Stok, string? Birim, bool AktifMi);
public record NarkotikIlacIstek(string Ad, string? Birim, bool AktifMi);
public record NarkotikHareketDto(int Id, int NarkotikIlacId, string IlacAd, string HareketTuru, decimal Miktar, DateTime Tarih, int? SakinId, string? SakinAd, string? Notlar);
public record NarkotikHareketIstek(int NarkotikIlacId, string HareketTuru, decimal Miktar, DateTime Tarih, int? SakinId, string? Notlar);

public class NarkotikIlacIstekDogrulayici : AbstractValidator<NarkotikIlacIstek>
{
    public NarkotikIlacIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Birim).MaximumLength(20);
    }
}

public class NarkotikHareketIstekDogrulayici : AbstractValidator<NarkotikHareketIstek>
{
    public NarkotikHareketIstekDogrulayici()
    {
        RuleFor(x => x.NarkotikIlacId).GreaterThan(0);
        RuleFor(x => x.HareketTuru).Must(v => v is "Giris" or "Kullanim");
        RuleFor(x => x.Miktar).GreaterThan(0);
        RuleFor(x => x.Tarih).NotEmpty();
        RuleFor(x => x.SakinId).NotEmpty().When(x => x.HareketTuru == "Kullanim");
    }
}

public record IlacEmriDto(
    int Id,
    int SakinId,
    string SakinAd,
    string IlacAdi,
    string? Doz,
    string? Birim,
    string? KullanimSikligi,
    string? Zamanlama,
    DateTime BaslangicTarihi,
    DateTime? BitisTarihi,
    bool AktifMi,
    string KayitTuru);

public record IlacEmriIstek(
    int SakinId,
    string IlacAdi,
    string? Doz,
    string? Birim,
    string? KullanimSikligi,
    string? Zamanlama,
    DateTime BaslangicTarihi,
    DateTime? BitisTarihi,
    bool AktifMi,
    string KayitTuru);

public class IlacEmriIstekDogrulayici : AbstractValidator<IlacEmriIstek>
{
    public IlacEmriIstekDogrulayici()
    {
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.IlacAdi).NotEmpty().MaximumLength(150);
        RuleFor(x => x.KayitTuru).Must(v => v is "IlkKayit" or "YeniEmir");
        RuleFor(x => x.BaslangicTarihi).NotEmpty();
    }
}

public record IlacUygulamaDto(
    int Id,
    int IlacEmriId,
    int SakinId,
    string SakinAd,
    string IlacAdi,
    DateTime Tarih,
    string Durum,
    string? Personel,
    string? Notlar);

public record IlacUygulamaIstek(int IlacEmriId, DateTime Tarih, string Durum, string? Personel, string? Notlar);

public class IlacUygulamaIstekDogrulayici : AbstractValidator<IlacUygulamaIstek>
{
    public IlacUygulamaIstekDogrulayici()
    {
        RuleFor(x => x.IlacEmriId).GreaterThan(0);
        RuleFor(x => x.Tarih).NotEmpty();
        RuleFor(x => x.Durum).Must(v => v is "Bekliyor" or "Uygulandı" or "Atlandı" or "Reddedildi");
    }
}
