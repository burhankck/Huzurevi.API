using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Yetkiler;
using System.Security.Claims;

namespace Huzurevi.API.Yetkilendirme;

public sealed class OturumBaglami : IOturumBaglami
{
    private readonly IHttpContextAccessor _http;

    public OturumBaglami(IHttpContextAccessor http) => _http = http;

    private ClaimsPrincipal? Kullanici => _http.HttpContext?.User;

    public int? KullaniciId
    {
        get
        {
            var deger = Kullanici?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Kullanici?.FindFirstValue("sub");
            return int.TryParse(deger, out var id) ? id : null;
        }
    }

    public int? KurulusId
    {
        get
        {
            var deger = Kullanici?.FindFirstValue("kurulusId");
            return int.TryParse(deger, out var id) ? id : null;
        }
    }

    public string? Rol =>
        Kullanici?.FindFirstValue(ClaimTypes.Role) ?? Kullanici?.FindFirstValue("role");

    public bool YoneticiMi => string.Equals(Rol, IzinKatalogu.Yonetici, StringComparison.Ordinal);

    public int? ListeFiltresi(int? istekKurulusId)
    {
        if (!YoneticiMi && KurulusId is int id)
            return id;
        return istekKurulusId;
    }

    public int YazmaKurulusId(int istekKurulusId)
    {
        KurulusDogrula(istekKurulusId);
        return !YoneticiMi && KurulusId is int id ? id : istekKurulusId;
    }

    public void KurulusDogrula(int? hedefKurulusId)
    {
        if (YoneticiMi || KurulusId is null)
            return;
        if (hedefKurulusId is null || hedefKurulusId != KurulusId)
            throw new ErisimEngellendiHatasi("Bu kuruluş verisine erişim yetkiniz yok.");
    }
}
