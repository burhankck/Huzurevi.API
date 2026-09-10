namespace Huzurevi.Application.Features.IlacTakipleri;

public record IlacTakipDto(
    int Id,
    int SakinId,
    string SakinAdSoyad,
    string IlacAdi,
    string Dozaj,
    string Zaman,
    bool VerildiMi,
    DateTime KayitTarihi,
    string? Notlar);

public record IlacTakipOlusturIstek(
    int SakinId,
    string IlacAdi,
    string? Dozaj,
    string? Zaman,
    string? Notlar);

public record IlacTakipDurumDto(int Id, bool VerildiMi);
