namespace Huzurevi.API.Models;

public class Yatak : TemelVarlik
{
    public string YatakNumarasi { get; set; } = string.Empty;
    public bool DoluMu { get; set; } = false;

    // Yatağın bağlı olduğu oda
    public int OdaId { get; set; }
    public Oda? Oda { get; set; }

    // Bu yatakta kalan sakin
    public Sakin? Sakin { get; set; }
}