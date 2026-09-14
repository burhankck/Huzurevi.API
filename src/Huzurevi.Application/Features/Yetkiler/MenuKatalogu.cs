namespace Huzurevi.Application.Features.Yetkiler;

public record MenuOgeDto(string Label, string? Icon, string? To, string? Izin, List<MenuOgeDto>? Items);

public static class MenuKatalogu
{
    public static IReadOnlyList<MenuOgeDto> TamAgac { get; } =
    [
        new("Ana Sayfa", null, null, null,
        [
            new("Genel Bakış", "pi pi-fw pi-home", "/", "anasayfa.goruntule", null)
        ]),
        new("Kurum Yönetimi", null, null, null,
        [
            new("Sakin Yönetimi", "pi pi-fw pi-users", "/sakinler", "sakin.goruntule", null),
            new("Oda & Yatak", "pi pi-fw pi-th-large", "/odalar", "oda.goruntule", null),
            new("Yataklar", "pi pi-fw pi-table", "/yataklar", "oda.goruntule", null),
            new("Oda Planı", "pi pi-fw pi-map", "/oda-plani", "oda.goruntule", null),
            new("Oda bakımı", "pi pi-fw pi-wrench", "/oda-bakim", "oda.goruntule", null),
            new("Ziyaretçiler", "pi pi-fw pi-id-card", "/ziyaretler", "ziyaret.goruntule", null),
            new("Personel", "pi pi-fw pi-id-card", "/personel", "personel.goruntule", null),
            new("Organizasyon", "pi pi-fw pi-sitemap", "/organizasyon", "organizasyon.goruntule", null)
        ]),
        new("Sağlık", null, null, null,
        [
            new("Kabul muayenesi", "pi pi-fw pi-verified", "/kabul-muayene", "sakin.goruntule", null),
            new("Fizyoterapi", "pi pi-fw pi-heart", "/fizyoterapi", "saglik.goruntule", null),
            new("Ölçümler", "pi pi-fw pi-chart-line", "/olcumler", "saglik.goruntule", null),
            new("Revir", "pi pi-fw pi-plus", "/revir", "saglik.goruntule", null),
            new("Narkotik stok", "pi pi-fw pi-box", "/narkotik", "narkotik.goruntule", null),
            new("Hemşire nöbet", "pi pi-fw pi-clock", "/hemsire-nobet", "saglik.goruntule", null),
            new("Periyodik muayene", "pi pi-fw pi-calendar", "/periyodik-muayene", "saglik.goruntule", null),
            new("İlaç emirleri", "pi pi-fw pi-list", "/ilac-takip", "saglik.goruntule", null)
        ]),
        new("Kurum süreçleri", null, null, null,
        [
            new("İzin süreçleri", "pi pi-fw pi-sign-out", "/izin-surecleri", "surec.goruntule", null),
            new("Eşya tespiti", "pi pi-fw pi-briefcase", "/esya-tespit", "surec.goruntule", null),
            new("Mirasçı teslim", "pi pi-fw pi-send", "/mirasci-teslim", "surec.goruntule", null),
            new("Sosyal inceleme", "pi pi-fw pi-file", "/sosyal-inceleme", "surec.goruntule", null),
            new("Psikolojik değerlendirme", "pi pi-fw pi-comments", "/psikolojik-degerlendirme", "surec.goruntule", null)
        ]),
        new("Yemekhane", null, null, null,
        [
            new("Yemekler", "pi pi-fw pi-book", "/yemekler", "yemekhane.goruntule", null),
            new("Beslenme profilleri", "pi pi-fw pi-user", "/beslenme-profilleri", "yemekhane.goruntule", null),
            new("Günlük menü", "pi pi-fw pi-calendar", "/gunluk-menu", "yemekhane.goruntule", null),
            new("Özel menü", "pi pi-fw pi-star", "/ozel-menu", "yemekhane.goruntule", null),
            new("Servis / tüketim", "pi pi-fw pi-check-square", "/yemek-servis", "yemekhane.goruntule", null),
            new("Sıvı alımı", "pi pi-fw pi-inbox", "/sivi-alimi", "yemekhane.goruntule", null),
            new("Beslenme özeti", "pi pi-fw pi-chart-bar", "/beslenme-ozet", "yemekhane.goruntule", null)
        ]),
        new("Kütüphane", null, null, null,
        [
            new("Dolap ve raf", "pi pi-fw pi-table", "/kutuphane-dolap", "kutuphane.goruntule", null),
            new("Kitaplar", "pi pi-fw pi-bookmark", "/kitaplar", "kutuphane.goruntule", null),
            new("Kopyalar", "pi pi-fw pi-clone", "/kitap-kopyalari", "kutuphane.goruntule", null),
            new("Ödünç / iade", "pi pi-fw pi-replay", "/kitap-odunc", "kutuphane.goruntule", null)
        ]),
        new("Sistem", null, null, null,
        [
            new("Kuruluşlar", "pi pi-fw pi-building", "/kuruluslar", "kurulus.goruntule", null),
            new("Adres tanımları", "pi pi-fw pi-map-marker", "/adres-tanimlari", "tanim.goruntule", null),
            new("Global tanımlar", "pi pi-fw pi-list", "/global-tanimlar", "tanim.goruntule", null),
            new("Kullanıcılar", "pi pi-fw pi-id-card", "/kullanicilar", "kullanici.goruntule", null),
            new("Roller ve yetkiler", "pi pi-fw pi-lock", "/roller", "rol.goruntule", null),
            new("PDF şablonları", "pi pi-fw pi-file-pdf", "/pdf-sablonlar", "cikti.goruntule", null),
            new("Sistem logları", "pi pi-fw pi-history", "/loglar", "log.goruntule", null),
            new("Yedekleme", "pi pi-fw pi-database", "/yedekleme", "yedek.goruntule", null),
            new("Onay yetkileri", "pi pi-fw pi-key", "/onay-yetkileri", "onay.goruntule", null),
            new("Genel ayarlar", "pi pi-fw pi-cog", "/genel-ayarlar", "ayar.goruntule", null)
        ])
    ];

    public static List<MenuOgeDto> Filtrele(IReadOnlyCollection<string> izinler)
    {
        bool izinVar(string? kod) => string.IsNullOrWhiteSpace(kod) || izinler.Contains(kod);

        return TamAgac
            .Select(grup =>
            {
                var maddeler = (grup.Items ?? [])
                    .Where(x => izinVar(x.Izin))
                    .ToList();
                return grup with { Items = maddeler };
            })
            .Where(grup => grup.Items is { Count: > 0 })
            .ToList();
    }
}

public record ProfilYetkileriDto(
    int? KurulusId,
    string? KurulusAd,
    string Rol,
    string? RolAd,
    List<string> Izinler,
    List<MenuOgeDto> Menu);
