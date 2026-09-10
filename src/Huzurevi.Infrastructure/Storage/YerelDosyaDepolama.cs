using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;

namespace Huzurevi.Infrastructure.Storage;

public class YerelDosyaDepolama : IDosyaDepolama
{
    private readonly string _kok;

    public YerelDosyaDepolama(string kokDizin)
    {
        _kok = Path.GetFullPath(kokDizin);
        Directory.CreateDirectory(_kok);
    }

    public async Task<string> KaydetAsync(string klasor, string uzanti, Stream icerik, CancellationToken ct = default)
    {
        var guvenliUzanti = string.IsNullOrWhiteSpace(uzanti) ? ".bin" : uzanti.Trim();
        if (!guvenliUzanti.StartsWith('.'))
        {
            guvenliUzanti = "." + guvenliUzanti;
        }

        var goreceli = Path.Combine(klasor.Replace('\\', '/'), $"{Guid.NewGuid():N}{guvenliUzanti.ToLowerInvariant()}")
            .Replace('\\', '/');
        var tam = TamYol(goreceli);
        Directory.CreateDirectory(Path.GetDirectoryName(tam)!);

        await using var cikti = File.Create(tam);
        await icerik.CopyToAsync(cikti, ct);
        return goreceli;
    }

    public Task SilAsync(string? goreceliYol, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(goreceliYol))
        {
            return Task.CompletedTask;
        }

        var tam = TamYol(goreceliYol);
        if (File.Exists(tam))
        {
            File.Delete(tam);
        }

        return Task.CompletedTask;
    }

    public Task<KayitliDosya?> AcAsync(string goreceliYol, string icerikTipi, string indirmeAdi, CancellationToken ct = default)
    {
        var tam = TamYol(goreceliYol);
        if (!File.Exists(tam))
        {
            return Task.FromResult<KayitliDosya?>(null);
        }

        Stream akis = File.OpenRead(tam);
        return Task.FromResult<KayitliDosya?>(new KayitliDosya(akis, icerikTipi, indirmeAdi));
    }

    private string TamYol(string goreceliYol)
    {
        var birlesik = Path.GetFullPath(Path.Combine(_kok, goreceliYol.Replace('/', Path.DirectorySeparatorChar)));
        if (!birlesik.StartsWith(_kok, StringComparison.OrdinalIgnoreCase))
        {
            throw new GecersizIstekHatasi("Geçersiz dosya yolu.");
        }

        return birlesik;
    }
}
