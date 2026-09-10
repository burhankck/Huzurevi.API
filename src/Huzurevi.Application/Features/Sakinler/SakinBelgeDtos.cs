namespace Huzurevi.Application.Features.Sakinler;

public record SakinBelgeDto(
    int Id,
    int SakinId,
    string Grup,
    string BelgeTuru,
    DateTime? BelgeTarihi,
    DateTime? GecerlilikTarihi,
    string? Aciklama,
    bool AktifMi,
    string OrijinalAd,
    string IcerikTipi,
    long Boyut,
    bool PdfMi);

public record BelgeYukleIstek(
    string Grup,
    string BelgeTuru,
    DateTime? BelgeTarihi,
    DateTime? GecerlilikTarihi,
    string? Aciklama,
    bool AktifMi);

public record BelgeGuncelleIstek(
    string Grup,
    string BelgeTuru,
    DateTime? BelgeTarihi,
    DateTime? GecerlilikTarihi,
    string? Aciklama,
    bool AktifMi);
