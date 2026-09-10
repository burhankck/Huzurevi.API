using Huzurevi.Application.Common.Interfaces;

namespace Huzurevi.Application.Features.Yedekleme;

public record YedekDto(
    int Id,
    DateTime OlusturulmaTarihi,
    string Ad,
    string Tur,
    string Durum,
    long Boyut,
    string Olusturan,
    string? HataMesaji,
    bool DosyaArsiviVarMi);

public interface IYedeklemeServisi
{
    Task<List<YedekDto>> ListeleAsync(CancellationToken ct = default);
    Task<YedekDto> OlusturAsync(string olusturan, string tur, CancellationToken ct = default);
    Task GeriYukleAsync(int id, string olusturan, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task<KayitliDosya> IndirAsync(int id, string parca, CancellationToken ct = default);
}
