namespace Huzurevi.Domain.Entities;

public class Rol : TemelVarlik
{
    public string Kod { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
    public bool SistemRoluMu { get; set; }
    public bool AktifMi { get; set; } = true;
}

public class RolIzin : TemelVarlik
{
    public int RolId { get; set; }
    public Rol? Rol { get; set; }
    public string IzinKodu { get; set; } = string.Empty;
}
