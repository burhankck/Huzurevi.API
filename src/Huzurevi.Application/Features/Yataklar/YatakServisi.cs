using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Yataklar;

public class YatakServisi : IYatakServisi
{
    private readonly IUygulamaDbContext _db;

    public YatakServisi(IUygulamaDbContext db)
    {
        _db = db;
    }

    public async Task<List<YatakDto>> TumunuGetirAsync(CancellationToken ct = default)
    {
        return await _db.Yataklar
            .AsNoTracking()
            .Include(y => y.Oda)
            .OrderBy(y => y.OdaId)
            .ThenBy(y => y.YatakNumarasi)
            .Select(y => new YatakDto(
                y.Id,
                y.YatakNumarasi,
                y.DoluMu,
                y.OdaId,
                y.Oda != null ? y.Oda.OdaNumarasi : null,
                y.SakinId))
            .ToListAsync(ct);
    }

    public async Task<List<BosYatakDto>> BosYataklariGetirAsync(CancellationToken ct = default)
    {
        return await _db.Yataklar
            .AsNoTracking()
            .Include(y => y.Oda)
            .Where(y => !y.DoluMu && y.SakinId == null && y.Oda != null && y.Oda.Durum == "Aktif")
            .OrderBy(y => y.Oda!.OdaNumarasi)
            .ThenBy(y => y.YatakNumarasi)
            .Select(y => new BosYatakDto(
                y.Id,
                y.YatakNumarasi,
                y.OdaId,
                y.Oda != null ? y.Oda.OdaNumarasi : string.Empty))
            .ToListAsync(ct);
    }

    public async Task<YatakDto> OlusturAsync(YatakOlusturIstek request, CancellationToken ct = default)
    {
        var oda = await _db.Odalar
            .Include(o => o.Yataklar)
            .FirstOrDefaultAsync(o => o.Id == request.OdaId, ct)
            ?? throw new KayitBulunamadiHatasi("Bağlanmak istenen oda bulunamadı.");

        if (oda.Yataklar.Count >= oda.Kapasite)
        {
            throw new GecersizIstekHatasi("Odanın yatak kapasitesi dolmuştur.");
        }

        var yatakNumarasi = request.YatakNumarasi.Trim();
        var ayniYatakVarMi = oda.Yataklar.Any(y =>
            y.YatakNumarasi.Equals(yatakNumarasi, StringComparison.OrdinalIgnoreCase));
        if (ayniYatakVarMi)
        {
            throw new CakismaHatasi("Bu odada aynı yatak numarası zaten var.");
        }

        var yatak = new Yatak
        {
            YatakNumarasi = yatakNumarasi,
            OdaId = oda.Id,
            DoluMu = false,
            OlusturulmaTarihi = DateTime.UtcNow
        };

        _db.Yataklar.Add(yatak);
        await _db.SaveChangesAsync(ct);

        return new YatakDto(yatak.Id, yatak.YatakNumarasi, yatak.DoluMu, yatak.OdaId, oda.OdaNumarasi, yatak.SakinId);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var yatak = await _db.Yataklar.FirstOrDefaultAsync(y => y.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Yatak bulunamadı.");

        if (yatak.DoluMu || yatak.SakinId != null)
        {
            throw new GecersizIstekHatasi("Dolu yatak silinemez. Önce sakini başka yatağa taşıyın.");
        }

        var simdi = DateTime.UtcNow;
        yatak.SilindiMi = true;
        yatak.SilinmeTarihi = simdi;
        yatak.GuncellenmeTarihi = simdi;
        await _db.SaveChangesAsync(ct);
    }

    public async Task AtaAsync(YatakAtamaIstek request, CancellationToken ct = default)
    {
        var yatak = await _db.Yataklar.Include(y => y.Oda).FirstOrDefaultAsync(y => y.Id == request.YatakId, ct)
            ?? throw new KayitBulunamadiHatasi("Yatak bulunamadı.");

        if (yatak.Oda is null || yatak.Oda.Durum != "Aktif")
        {
            throw new GecersizIstekHatasi("Bakımda veya kapalı odaya sakin yerleştirilemez.");
        }

        if (yatak.DoluMu || yatak.SakinId != null)
        {
            throw new GecersizIstekHatasi("Bu yatak zaten dolu.");
        }

        var sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == request.SakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");

        if (sakin.Durum != "Aktif")
        {
            throw new GecersizIstekHatasi("Yalnızca aktif sakinlere yatak atanabilir.");
        }

        var mevcutYatak = await _db.Yataklar.FirstOrDefaultAsync(y => y.SakinId == sakin.Id, ct);
        var girisTarihi = SakinYerlesimYardimcisi.ToUtc(request.GirisTarihi);

        if (mevcutYatak != null)
        {
            await SakinYerlesimYardimcisi.AciklariKapatAsync(_db, sakin.Id, girisTarihi, ct);
            mevcutYatak.SakinId = null;
            mevcutYatak.DoluMu = false;
            mevcutYatak.GuncellenmeTarihi = DateTime.UtcNow;
        }

        yatak.SakinId = sakin.Id;
        yatak.DoluMu = true;
        yatak.GuncellenmeTarihi = DateTime.UtcNow;

        sakin.YatakId = yatak.Id;
        sakin.GuncellenmeTarihi = DateTime.UtcNow;

        _db.SakinYerlesimleri.Add(SakinYerlesimYardimcisi.Olustur(yatak, sakin.Id, girisTarihi));

        await _db.SaveChangesAsync(ct);
    }

    public async Task BosaltAsync(int sakinId, YatakBosaltIstek istek, CancellationToken ct = default)
    {
        var sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == sakinId, ct);
        var yatak = await _db.Yataklar.FirstOrDefaultAsync(y => y.SakinId == sakinId, ct);

        if (yatak == null && (sakin == null || sakin.YatakId == null))
        {
            throw new KayitBulunamadiHatasi("Bu sakine ait atanmış bir yatak bulunamadı.");
        }

        var cikisTarihi = SakinYerlesimYardimcisi.ToUtc(istek.CikisTarihi);
        await SakinYerlesimYardimcisi.AciklariKapatAsync(_db, sakinId, cikisTarihi, ct);

        if (yatak != null)
        {
            yatak.SakinId = null;
            yatak.DoluMu = false;
            yatak.GuncellenmeTarihi = DateTime.UtcNow;
        }

        if (sakin != null)
        {
            sakin.YatakId = null;
            sakin.GuncellenmeTarihi = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<YerlesimGecmisDto>> GecmisiGetirAsync(int sakinId, CancellationToken ct = default)
    {
        if (!await _db.Sakinler.AnyAsync(s => s.Id == sakinId, ct))
        {
            throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        }

        var kayitlar = await _db.SakinYerlesimleri.AsNoTracking()
            .Where(x => x.SakinId == sakinId)
            .OrderByDescending(x => x.GirisTarihi)
            .ThenByDescending(x => x.Id)
            .ToListAsync(ct);

        return kayitlar.Select(x => new YerlesimGecmisDto(
            x.Id,
            x.SakinId,
            x.OdaNumarasi,
            x.YatakNumarasi,
            x.Blok,
            x.Kat,
            x.GirisTarihi,
            x.CikisTarihi,
            x.CikisTarihi is null)).ToList();
    }
}
