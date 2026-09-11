namespace Huzurevi.Application.Features.Odalar;

public record OdaDto(
    int Id,
    string OdaNumarasi,
    string OdaNo,
    string Blok,
    int Kat,
    int Kapasite,
    string Durum,
    string? OdaTipi,
    List<string> Ozellikler,
    string? Notlar,
    int YatakSayisi,
    int DoluYatakSayisi,
    List<YatakOzetDto> Yataklar);

public record YatakOzetDto(
    int Id,
    string YatakNumarasi,
    bool DoluMu,
    string YatakTipi,
    List<string> Ozellikler,
    string Durum,
    SakinOzetDto? Sakin);

public record SakinOzetDto(int Id, string AdSoyad, string TcKimlikNo);

public record OdaOlusturIstek(
    string? OdaNo,
    string? OdaNumarasi,
    string? Blok,
    int Kat,
    int Kapasite,
    string? OdaTipi,
    List<string>? Ozellikler,
    string? Notlar);

public record OdaGuncelleIstek(
    string? OdaNo,
    string? OdaNumarasi,
    string? Blok,
    int Kat,
    int Kapasite,
    string? Durum,
    string? OdaTipi,
    List<string>? Ozellikler,
    string? Notlar);
