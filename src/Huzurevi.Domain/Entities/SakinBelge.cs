namespace Huzurevi.Domain.Entities;

public class SakinBelge : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string Grup { get; set; } = "Genel";
    public string BelgeTuru { get; set; } = string.Empty;
    public DateTime? BelgeTarihi { get; set; }
    public DateTime? GecerlilikTarihi { get; set; }
    public string? Aciklama { get; set; }
    public bool AktifMi { get; set; } = true;
    public string OrijinalAd { get; set; } = string.Empty;
    public string SaklamaYolu { get; set; } = string.Empty;
    public string IcerikTipi { get; set; } = string.Empty;
    public long Boyut { get; set; }
}
