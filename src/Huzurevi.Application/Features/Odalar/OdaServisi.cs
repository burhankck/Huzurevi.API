using Huzurevi.Application.Common;
using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Odalar;

public class OdaServisi : IOdaServisi
{
    private readonly IUygulamaDbContext _db;

    public OdaServisi(IUygulamaDbContext db)
    {
        _db = db;
    }

    public async Task<List<OdaDto>> TumunuGetirAsync(CancellationToken ct = default)
    {
        var odalar = await Sorgula().OrderBy(o => o.Kat).ThenBy(o => o.OdaNumarasi).ToListAsync(ct);
        return odalar.Select(Map).ToList();
    }

    public async Task<OdaDto> GetirAsync(int id, CancellationToken ct = default)
    {
        var oda = await Sorgula().FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Oda bulunamadı.");
        return Map(oda);
    }

    public async Task<OdaDto> OlusturAsync(OdaOlusturIstek request, CancellationToken ct = default)
    {
        var odaNumarasi = ResolveOdaNumarasi(request.OdaNumarasi, request.OdaNo);
        await NumaraBenzersizMiAsync(odaNumarasi, null, ct);

        var oda = new Oda
        {
            OdaNumarasi = odaNumarasi,
            Blok = request.Blok?.Trim() ?? string.Empty,
            Kat = request.Kat,
            Kapasite = request.Kapasite,
            Durum = "Aktif",
            OdaTipi = string.IsNullOrWhiteSpace(request.OdaTipi) ? "Standart" : request.OdaTipi.Trim(),
            Ozellikler = CokluSecim.Metin(request.Ozellikler),
            Notlar = string.IsNullOrWhiteSpace(request.Notlar) ? null : request.Notlar.Trim(),
            OlusturulmaTarihi = DateTime.UtcNow
        };

        for (var i = 1; i <= request.Kapasite; i++)
        {
            oda.Yataklar.Add(new Yatak
            {
                YatakNumarasi = i.ToString(),
                DoluMu = false,
                YatakTipi = "Standart",
                Durum = "Aktif",
                OlusturulmaTarihi = DateTime.UtcNow
            });
        }

        _db.Odalar.Add(oda);
        await _db.SaveChangesAsync(ct);

        return await GetirAsync(oda.Id, ct);
    }

    public async Task GuncelleAsync(int id, OdaGuncelleIstek request, CancellationToken ct = default)
    {
        var oda = await _db.Odalar
            .Include(o => o.Yataklar)
            .FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Oda bulunamadı.");

        var odaNumarasi = ResolveOdaNumarasi(request.OdaNumarasi, request.OdaNo);
        await NumaraBenzersizMiAsync(odaNumarasi, id, ct);

        var yatakSayisi = oda.Yataklar.Count(y => !y.SilindiMi);
        if (request.Kapasite < yatakSayisi)
        {
            throw new GecersizIstekHatasi(
                $"Kapasite {yatakSayisi} yatağın altına inemez. Önce boş yatakları silin.");
        }

        oda.OdaNumarasi = odaNumarasi;
        oda.Blok = request.Blok?.Trim() ?? string.Empty;
        oda.Kat = request.Kat;
        oda.Kapasite = request.Kapasite;
        oda.Durum = string.IsNullOrWhiteSpace(request.Durum) ? oda.Durum : request.Durum.Trim();
        oda.OdaTipi = string.IsNullOrWhiteSpace(request.OdaTipi) ? oda.OdaTipi : request.OdaTipi.Trim();
        oda.Ozellikler = CokluSecim.Metin(request.Ozellikler);
        oda.Notlar = string.IsNullOrWhiteSpace(request.Notlar) ? null : request.Notlar.Trim();
        oda.GuncellenmeTarihi = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var oda = await _db.Odalar
            .Include(o => o.Yataklar)
            .FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Oda bulunamadı.");

        if (oda.Yataklar.Any(y => y.DoluMu || y.SakinId != null))
        {
            throw new GecersizIstekHatasi("Dolu yatağı olan oda silinemez. Önce sakinleri başka odaya taşıyın.");
        }

        var simdi = DateTime.UtcNow;
        oda.SilindiMi = true;
        oda.SilinmeTarihi = simdi;
        oda.GuncellenmeTarihi = simdi;

        foreach (var yatak in oda.Yataklar)
        {
            yatak.SilindiMi = true;
            yatak.SilinmeTarihi = simdi;
            yatak.GuncellenmeTarihi = simdi;
        }

        await _db.SaveChangesAsync(ct);
    }

    private IQueryable<Oda> Sorgula() =>
        _db.Odalar
            .AsNoTracking()
            .Include(o => o.Yataklar)
                .ThenInclude(y => y.Sakin);

    private async Task NumaraBenzersizMiAsync(string odaNumarasi, int? haricId, CancellationToken ct)
    {
        var ayniNumaraVarMi = await _db.Odalar.AnyAsync(
            o => o.OdaNumarasi == odaNumarasi && (haricId == null || o.Id != haricId),
            ct);

        if (ayniNumaraVarMi)
        {
            throw new CakismaHatasi("Bu oda numarası zaten kayıtlı.");
        }
    }

    private static string ResolveOdaNumarasi(string? odaNumarasi, string? odaNo)
    {
        var numara = !string.IsNullOrWhiteSpace(odaNumarasi) ? odaNumarasi : odaNo;
        if (string.IsNullOrWhiteSpace(numara))
        {
            throw new GecersizIstekHatasi("Oda numarası zorunludur.");
        }

        return numara.Trim();
    }

    private static OdaDto Map(Oda oda)
    {
        var yataklar = oda.Yataklar.Where(y => !y.SilindiMi).ToList();
        return new OdaDto(
            oda.Id,
            oda.OdaNumarasi,
            oda.OdaNumarasi,
            oda.Blok,
            oda.Kat,
            oda.Kapasite,
            oda.Durum,
            oda.OdaTipi,
            CokluSecim.Liste(oda.Ozellikler),
            oda.Notlar,
            yataklar.Count,
            yataklar.Count(y => y.DoluMu || y.SakinId != null),
            yataklar.Select(y => new YatakOzetDto(
                y.Id,
                y.YatakNumarasi,
                y.DoluMu,
                string.IsNullOrWhiteSpace(y.YatakTipi) ? "Standart" : y.YatakTipi,
                CokluSecim.Liste(y.Ozellikler),
                string.IsNullOrWhiteSpace(y.Durum) ? "Aktif" : y.Durum,
                y.Sakin == null
                    ? null
                    : new SakinOzetDto(y.Sakin.Id, $"{y.Sakin.Ad} {y.Sakin.Soyad}", y.Sakin.TcKimlikNo)))
            .ToList());
    }
}
