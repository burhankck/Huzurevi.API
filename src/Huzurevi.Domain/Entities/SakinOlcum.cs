namespace Huzurevi.Domain.Entities;

public class SakinOlcum : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public DateTime Tarih { get; set; }
    public decimal? BoyCm { get; set; }
    public decimal? KiloKg { get; set; }
    public string? KanGrubu { get; set; }
    public string? Aciklama { get; set; }
    public bool AktifMi { get; set; } = true;
}
