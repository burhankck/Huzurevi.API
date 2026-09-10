namespace Huzurevi.Domain.Entities;

public class Kullanici : TemelVarlik
{
    public string KullaniciAdi { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Eposta { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? TcKimlikNo { get; set; }
    public int? PersonelId { get; set; }
    public Personel? Personel { get; set; }
    public string SifreHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Personel";
    public bool AktifMi { get; set; } = true;
    public int BasarisizGirisSayisi { get; set; }
    public DateTime? SonGirisTarihi { get; set; }
    public DateTime? SifreDegistirilmeTarihi { get; set; }
    public string? SifreSifirlamaTokenHash { get; set; }
    public DateTime? SifreSifirlamaBitis { get; set; }
}
