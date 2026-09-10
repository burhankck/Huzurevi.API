namespace Huzurevi.API.Models;

public class Sakin : TemelVarlik
{
    public string TcKimlikNo { get; set; } = string.Empty; // Benzersiz TC Kimlik No
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public DateTime DogumTarihi { get; set; }
    public DateTime KabulTarihi { get; set; } = DateTime.UtcNow;
    public string Durum { get; set; } = "Aktif"; // Aktif, İzinli, Ayrıldı

    // Sakinin kaldığı yatak
    public int? YatakId { get; set; }
    public Yatak? Yatak { get; set; }
}