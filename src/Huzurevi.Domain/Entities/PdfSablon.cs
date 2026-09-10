namespace Huzurevi.Domain.Entities;

public class PdfSablon : TemelVarlik
{
    public string Kod { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
    public string Baslik { get; set; } = string.Empty;
    public string Icerik { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;
}

public class PdfArsiv : TemelVarlik
{
    public int SablonId { get; set; }
    public PdfSablon? Sablon { get; set; }
    public int? SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public string DosyaYolu { get; set; } = string.Empty;
    public string DosyaAdi { get; set; } = string.Empty;
    public string Olusturan { get; set; } = string.Empty;
}
