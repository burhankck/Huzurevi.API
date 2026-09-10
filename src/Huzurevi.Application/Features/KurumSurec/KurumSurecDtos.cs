using FluentValidation;

namespace Huzurevi.Application.Features.KurumSurec;

public static class OnayAlanlari
{
    public const string Izin = "Izin";
    public const string Esya = "Esya";
    public const string SosyalInceleme = "SosyalInceleme";
    public const string Psikolojik = "Psikolojik";
    public const string MirasciTeslim = "MirasciTeslim";

    public static readonly string[] Tum = [Izin, Esya, SosyalInceleme, Psikolojik, MirasciTeslim];
}

public static class OnayDurumlari
{
    public const string Bekliyor = "Bekliyor";
    public const string Onaylandi = "Onaylandi";
    public const string Reddedildi = "Reddedildi";
}

public record OnayIstek(bool OnaylandiMi, string? Not);

public class OnayIstekDogrulayici : AbstractValidator<OnayIstek>
{
    public OnayIstekDogrulayici()
    {
        RuleFor(x => x.Not).MaximumLength(500);
    }
}

public record OnayYetkisiDto(int Id, int KullaniciId, string KullaniciAd, string Alan, bool AktifMi);
public record OnayYetkisiIstek(int KullaniciId, string Alan, bool AktifMi);

public class OnayYetkisiIstekDogrulayici : AbstractValidator<OnayYetkisiIstek>
{
    public OnayYetkisiIstekDogrulayici()
    {
        RuleFor(x => x.KullaniciId).GreaterThan(0);
        RuleFor(x => x.Alan).Must(v => OnayAlanlari.Tum.Contains(v));
    }
}

public record IzinSureciDto(
    int Id, int SakinId, string SakinAd, string IzinTuru, DateTime BaslangicTarihi, DateTime BitisTarihi,
    string? TeslimAlan, string? Notlar, string OnayDurumu, string? Onaylayan, DateTime? OnayTarihi, string? OnayNotu);

public record IzinSureciIstek(
    int SakinId, string IzinTuru, DateTime BaslangicTarihi, DateTime BitisTarihi, string? TeslimAlan, string? Notlar);

public class IzinSureciIstekDogrulayici : AbstractValidator<IzinSureciIstek>
{
    public IzinSureciIstekDogrulayici()
    {
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.IzinTuru).NotEmpty().MaximumLength(40);
        RuleFor(x => x.BitisTarihi).GreaterThanOrEqualTo(x => x.BaslangicTarihi);
    }
}

public record EsyaTespitDto(
    int Id, int SakinId, string SakinAd, string Kategori, int Adet, string? Aciklama, DateTime TespitTarihi,
    string OnayDurumu, string? Onaylayan, DateTime? OnayTarihi, string? OnayNotu);

public record EsyaTespitIstek(int SakinId, string Kategori, int Adet, string? Aciklama, DateTime TespitTarihi);

public class EsyaTespitIstekDogrulayici : AbstractValidator<EsyaTespitIstek>
{
    public EsyaTespitIstekDogrulayici()
    {
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.Kategori).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Adet).InclusiveBetween(1, 9999);
        RuleFor(x => x.TespitTarihi).NotEmpty();
    }
}

public record MirasciTeslimDto(
    int Id, int SakinId, string SakinAd, int? MirasciId, string? MirasciAd, string TeslimAlan, string? TeslimAlanTelefon,
    DateTime TeslimTarihi, string? EsyaOzeti, string? Notlar, bool EvrakVarMi, string OnayDurumu, string? Onaylayan,
    DateTime? OnayTarihi, string? OnayNotu);

public record MirasciTeslimIstek(
    int SakinId, int? MirasciId, string TeslimAlan, string? TeslimAlanTelefon, DateTime TeslimTarihi, string? EsyaOzeti, string? Notlar);

public class MirasciTeslimIstekDogrulayici : AbstractValidator<MirasciTeslimIstek>
{
    public MirasciTeslimIstekDogrulayici()
    {
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.TeslimAlan).NotEmpty().MaximumLength(120);
        RuleFor(x => x.TeslimTarihi).NotEmpty();
    }
}

public record SosyalIncelemeDto(
    int Id, int? SakinId, string? SakinAd, DateTime Tarih, string Durum, string? BasvuruYapan, string? TcKimlikNo, string AdSoyad,
    DateTime? DogumTarihi, string? DogumYeri, string? Cinsiyet, string? MedeniDurum, string? EgitimDurumu, string? Telefon,
    string? Eposta, string? Adres, string? GelirKaynagi, string? AylikGelir, string? SosyalGuvence, string? Mulk,
    string? AileUyeleri, string? YakinlikDereceleri, string? Iletisim, string? KronikHastaliklar, string? KullanilanIlaclar,
    string? EngelDurumu, string? BakimIhtiyaci, string? UzmanGorusu, string? Oneri, string? Sonuc, string OnayDurumu,
    string? Onaylayan, DateTime? OnayTarihi, string? OnayNotu);

public record SosyalIncelemeIstek(
    int? SakinId, DateTime Tarih, string Durum, string? BasvuruYapan, string? TcKimlikNo, string AdSoyad, DateTime? DogumTarihi,
    string? DogumYeri, string? Cinsiyet, string? MedeniDurum, string? EgitimDurumu, string? Telefon, string? Eposta, string? Adres,
    string? GelirKaynagi, string? AylikGelir, string? SosyalGuvence, string? Mulk, string? AileUyeleri, string? YakinlikDereceleri,
    string? Iletisim, string? KronikHastaliklar, string? KullanilanIlaclar, string? EngelDurumu, string? BakimIhtiyaci,
    string? UzmanGorusu, string? Oneri, string? Sonuc);

public class SosyalIncelemeIstekDogrulayici : AbstractValidator<SosyalIncelemeIstek>
{
    public SosyalIncelemeIstekDogrulayici()
    {
        RuleFor(x => x.AdSoyad).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Tarih).NotEmpty();
        RuleFor(x => x.Durum).NotEmpty().MaximumLength(30);
        RuleFor(x => x.TcKimlikNo).MaximumLength(11);
    }
}

public record PsikolojikDegerlendirmeDto(
    int Id, int SakinId, string SakinAd, DateTime Tarih, string Tur, int? Puan, string? SosyalDurum, string? UzmanGorusu,
    string? Oneriler, string OnayDurumu, string? Onaylayan, DateTime? OnayTarihi, string? OnayNotu);

public record PsikolojikDegerlendirmeIstek(int SakinId, DateTime Tarih, string Tur, int? Puan, string? SosyalDurum, string? UzmanGorusu, string? Oneriler);

public class PsikolojikDegerlendirmeIstekDogrulayici : AbstractValidator<PsikolojikDegerlendirmeIstek>
{
    public PsikolojikDegerlendirmeIstekDogrulayici()
    {
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.Tarih).NotEmpty();
        RuleFor(x => x.Tur).Must(v => v is "Psikolojik" or "Sosyal");
        RuleFor(x => x.Puan).InclusiveBetween(0, 100).When(x => x.Puan is not null);
    }
}
