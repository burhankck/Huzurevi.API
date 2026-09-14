namespace Huzurevi.Application.Common.Interfaces;

public interface IOturumBaglami
{
    int? KullaniciId { get; }
    int? KurulusId { get; }
    string? Rol { get; }
    bool YoneticiMi { get; }
    int? ListeFiltresi(int? istekKurulusId);
    int YazmaKurulusId(int istekKurulusId);
    void KurulusDogrula(int? hedefKurulusId);
}
