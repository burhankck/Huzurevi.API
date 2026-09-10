namespace Huzurevi.Application.Common.Interfaces;

public record KayitliDosya(Stream Icerik, string IcerikTipi, string IndirmeAdi);

public interface IDosyaDepolama
{
    Task<string> KaydetAsync(string klasor, string uzanti, Stream icerik, CancellationToken ct = default);
    Task SilAsync(string? goreceliYol, CancellationToken ct = default);
    Task<KayitliDosya?> AcAsync(string goreceliYol, string icerikTipi, string indirmeAdi, CancellationToken ct = default);
}
