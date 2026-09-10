using FluentValidation;

namespace Huzurevi.Application.Features.Sakinler;

internal static class SakinKurallari
{
    public static readonly string[] Durumlar = ["Aktif", "İzinli", "Ayrıldı"];
    public static readonly string[] Cinsiyetler = ["Erkek", "Kadın", "Belirtilmedi"];
    public static readonly string[] MedeniDurumlar = ["Bekar", "Evli", "Dul", "Boşanmış", "Belirtilmedi"];
    public static readonly string[] KanGruplari =
    [
        "0 Rh+", "0 Rh-", "A Rh+", "A Rh-", "B Rh+", "B Rh-", "AB Rh+", "AB Rh-", "Bilinmiyor"
    ];
    public static readonly string[] Yakinliklar =
    [
        "Eş", "Çocuk", "Torun", "Kardeş", "Ebeveyn", "Vasi", "Diğer"
    ];
    public static readonly string[] OgrenimDurumlari =
    [
        "Okur-yazar değil", "İlkokul", "Ortaokul", "Lise", "Ön lisans", "Lisans", "Lisansüstü", "Belirtilmedi"
    ];
    public static readonly string[] EngelDurumlari =
    [
        "Yok", "Bedensel", "Görme", "İşitme", "Zihinsel", "Birden fazla", "Belirtilmedi"
    ];
    public static readonly string[] UcretDurumlari = ["Ücretli", "Ücretsiz", "Kısmi", "Sosyal yardım"];
    public static readonly string[] BasvuruDurumlari = ["Başvuru", "Değerlendirme", "Kabul", "Red", "Arşiv"];
    public static readonly string[] KayitTurleri = ["Yeni kayıt", "Nakil", "Geçici", "Diğer"];
    public static readonly string[] AyrilisDurumlari = ["Kendi isteği", "Vefat", "Nakil", "Aile yanına", "Diğer"];
    public static readonly string[] KabulNedenleri = ["Yaşlılık", "Bakıma muhtaçlık", "Sosyal yoksunluk", "Diğer"];
    public static readonly string[] KabulSekilleri = ["Başvuru", "Mahkeme", "Sosyal hizmet", "Nakil"];
}

public class SakinOlusturIstekDogrulayici : AbstractValidator<SakinOlusturIstek>
{
    public SakinOlusturIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TcKimlikNo)
            .NotEmpty()
            .Length(11)
            .Matches(@"^\d{11}$")
            .WithMessage("T.C. Kimlik No 11 haneli rakamlardan oluşmalıdır.");
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Durum)
            .Must(durum => durum is null || SakinKurallari.Durumlar.Contains(durum))
            .WithMessage("Durum Aktif, İzinli veya Ayrıldı olmalıdır.");
    }
}

public class SakinGuncelleIstekDogrulayici : AbstractValidator<SakinGuncelleIstek>
{
    public SakinGuncelleIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.AcilTelefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.AcilTelefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.DogumYeri).MaximumLength(100);
        RuleFor(x => x.Uyruk).MaximumLength(50);
        RuleFor(x => x.Adres).MaximumLength(300);
        RuleFor(x => x.Notlar).MaximumLength(500);
        RuleFor(x => x.Durum)
            .Must(durum => durum is null || SakinKurallari.Durumlar.Contains(durum))
            .WithMessage("Durum Aktif, İzinli veya Ayrıldı olmalıdır.");
        RuleFor(x => x.Cinsiyet)
            .Must(v => v is null || SakinKurallari.Cinsiyetler.Contains(v))
            .WithMessage("Cinsiyet değeri geçersiz.");
        RuleFor(x => x.MedeniDurum)
            .Must(v => v is null || SakinKurallari.MedeniDurumlar.Contains(v))
            .WithMessage("Medeni durum değeri geçersiz.");
        RuleFor(x => x.KanGrubu)
            .Must(v => v is null || SakinKurallari.KanGruplari.Contains(v))
            .WithMessage("Kan grubu değeri geçersiz.");
        RuleFor(x => x.BabaAdi).MaximumLength(100);
        RuleFor(x => x.AnaAdi).MaximumLength(100);
        RuleFor(x => x.Meslek).MaximumLength(100);
        RuleFor(x => x.NeredenGeldigi).MaximumLength(150);
        RuleFor(x => x.OncekiYasamYeri).MaximumLength(150);
        RuleFor(x => x.NufusKutukIli).MaximumLength(50);
        RuleFor(x => x.AylikGelir).GreaterThanOrEqualTo(0).When(x => x.AylikGelir is not null);
        RuleFor(x => x.SonOturduguAdres).MaximumLength(300);
        RuleFor(x => x.KimGetirdi).MaximumLength(150);
        RuleFor(x => x.OgrenimDurumu).Must(v => v is null || SakinKurallari.OgrenimDurumlari.Contains(v));
        RuleFor(x => x.EngelDurumu).Must(v => v is null || SakinKurallari.EngelDurumlari.Contains(v));
        RuleFor(x => x.UcretDurumu).Must(v => v is null || SakinKurallari.UcretDurumlari.Contains(v));
        RuleFor(x => x.BasvuruDurumu).Must(v => v is null || SakinKurallari.BasvuruDurumlari.Contains(v));
        RuleFor(x => x.KayitTuru).Must(v => v is null || SakinKurallari.KayitTurleri.Contains(v));
        RuleFor(x => x.AyrilisDurumu).Must(v => v is null || SakinKurallari.AyrilisDurumlari.Contains(v));
        RuleFor(x => x.KabulNedeni).Must(v => v is null || SakinKurallari.KabulNedenleri.Contains(v));
        RuleFor(x => x.KabulSekli).Must(v => v is null || SakinKurallari.KabulSekilleri.Contains(v));
    }
}

public class YakinOlusturIstekDogrulayici : AbstractValidator<YakinOlusturIstek>
{
    public YakinOlusturIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Yakinlik)
            .NotEmpty()
            .Must(v => SakinKurallari.Yakinliklar.Contains(v))
            .WithMessage("Yakınlık derecesi geçersiz.");
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Eposta).EmailAddress().MaximumLength(150).When(x => !string.IsNullOrWhiteSpace(x.Eposta));
        RuleFor(x => x.Adres).MaximumLength(300);
        RuleFor(x => x.Meslek).MaximumLength(100);
        RuleFor(x => x.Aciklama).MaximumLength(500);
        RuleFor(x => x.Oncelik).InclusiveBetween(1, 99);
    }
}

public class YakinGuncelleIstekDogrulayici : AbstractValidator<YakinGuncelleIstek>
{
    public YakinGuncelleIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Yakinlik)
            .NotEmpty()
            .Must(v => SakinKurallari.Yakinliklar.Contains(v))
            .WithMessage("Yakınlık derecesi geçersiz.");
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Eposta).EmailAddress().MaximumLength(150).When(x => !string.IsNullOrWhiteSpace(x.Eposta));
        RuleFor(x => x.Adres).MaximumLength(300);
        RuleFor(x => x.Meslek).MaximumLength(100);
        RuleFor(x => x.Aciklama).MaximumLength(500);
        RuleFor(x => x.Oncelik).InclusiveBetween(1, 99);
    }
}
