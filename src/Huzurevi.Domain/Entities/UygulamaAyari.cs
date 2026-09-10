namespace Huzurevi.Domain.Entities;

public class UygulamaAyari
{
    public int Id { get; set; }
    public bool BakimModu { get; set; }
    public int SifreMinUzunluk { get; set; } = 6;
    public int MaksBasarisizGiris { get; set; } = 5;
    public int OturumDakika { get; set; } = 480;
    public int SifreGecerlilikGun { get; set; }
    public int LogSaklamaGun { get; set; } = 365;
    public bool YedeklemeAktifMi { get; set; }
    public int YedekSaat { get; set; } = 2;
    public int YedekSaklamaAdet { get; set; } = 14;
}
