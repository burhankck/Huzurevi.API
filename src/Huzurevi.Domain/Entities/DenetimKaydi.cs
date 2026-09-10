namespace Huzurevi.Domain.Entities;

public class DenetimKaydi
{
    public long Id { get; set; }
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
    public string Tur { get; set; } = "Erisim";
    public string Islem { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public string? KullaniciAdi { get; set; }
    public int? KullaniciId { get; set; }
    public string? IpAdresi { get; set; }
    public int? DurumKodu { get; set; }
    public string? Yol { get; set; }
    public string? HataTipi { get; set; }
    public string? TeknikDetay { get; set; }
}
