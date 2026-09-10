namespace Huzurevi.Domain.Entities;

public class IlacTakip
{
    public int Id { get; set; }
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string IlacAdi { get; set; } = string.Empty;
    public string Dozaj { get; set; } = string.Empty;
    public string Zaman { get; set; } = string.Empty;
    public bool VerildiMi { get; set; }
    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;
    public string? Notlar { get; set; }
}
