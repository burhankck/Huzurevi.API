namespace Huzurevi.Application.Features.Ziyaretler;

public interface IZiyaretServisi
{
    Task<List<ZiyaretDto>> TumunuGetirAsync(int? sakinId, bool? iceride, CancellationToken ct = default);
    Task<List<ZiyaretDto>> IceridekileriGetirAsync(CancellationToken ct = default);
    Task<ZiyaretDto> GirisYapAsync(ZiyaretGirisIstek istek, CancellationToken ct = default);
    Task CikisYapAsync(int id, CancellationToken ct = default);
}
