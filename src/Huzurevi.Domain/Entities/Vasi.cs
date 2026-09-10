namespace Huzurevi.Domain.Entities;

public class Vasi : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string? TcKimlikNo { get; set; }
    public DateTime? DogumTarihi { get; set; }
    public string? Telefon { get; set; }
    public string? Eposta { get; set; }
    public string? Adres { get; set; }
    public string? Yakinlik { get; set; }
    public string? MahkemeAdi { get; set; }
    public string? KararNo { get; set; }
    public DateTime? KararTarihi { get; set; }
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public string? Kapsam { get; set; }
    public string VasiTuru { get; set; } = "Yasal Vasi";
    public string? Sebep { get; set; }
    public string Durum { get; set; } = "Aktif";
    public string? Aciklama { get; set; }
}
