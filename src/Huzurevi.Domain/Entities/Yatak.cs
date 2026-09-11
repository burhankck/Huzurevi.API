namespace Huzurevi.Domain.Entities;

public class Yatak : TemelVarlik
{
    public string YatakNumarasi { get; set; } = string.Empty;
    public bool DoluMu { get; set; }
    public string YatakTipi { get; set; } = "Standart";
    public string? Ozellikler { get; set; }
    public string Durum { get; set; } = "Aktif";

    public int OdaId { get; set; }
    public Oda? Oda { get; set; }

    public int? SakinId { get; set; }
    public Sakin? Sakin { get; set; }
}
