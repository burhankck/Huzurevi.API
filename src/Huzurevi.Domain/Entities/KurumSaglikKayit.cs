namespace Huzurevi.Domain.Entities;

public class KurumSaglikKayit : TemelVarlik
{
    public string Tur { get; set; } = string.Empty;
    public int? SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public DateTime Tarih { get; set; }
    public string? Personel { get; set; }
    public string? Notlar { get; set; }
    public bool ImzalandiMi { get; set; }
    public string? Imzalayan { get; set; }
    public DateTime? ImzaTarihi { get; set; }

    public decimal? KanSekeri { get; set; }
    public string? OlcumZamani { get; set; }
    public int? Sistolik { get; set; }
    public int? Diastolik { get; set; }
    public int? Nabiz { get; set; }

    public string? Tedaviler { get; set; }
    public string? DurumDegerlendirme { get; set; }
    public string? HareketKabiliyeti { get; set; }
    public string? GucDenge { get; set; }

    public string? KayitTuru { get; set; }
    public string? YapilanIslemler { get; set; }
    public string? Malzeme { get; set; }

    public string? Nobetci { get; set; }
    public string? GenelDurum { get; set; }
    public string? OnemliOlaylar { get; set; }
    public string? DevirTeslim { get; set; }

    public string? Doktor { get; set; }
    public string? Bulgular { get; set; }
    public string? FizikMuayene { get; set; }
    public string? LabSonuclari { get; set; }
    public string? Oneriler { get; set; }
}
