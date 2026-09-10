namespace Huzurevi.Domain.Entities;

public class Kurulus : TemelVarlik
{
    public string Ad { get; set; } = string.Empty;
    public string KisaAd { get; set; } = string.Empty;
    public string PlakaKodu { get; set; } = string.Empty;
    public string? Adres { get; set; }
    public string? Telefon { get; set; }
    public string? Dahili { get; set; }
    public bool AktifMi { get; set; } = true;
}

public class KullaniciKurulus : TemelVarlik
{
    public int KullaniciId { get; set; }
    public Kullanici? Kullanici { get; set; }
    public int KurulusId { get; set; }
    public Kurulus? Kurulus { get; set; }
    public string Rol { get; set; } = "Personel";
    public bool AktifMi { get; set; } = true;
}

public class Ulke : TemelVarlik
{
    public string Ad { get; set; } = string.Empty;
    public string Kod { get; set; } = string.Empty;
}

public class Il : TemelVarlik
{
    public int UlkeId { get; set; }
    public Ulke? Ulke { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string PlakaKodu { get; set; } = string.Empty;
}

public class Ilce : TemelVarlik
{
    public int IlId { get; set; }
    public Il? Il { get; set; }
    public string Ad { get; set; } = string.Empty;
}

public class Mahalle : TemelVarlik
{
    public int IlceId { get; set; }
    public Ilce? Ilce { get; set; }
    public string Ad { get; set; } = string.Empty;
}

public class GlobalTanim : TemelVarlik
{
    public string Kategori { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
    public string? Kod { get; set; }
    public bool AktifMi { get; set; } = true;
}

public class Personel : TemelVarlik
{
    public int KurulusId { get; set; }
    public Kurulus? Kurulus { get; set; }
    public string SicilNo { get; set; } = string.Empty;
    public string? TcKimlikNo { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string? Unvan { get; set; }
    public string? Meslek { get; set; }
    public string? Gorev { get; set; }
    public string Durum { get; set; } = "Aktif";
    public DateTime? DogumTarihi { get; set; }
    public string? Cinsiyet { get; set; }
    public string? KanGrubu { get; set; }
    public string? Eposta { get; set; }
    public string? Telefon { get; set; }
    public string? Adres { get; set; }
    public int? UlkeId { get; set; }
    public Ulke? Ulke { get; set; }
    public int? IlId { get; set; }
    public Il? Il { get; set; }
    public int? IlceId { get; set; }
    public Ilce? Ilce { get; set; }
    public int? MahalleId { get; set; }
    public Mahalle? Mahalle { get; set; }
    public DateTime? IseBaslamaTarihi { get; set; }
    public DateTime? AyrilisTarihi { get; set; }
    public string? Notlar { get; set; }
    public string? FotoYolu { get; set; }
}

public class PersonelYakin : TemelVarlik
{
    public int PersonelId { get; set; }
    public Personel? Personel { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Yakinlik { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? Adres { get; set; }
}

public class PersonelBelge : TemelVarlik
{
    public int PersonelId { get; set; }
    public Personel? Personel { get; set; }
    public string BelgeTuru { get; set; } = string.Empty;
    public DateTime? BelgeTarihi { get; set; }
    public string? Aciklama { get; set; }
    public string OrijinalAd { get; set; } = string.Empty;
    public string SaklamaYolu { get; set; } = string.Empty;
    public string IcerikTipi { get; set; } = string.Empty;
    public long Boyut { get; set; }
}

public class OrganizasyonBirimi : TemelVarlik
{
    public int KurulusId { get; set; }
    public Kurulus? Kurulus { get; set; }
    public int? UstBirimId { get; set; }
    public OrganizasyonBirimi? UstBirim { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string? Kod { get; set; }
    public bool AktifMi { get; set; } = true;
}

public class PersonelAtama : TemelVarlik
{
    public int PersonelId { get; set; }
    public Personel? Personel { get; set; }
    public int BirimId { get; set; }
    public OrganizasyonBirimi? Birim { get; set; }
    public string? Gorev { get; set; }
    public DateTime BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public bool AktifMi { get; set; } = true;
}
