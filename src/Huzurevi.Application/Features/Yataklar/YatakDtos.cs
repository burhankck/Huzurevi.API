namespace Huzurevi.Application.Features.Yataklar;

public record YatakDto(
    int Id,
    string YatakNumarasi,
    bool DoluMu,
    int OdaId,
    string? OdaNumarasi,
    int? SakinId);

public record BosYatakDto(
    int Id,
    string YatakNumarasi,
    int OdaId,
    string OdaNumarasi);

public record YatakOlusturIstek(int OdaId, string YatakNumarasi);

public record YatakAtamaIstek(int YatakId, int SakinId, DateTime GirisTarihi);

public record YatakBosaltIstek(DateTime CikisTarihi);

public record YerlesimGecmisDto(
    int Id,
    int SakinId,
    string OdaNumarasi,
    string YatakNumarasi,
    string? Blok,
    int Kat,
    DateTime GirisTarihi,
    DateTime? CikisTarihi,
    bool AktifMi);
