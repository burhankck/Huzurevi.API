namespace Huzurevi.Domain.Entities;

public class SakinMal : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string MalTuru { get; set; } = string.Empty;
    public int Adet { get; set; } = 1;
    public decimal? Deger { get; set; }
    public string? Adres { get; set; }
    public string? Aciklama { get; set; }
    public bool AktifMi { get; set; } = true;
}
