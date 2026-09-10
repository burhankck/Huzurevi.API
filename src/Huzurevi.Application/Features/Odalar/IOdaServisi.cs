namespace Huzurevi.Application.Features.Odalar;

public interface IOdaServisi
{
    Task<List<OdaDto>> TumunuGetirAsync(CancellationToken ct = default);
    Task<OdaDto> GetirAsync(int id, CancellationToken ct = default);
    Task<OdaDto> OlusturAsync(OdaOlusturIstek request, CancellationToken ct = default);
    Task GuncelleAsync(int id, OdaGuncelleIstek request, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
}
