namespace Huzurevi.Domain.Entities;

public class OnayYetkisi : TemelVarlik
{
    public int KullaniciId { get; set; }
    public Kullanici? Kullanici { get; set; }
    public string Alan { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;
}

public class IzinSureci : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public string IzinTuru { get; set; } = "Günlük";
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public string? TeslimAlan { get; set; }
    public string? Notlar { get; set; }
    public string OnayDurumu { get; set; } = "Bekliyor";
    public string? Onaylayan { get; set; }
    public DateTime? OnayTarihi { get; set; }
    public string? OnayNotu { get; set; }
}

public class EsyaTespit : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public string Kategori { get; set; } = string.Empty;
    public int Adet { get; set; } = 1;
    public string? Aciklama { get; set; }
    public DateTime TespitTarihi { get; set; }
    public string OnayDurumu { get; set; } = "Bekliyor";
    public string? Onaylayan { get; set; }
    public DateTime? OnayTarihi { get; set; }
    public string? OnayNotu { get; set; }
}

public class MirasciTeslim : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public int? MirasciId { get; set; }
    public Mirasci? Mirasci { get; set; }
    public string TeslimAlan { get; set; } = string.Empty;
    public string? TeslimAlanTelefon { get; set; }
    public DateTime TeslimTarihi { get; set; }
    public string? EsyaOzeti { get; set; }
    public string? Notlar { get; set; }
    public string? EvrakYolu { get; set; }
    public string OnayDurumu { get; set; } = "Bekliyor";
    public string? Onaylayan { get; set; }
    public DateTime? OnayTarihi { get; set; }
    public string? OnayNotu { get; set; }
}

public class SosyalInceleme : TemelVarlik
{
    public int? SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public DateTime Tarih { get; set; }
    public string Durum { get; set; } = "Taslak";
    public string? BasvuruYapan { get; set; }
    public string? TcKimlikNo { get; set; }
    public string AdSoyad { get; set; } = string.Empty;
    public DateTime? DogumTarihi { get; set; }
    public string? DogumYeri { get; set; }
    public string? Cinsiyet { get; set; }
    public string? MedeniDurum { get; set; }
    public string? EgitimDurumu { get; set; }
    public string? Telefon { get; set; }
    public string? Eposta { get; set; }
    public string? Adres { get; set; }
    public string? GelirKaynagi { get; set; }
    public string? AylikGelir { get; set; }
    public string? SosyalGuvence { get; set; }
    public string? Mulk { get; set; }
    public string? AileUyeleri { get; set; }
    public string? YakinlikDereceleri { get; set; }
    public string? Iletisim { get; set; }
    public string? KronikHastaliklar { get; set; }
    public string? KullanilanIlaclar { get; set; }
    public string? EngelDurumu { get; set; }
    public string? BakimIhtiyaci { get; set; }
    public string? UzmanGorusu { get; set; }
    public string? Oneri { get; set; }
    public string? Sonuc { get; set; }
    public string OnayDurumu { get; set; } = "Bekliyor";
    public string? Onaylayan { get; set; }
    public DateTime? OnayTarihi { get; set; }
    public string? OnayNotu { get; set; }
}

public class PsikolojikDegerlendirme : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }
    public DateTime Tarih { get; set; }
    public string Tur { get; set; } = "Psikolojik";
    public int? Puan { get; set; }
    public string? SosyalDurum { get; set; }
    public string? UzmanGorusu { get; set; }
    public string? Oneriler { get; set; }
    public string OnayDurumu { get; set; } = "Bekliyor";
    public string? Onaylayan { get; set; }
    public DateTime? OnayTarihi { get; set; }
    public string? OnayNotu { get; set; }
}
