namespace Huzurevi.Domain.Entities;

public class SakinSaglikDegerlendirme : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string Tur { get; set; } = string.Empty;
    public DateTime Tarih { get; set; }
    public string Durum { get; set; } = string.Empty;
    public string? Seviye { get; set; }
    public string? Taraf { get; set; }
    public bool YardimciAracMi { get; set; }
    public string? Aciklama { get; set; }
    public bool AktifMi { get; set; } = true;
}
