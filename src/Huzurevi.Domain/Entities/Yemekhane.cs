namespace Huzurevi.Domain.Entities;

public class Yemek : TemelVarlik
{
    public string Ad { get; set; } = string.Empty;
    public string Kategori { get; set; } = "Ana yemek";
    public decimal? Kalori { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Karbonhidrat { get; set; }
    public decimal? Yag { get; set; }
    public string? Alerjenler { get; set; }
    public string? Tekstur { get; set; }
    public bool AktifMi { get; set; } = true;
}

public class BeslenmeProfili : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public string? TeksturTercihi { get; set; }
    public decimal? HedefKalori { get; set; }
    public decimal? HedefProtein { get; set; }
    public decimal? HedefSiviMl { get; set; }
    public string? Notlar { get; set; }
}

public class GunlukMenu : TemelVarlik
{
    public DateTime Tarih { get; set; }
    public string OgunTipi { get; set; } = "Öğle";
    public int YemekId { get; set; }
    public Yemek? Yemek { get; set; }
}

public class OzelMenuPlani : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public DateTime Tarih { get; set; }
    public string OgunTipi { get; set; } = "Öğle";
    public int YemekId { get; set; }
    public Yemek? Yemek { get; set; }
    public string? TeksturMod { get; set; }
    public decimal? SiviMl { get; set; }
    public string? Notlar { get; set; }
}

public class YemekTuketim : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public DateTime Tarih { get; set; }
    public string OgunTipi { get; set; } = "Öğle";
    public int YemekId { get; set; }
    public Yemek? Yemek { get; set; }
    public bool TuketildiMi { get; set; }
    public string? TuketilmemeNedeni { get; set; }
}

public class SiviAlimi : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public DateTime Tarih { get; set; }
    public string SiviTuru { get; set; } = "Su";
    public decimal MiktarMl { get; set; }
}
