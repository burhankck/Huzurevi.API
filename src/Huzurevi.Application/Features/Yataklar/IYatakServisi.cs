namespace Huzurevi.Application.Features.Yataklar;

public interface IYatakServisi
{
    Task<List<YatakDto>> TumunuGetirAsync(CancellationToken ct = default);
    Task<List<BosYatakDto>> BosYataklariGetirAsync(CancellationToken ct = default);
    Task<YatakDto> OlusturAsync(YatakOlusturIstek request, CancellationToken ct = default);
    Task GuncelleAsync(int id, YatakGuncelleIstek request, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task AtaAsync(YatakAtamaIstek request, CancellationToken ct = default);
    Task BosaltAsync(int sakinId, YatakBosaltIstek istek, CancellationToken ct = default);
    Task<List<YerlesimGecmisDto>> GecmisiGetirAsync(int sakinId, CancellationToken ct = default);
}
