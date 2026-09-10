namespace Huzurevi.Domain.Entities;

public class SakinSaglikKayit : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string Tur { get; set; } = string.Empty;
    public DateTime Tarih { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string? DurumTipi { get; set; }
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public DateTime? AmeliyatTarihi { get; set; }
    public string? Hastane { get; set; }
    public string? Komplikasyon { get; set; }
    public string? MarkaModel { get; set; }
    public string? SeriNo { get; set; }
    public DateTime? TeminTarihi { get; set; }
    public DateTime? KontrolTarihi { get; set; }
    public string? Taraf { get; set; }
    public string? Derece { get; set; }
    public string? Bolge { get; set; }
    public string? Teshis { get; set; }
    public string? Doz { get; set; }
    public string? KullanimSikligi { get; set; }
    public string? UygulamaYolu { get; set; }
    public bool ReceteliMi { get; set; }
    public string? ReceteNo { get; set; }
    public string? ZamanlamaTipi { get; set; }
    public string? ZamanDilimleri { get; set; }
    public string? ReaksiyonTipi { get; set; }
    public string? Aciklama { get; set; }
    public bool AktifMi { get; set; } = true;
}
