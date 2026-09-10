namespace Huzurevi.Application.Features.Sakinler;

public interface ISakinServisi
{
    Task<List<SakinListDto>> TumunuGetirAsync(CancellationToken ct = default);
    Task<SakinDetayDto> GetirAsync(int id, CancellationToken ct = default);
    Task<SakinListDto> OlusturAsync(SakinOlusturIstek request, CancellationToken ct = default);
    Task GuncelleAsync(int id, SakinGuncelleIstek request, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);

    Task<List<YakinDto>> YakinlariGetirAsync(int sakinId, CancellationToken ct = default);
    Task<YakinDto> YakinOlusturAsync(int sakinId, YakinOlusturIstek request, CancellationToken ct = default);
    Task YakinGuncelleAsync(int sakinId, int yakinId, YakinGuncelleIstek request, CancellationToken ct = default);
    Task YakinSilAsync(int sakinId, int yakinId, CancellationToken ct = default);
    Task<List<KabulMuayeneDto>> KabulMuayeneleriniGetirAsync(string? arama, CancellationToken ct = default);
}
