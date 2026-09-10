namespace Huzurevi.Application.Features.Yetkiler;

public record IzinTanim(string Kod, string Grup, string Islem);

public static class IzinKatalogu
{
    public const string Yonetici = "Yonetici";
    public const string Personel = "Personel";

    private static readonly string[] Crud = ["goruntule", "ekle", "duzenle", "sil"];

    public static IReadOnlyList<IzinTanim> Tum { get; } = Olustur();

    public static IReadOnlyList<string> TumKodlar { get; } = Tum.Select(x => x.Kod).ToList();

    public static readonly HashSet<string> SistemKaynaklari =
        ["kurulus", "tanim", "kullanici", "rol", "ayar", "personel", "organizasyon", "onay", "cikti", "log", "yedek"];

    public static IReadOnlyList<string> PersonelVarsayilan { get; } = Tum
        .Where(x =>
        {
            var kaynak = x.Kod.Split('.')[0];
            if (SistemKaynaklari.Contains(kaynak)) return false;
            if (x.Kod is "sakin.sil" or "oda.sil") return false;
            return true;
        })
        .Select(x => x.Kod)
        .ToList();

    private static List<IzinTanim> Olustur()
    {
        var liste = new List<IzinTanim>();
        void Grup(string kaynak, string grup, params string[] islemler)
        {
            foreach (var islem in islemler)
            {
                var ad = islem switch
                {
                    "goruntule" => "Görüntüle",
                    "ekle" => "Ekle",
                    "duzenle" => "Düzenle",
                    "sil" => "Sil",
                    _ => islem
                };
                liste.Add(new IzinTanim($"{kaynak}.{islem}", grup, ad));
            }
        }

        Grup("anasayfa", "Ana sayfa", "goruntule");
        Grup("sakin", "Sakin", Crud);
        Grup("oda", "Oda ve yatak", Crud);
        Grup("ziyaret", "Ziyaretçiler", Crud);
        Grup("saglik", "Sağlık", Crud);
        Grup("surec", "Kurum süreçleri", Crud);
        Grup("yemekhane", "Yemekhane", Crud);
        Grup("kutuphane", "Kütüphane", Crud);
        Grup("personel", "Personel", Crud);
        Grup("organizasyon", "Organizasyon", Crud);
        Grup("kurulus", "Kuruluşlar", Crud);
        Grup("tanim", "Adres ve global tanımlar", Crud);
        Grup("kullanici", "Kullanıcılar", Crud);
        Grup("rol", "Roller ve yetkiler", Crud);
        Grup("onay", "Onay yetkileri", Crud);
        Grup("ayar", "Genel ayarlar", "goruntule", "duzenle");
        Grup("cikti", "PDF şablonları", Crud);
        Grup("log", "Sistem logları", "goruntule");
        Grup("yedek", "Yedekleme", Crud);
        return liste;
    }
}

public static class RolKodUretici
{
    public static string Uret(string? kod, string? ad)
    {
        var kaynak = string.IsNullOrWhiteSpace(kod) ? ad : kod;
        var latin = Latin(kaynak ?? "");
        var temiz = new string(latin.Where(c => char.IsAsciiLetterOrDigit(c) || c == '_').ToArray());
        if (string.IsNullOrWhiteSpace(temiz))
            throw new Huzurevi.Application.Common.Exceptions.GecersizIstekHatasi("Rol adı en az bir harf içermelidir.");
        return temiz.Length > 40 ? temiz[..40] : temiz;
    }

    private static string Latin(string metin)
    {
        return metin.Trim()
            .Replace("ç", "c").Replace("Ç", "C")
            .Replace("ğ", "g").Replace("Ğ", "G")
            .Replace("ı", "i").Replace("İ", "I").Replace("I", "I")
            .Replace("ö", "o").Replace("Ö", "O")
            .Replace("ş", "s").Replace("Ş", "S")
            .Replace("ü", "u").Replace("Ü", "U")
            .Replace(" ", "_")
            .Replace("-", "_");
    }
}
