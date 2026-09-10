namespace Huzurevi.Domain.Entities;

public class Yatak : TemelVarlik
{
    public string YatakNumarasi { get; set; } = string.Empty;
    public bool DoluMu { get; set; }

    public int OdaId { get; set; }
    public Oda? Oda { get; set; }

    public int? SakinId { get; set; }
    public Sakin? Sakin { get; set; }
}
