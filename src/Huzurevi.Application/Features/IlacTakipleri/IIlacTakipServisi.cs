namespace Huzurevi.Application.Features.IlacTakipleri;

public interface IIlacTakipServisi
{
    Task<List<IlacTakipDto>> TumunuGetirAsync(CancellationToken ct = default);
    Task<IlacTakipDto> OlusturAsync(IlacTakipOlusturIstek request, CancellationToken ct = default);
    Task<IlacTakipDurumDto> DurumDegistirAsync(int id, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
}
