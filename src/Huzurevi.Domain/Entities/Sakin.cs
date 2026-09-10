namespace Huzurevi.Domain.Entities;

public class Sakin : TemelVarlik
{
    public string TcKimlikNo { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public DateTime DogumTarihi { get; set; }
    public DateTime KabulTarihi { get; set; } = DateTime.UtcNow;
    public string Durum { get; set; } = "Aktif";

    public string? Cinsiyet { get; set; }
    public string? DogumYeri { get; set; }
    public string? Uyruk { get; set; }
    public string? MedeniDurum { get; set; }
    public string? KanGrubu { get; set; }
    public string? Adres { get; set; }
    public string? AcilTelefon { get; set; }
    public string? Notlar { get; set; }
    public string? FotoYolu { get; set; }
    public string? KayitNo { get; set; }
    public string? BabaAdi { get; set; }
    public string? AnaAdi { get; set; }
    public string? OgrenimDurumu { get; set; }
    public string? Meslek { get; set; }
    public string? EngelDurumu { get; set; }
    public string? NeredenGeldigi { get; set; }
    public string? OncekiYasamYeri { get; set; }
    public string? NufusKutukIli { get; set; }
    public string? UcretDurumu { get; set; }
    public decimal? AylikGelir { get; set; }
    public string? BasvuruDurumu { get; set; }
    public string? KayitTuru { get; set; }
    public string? AyrilisDurumu { get; set; }
    public DateTime? AyrilisTarihi { get; set; }
    public string? KabulNedeni { get; set; }
    public string? KabulSekli { get; set; }
    public string? SonOturduguAdres { get; set; }
    public string? KimGetirdi { get; set; }

    public int? YatakId { get; set; }
    public Yatak? Yatak { get; set; }

    public ICollection<IlacTakip> IlacTakipleri { get; set; } = new List<IlacTakip>();
    public ICollection<Yakin> Yakinlar { get; set; } = new List<Yakin>();
    public ICollection<Ziyaret> Ziyaretler { get; set; } = new List<Ziyaret>();
    public ICollection<Vasi> Vasiler { get; set; } = new List<Vasi>();
    public ICollection<Mirasci> Mirascilar { get; set; } = new List<Mirasci>();
    public ICollection<SakinBelge> Belgeler { get; set; } = new List<SakinBelge>();
    public ICollection<SakinMal> Mallar { get; set; } = new List<SakinMal>();
    public ICollection<SakinGelir> Gelirler { get; set; } = new List<SakinGelir>();
    public ICollection<SakinSosyalGuvence> SosyalGuvenceler { get; set; } = new List<SakinSosyalGuvence>();
    public ICollection<SakinGunlukIzin> GunlukIzinler { get; set; } = new List<SakinGunlukIzin>();
    public ICollection<SakinEmanet> Emanetler { get; set; } = new List<SakinEmanet>();
    public ICollection<SakinYerlesim> Yerlesimler { get; set; } = new List<SakinYerlesim>();
    public ICollection<SakinOlcum> Olcumler { get; set; } = new List<SakinOlcum>();
    public ICollection<SakinSaglikDegerlendirme> SaglikDegerlendirmeleri { get; set; } = new List<SakinSaglikDegerlendirme>();
    public ICollection<SakinSaglikKayit> SaglikKayitlari { get; set; } = new List<SakinSaglikKayit>();
}
