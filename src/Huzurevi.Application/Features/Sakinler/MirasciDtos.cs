namespace Huzurevi.Application.Features.Sakinler;

public record MirasciDto(
    int Id,
    int SakinId,
    string Ad,
    string Soyad,
    string? TcKimlikNo,
    string? Telefon,
    string Yakinlik,
    string? Adres);

public record MirasciOlusturIstek(
    string Ad,
    string Soyad,
    string? TcKimlikNo,
    string? Telefon,
    string Yakinlik,
    string? Adres);

public record MirasciGuncelleIstek(
    string Ad,
    string Soyad,
    string? TcKimlikNo,
    string? Telefon,
    string Yakinlik,
    string? Adres);
