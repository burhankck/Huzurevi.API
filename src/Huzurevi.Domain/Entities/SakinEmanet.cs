namespace Huzurevi.Domain.Entities;

public class SakinEmanet : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public string IslemTuru { get; set; } = string.Empty;
    public string EmanetTuru { get; set; } = string.Empty;
    public string Durum { get; set; } = "Emanette";
    public int Adet { get; set; } = 1;
    public decimal? Deger { get; set; }
    public DateTime Tarih { get; set; }
    public string? Saat { get; set; }
    public string TeslimEden { get; set; } = string.Empty;
    public string TeslimAlan { get; set; } = string.Empty;
    public string? Aciklama { get; set; }
}
