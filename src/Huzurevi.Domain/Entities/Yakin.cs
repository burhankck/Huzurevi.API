namespace Huzurevi.Domain.Entities;

public class Yakin : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Yakinlik { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? Eposta { get; set; }
    public string? Adres { get; set; }
    public string? Meslek { get; set; }
    public string? Aciklama { get; set; }
    public int Oncelik { get; set; } = 1;
    public bool AcilDurumKisisiMi { get; set; }
}
