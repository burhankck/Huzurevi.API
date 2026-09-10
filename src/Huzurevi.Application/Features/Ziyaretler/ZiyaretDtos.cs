namespace Huzurevi.Application.Features.Ziyaretler;

public record ZiyaretDto(
    int Id,
    int SakinId,
    string SakinAdSoyad,
    string ZiyaretciAd,
    string ZiyaretciSoyad,
    string? TcKimlikNo,
    string? Telefon,
    string? Yakinlik,
    DateTime GirisTarihi,
    DateTime? CikisTarihi,
    bool IcerideMi,
    string? Notlar);

public record ZiyaretGirisIstek(
    int SakinId,
    string ZiyaretciAd,
    string ZiyaretciSoyad,
    string? TcKimlikNo,
    string? Telefon,
    string? Yakinlik,
    string? Notlar);
