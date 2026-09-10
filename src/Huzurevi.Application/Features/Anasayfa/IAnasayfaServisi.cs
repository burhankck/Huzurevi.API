namespace Huzurevi.Application.Features.Anasayfa;

public interface IAnasayfaServisi
{
    Task<AnasayfaIstatistikDto> IstatistikleriGetirAsync(CancellationToken ct = default);
}
