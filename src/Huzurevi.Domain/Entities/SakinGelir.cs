namespace Huzurevi.Domain.Entities;

public class SakinGelir : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string GelirTuru { get; set; } = string.Empty;
    public string Periyot { get; set; } = "Aylık";
    public decimal? Deger { get; set; }
    public string? Aciklama { get; set; }
    public bool AktifMi { get; set; } = true;
}
