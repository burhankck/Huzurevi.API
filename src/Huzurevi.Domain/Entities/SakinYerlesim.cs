namespace Huzurevi.Domain.Entities;

public class SakinYerlesim : TemelVarlik
{
    public int SakinId { get; set; }
    public Sakin? Sakin { get; set; }

    public int? OdaId { get; set; }
    public Oda? Oda { get; set; }
    public int? YatakId { get; set; }
    public Yatak? Yatak { get; set; }

    public string OdaNumarasi { get; set; } = string.Empty;
    public string YatakNumarasi { get; set; } = string.Empty;
    public string? Blok { get; set; }
    public int Kat { get; set; }

    public DateTime GirisTarihi { get; set; }
    public DateTime? CikisTarihi { get; set; }
}
