namespace Huzurevi.API.Models;

public abstract class TemelVarlik
{
    public int Id { get; set; }
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? GuncellenmeTarihi { get; set; }
    public bool SilindiMi { get; set; } = false;
    public DateTime? SilinmeTarihi { get; set; }
}