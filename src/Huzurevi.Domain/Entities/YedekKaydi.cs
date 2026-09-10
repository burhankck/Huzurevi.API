namespace Huzurevi.Domain.Entities;

public class YedekKaydi
{
    public int Id { get; set; }
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
    public string Ad { get; set; } = string.Empty;
    public string Tur { get; set; } = "Manuel";
    public string Durum { get; set; } = "Tamam";
    public long Boyut { get; set; }
    public string VeritabaniYolu { get; set; } = string.Empty;
    public string? DosyaArsivYolu { get; set; }
    public string? HataMesaji { get; set; }
    public string Olusturan { get; set; } = string.Empty;
}
