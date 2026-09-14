using Huzurevi.Application.Features.Yetkiler;

namespace Huzurevi.Application.Features.Kimlik;

public interface IKimlikServisi
{
    Task<GirisSonuc> GirisYapAsync(GirisIstek istek, CancellationToken ct = default);
    Task<KullaniciOzetDto> BeniGetirAsync(int kullaniciId, int? kurulusId, CancellationToken ct = default);
    Task<ProfilYetkileriDto> ProfilYetkileriGetirAsync(int kullaniciId, int? kurulusId, CancellationToken ct = default);
    Task<GirisSonuc> KurulusSecAsync(int kullaniciId, int kurulusId, CancellationToken ct = default);
    Task<KullaniciOzetDto> ProfilGuncelleAsync(int kullaniciId, ProfilGuncelleIstek istek, CancellationToken ct = default);
    Task<SifremiUnuttumSonuc> SifremiUnuttumAsync(SifremiUnuttumIstek istek, CancellationToken ct = default);
    Task SifreSifirlaAsync(SifreSifirlaIstek istek, CancellationToken ct = default);
}
