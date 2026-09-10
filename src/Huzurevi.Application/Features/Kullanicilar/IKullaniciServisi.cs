namespace Huzurevi.Application.Features.Kullanicilar;

public interface IKullaniciServisi
{
    Task<List<KullaniciListDto>> TumunuGetirAsync(CancellationToken ct = default);
    Task<KullaniciListDto> GetirAsync(int id, CancellationToken ct = default);
    Task<KullaniciListDto> OlusturAsync(KullaniciOlusturIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, KullaniciGuncelleIstek istek, int islemYapanId, CancellationToken ct = default);
    Task SifreAtaAsync(int id, KullaniciSifreIstek istek, CancellationToken ct = default);
    Task KilidiAcAsync(int id, CancellationToken ct = default);
    Task SilAsync(int id, int islemYapanId, CancellationToken ct = default);
}
