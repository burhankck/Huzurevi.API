namespace Huzurevi.Domain.Entities;

public class SakinSosyalGuvence : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string GuvenceTuru { get; set; } = string.Empty;
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public string? Aciklama { get; set; }
    public bool AktifMi { get; set; } = true;
}
