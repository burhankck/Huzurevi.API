namespace Huzurevi.Application.Features.Kullanicilar;

public record KullaniciUyelikDto(int KurulusId, string KurulusAd, string Rol, bool AktifMi);
public record KullaniciUyelikIstek(int KurulusId, string Rol, bool AktifMi);

public record KullaniciListDto(
    int Id,
    string KullaniciAdi,
    string Ad,
    string Soyad,
    string Eposta,
    string? Telefon,
    string? TcKimlikNo,
    int? PersonelId,
    string? PersonelAd,
    string Rol,
    bool AktifMi,
    int BasarisizGirisSayisi,
    DateTime? SonGirisTarihi,
    List<KullaniciUyelikDto> Uyelikler);

public record KullaniciOlusturIstek(
    string KullaniciAdi,
    string Ad,
    string Soyad,
    string Eposta,
    string? Telefon,
    string? TcKimlikNo,
    int? PersonelId,
    string Rol,
    string Sifre,
    List<KullaniciUyelikIstek> Uyelikler);

public record KullaniciGuncelleIstek(
    string Ad,
    string Soyad,
    string Eposta,
    string? Telefon,
    string? TcKimlikNo,
    int? PersonelId,
    string Rol,
    bool AktifMi,
    List<KullaniciUyelikIstek> Uyelikler);

public record KullaniciSifreIstek(string YeniSifre);
