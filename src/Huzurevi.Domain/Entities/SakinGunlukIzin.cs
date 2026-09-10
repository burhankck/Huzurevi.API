namespace Huzurevi.Domain.Entities;

public class SakinGunlukIzin : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public DateTime Tarih { get; set; }
    public string Yer { get; set; } = string.Empty;
    public string? CikisSaati { get; set; }
    public string? DonusSaati { get; set; }
    public bool AktifMi { get; set; } = true;
}
