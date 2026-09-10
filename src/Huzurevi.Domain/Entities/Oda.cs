namespace Huzurevi.Domain.Entities;

public class Oda : TemelVarlik
{
    public string OdaNumarasi { get; set; } = string.Empty;
    public string Blok { get; set; } = string.Empty;
    public int Kat { get; set; }
    public int Kapasite { get; set; }
    public string Durum { get; set; } = "Aktif";
    public string? OdaTipi { get; set; }

    public ICollection<Yatak> Yataklar { get; set; } = new List<Yatak>();
}
