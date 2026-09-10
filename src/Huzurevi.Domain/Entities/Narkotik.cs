namespace Huzurevi.Domain.Entities;

public class NarkotikIlac : TemelVarlik
{
    public string Ad { get; set; } = string.Empty;
    public decimal Stok { get; set; }
    public string? Birim { get; set; }
    public bool AktifMi { get; set; } = true;
    public ICollection<NarkotikHareket> Hareketler { get; set; } = new List<NarkotikHareket>();
}

public class NarkotikHareket : TemelVarlik
{
    public int NarkotikIlacId { get; set; }
    public NarkotikIlac? Ilac { get; set; }
    public string HareketTuru { get; set; } = "Giris";
    public decimal Miktar { get; set; }
    public DateTime Tarih { get; set; }
    public int? SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public string? Notlar { get; set; }
}
