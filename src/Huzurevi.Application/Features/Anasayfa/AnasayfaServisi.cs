using Huzurevi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Anasayfa;

public class AnasayfaServisi : IAnasayfaServisi
{
    private readonly IUygulamaDbContext _db;

    public AnasayfaServisi(IUygulamaDbContext db)
    {
        _db = db;
    }

    public async Task<AnasayfaIstatistikDto> IstatistikleriGetirAsync(CancellationToken ct = default)
    {
        var toplamSakin = await _db.Sakinler.CountAsync(ct);
        var toplamOda = await _db.Odalar.CountAsync(ct);
        var toplamYatak = await _db.Yataklar.CountAsync(ct);
        var doluYatak = await _db.Yataklar.CountAsync(y => y.DoluMu, ct);
        var bosYatak = toplamYatak - doluYatak;
        var dolulukOrani = toplamYatak > 0
            ? (int)Math.Round(doluYatak * 100.0 / toplamYatak)
            : 0;

        var iceridekiZiyaretci = await _db.Ziyaretler.CountAsync(z => z.CikisTarihi == null, ct);

        var sonSakinler = await _db.Sakinler
            .AsNoTracking()
            .OrderByDescending(s => s.Id)
            .Take(5)
            .Select(s => new SonSakinDto(s.Id, s.Ad, s.Soyad, s.TcKimlikNo, s.Durum))
            .ToListAsync(ct);

        return new AnasayfaIstatistikDto(
            toplamSakin,
            toplamOda,
            toplamYatak,
            doluYatak,
            bosYatak,
            dolulukOrani,
            iceridekiZiyaretci,
            sonSakinler);
    }
}
