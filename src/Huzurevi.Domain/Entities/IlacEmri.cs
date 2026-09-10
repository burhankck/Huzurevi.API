namespace Huzurevi.Domain.Entities;

public class IlacEmri : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public string IlacAdi { get; set; } = string.Empty;
    public string? Doz { get; set; }
    public string? Birim { get; set; }
    public string? KullanimSikligi { get; set; }
    public string? Zamanlama { get; set; }
    public DateTime BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public bool AktifMi { get; set; } = true;
    public string KayitTuru { get; set; } = "YeniEmir";
    public ICollection<IlacUygulama> Uygulamalar { get; set; } = new List<IlacUygulama>();
}

public class IlacUygulama : TemelVarlik
{
    public int IlacEmriId { get; set; }
    public IlacEmri? Emir { get; set; }
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public DateTime Tarih { get; set; }
    public string Durum { get; set; } = "Bekliyor";
    public string? Personel { get; set; }
    public string? Notlar { get; set; }
}
