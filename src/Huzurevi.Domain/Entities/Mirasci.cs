namespace Huzurevi.Domain.Entities;

public class Mirasci : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string? TcKimlikNo { get; set; }
    public string? Telefon { get; set; }
    public string Yakinlik { get; set; } = string.Empty;
    public string? Adres { get; set; }
}
