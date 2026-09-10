namespace Huzurevi.Domain.Entities;

public class KutuphaneDolap : TemelVarlik
{
    public string Ad { get; set; } = string.Empty;
    public string? Konum { get; set; }
    public bool AktifMi { get; set; } = true;
    public ICollection<KutuphaneRaf> Raflar { get; set; } = new List<KutuphaneRaf>();
}

public class KutuphaneRaf : TemelVarlik
{
    public int DolapId { get; set; }
    public KutuphaneDolap? Dolap { get; set; }
    public string Ad { get; set; } = string.Empty;
}

public class Kitap : TemelVarlik
{
    public string? Isbn { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string? Yazar { get; set; }
    public string? Yayinevi { get; set; }
    public string? Aciklama { get; set; }
    public string? KapakYolu { get; set; }
    public ICollection<KitapKopya> Kopyalar { get; set; } = new List<KitapKopya>();
}

public class KitapKopya : TemelVarlik
{
    public int KitapId { get; set; }
    public Kitap? Kitap { get; set; }
    public string Barkod { get; set; } = string.Empty;
    public string Durum { get; set; } = "Rafta";
    public int? RafId { get; set; }
    public KutuphaneRaf? Raf { get; set; }
    public string? Konum { get; set; }
}

public class KitapOdunc : TemelVarlik
{
    public int KopyaId { get; set; }
    public KitapKopya? Kopya { get; set; }
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public DateTime OduncTarihi { get; set; }
    public DateTime PlanlananTeslim { get; set; }
    public DateTime? IadeTarihi { get; set; }
    public string? DurumNotu { get; set; }
}
