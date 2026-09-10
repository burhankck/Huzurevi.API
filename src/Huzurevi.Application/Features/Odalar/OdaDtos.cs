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
    int YatakSayisi,
    int DoluYatakSayisi,
    List<YatakOzetDto> Yataklar);

public record YatakOzetDto(
    int Id,
    string YatakNumarasi,
    bool DoluMu,
    SakinOzetDto? Sakin);

public record SakinOzetDto(int Id, string AdSoyad, string TcKimlikNo);

public record OdaOlusturIstek(
    string? OdaNo,
    string? OdaNumarasi,
    string? Blok,
    int Kat,
    int Kapasite,
    string? OdaTipi);

public record OdaGuncelleIstek(
    string? OdaNo,
    string? OdaNumarasi,
    string? Blok,
    int Kat,
    int Kapasite,
    string? Durum,
    string? OdaTipi);
