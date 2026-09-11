namespace Huzurevi.Domain.Entities;

public class OdaBakim : TemelVarlik
{
    public int OdaId { get; set; }
    public Oda? Oda { get; set; }

    public string BakimTuru { get; set; } = string.Empty;
    public DateTime BakimTarihi { get; set; }

    public int? PersonelId { get; set; }
    public Personel? Personel { get; set; }

    public int? SureDakika { get; set; }
    public decimal? Maliyet { get; set; }
    public string? Not { get; set; }
}
