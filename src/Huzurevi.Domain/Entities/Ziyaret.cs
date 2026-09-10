namespace Huzurevi.Domain.Entities;

public class Ziyaret : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string ZiyaretciAd { get; set; } = string.Empty;
    public string ZiyaretciSoyad { get; set; } = string.Empty;
    public string? TcKimlikNo { get; set; }
    public string? Telefon { get; set; }
    public string? Yakinlik { get; set; }
    public DateTime GirisTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? CikisTarihi { get; set; }
    public string? Notlar { get; set; }
}
