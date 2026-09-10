using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Sakinler;

public interface ISakinGunlukIzinServisi
{
    Task<List<SakinGunlukIzinDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default);
    Task<SakinGunlukIzinDto> OlusturAsync(int sakinId, SakinGunlukIzinIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int sakinId, int id, SakinGunlukIzinIstek istek, CancellationToken ct = default);
    Task SilAsync(int sakinId, int id, CancellationToken ct = default);
}

public interface ISakinEmanetServisi
{
    Task<List<SakinEmanetDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default);
    Task<SakinEmanetDto> OlusturAsync(int sakinId, SakinEmanetIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int sakinId, int id, SakinEmanetIstek istek, CancellationToken ct = default);
    Task SilAsync(int sakinId, int id, CancellationToken ct = default);
}

public class SakinGunlukIzinServisi : ISakinGunlukIzinServisi
{
    private readonly IUygulamaDbContext _db;
    public SakinGunlukIzinServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<SakinGunlukIzinDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayitlar = await _db.SakinGunlukIzinleri.AsNoTracking().Where(x => x.SakinId == sakinId)
            .OrderByDescending(x => x.Tarih).ThenBy(x => x.CikisSaati).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<SakinGunlukIzinDto> OlusturAsync(int sakinId, SakinGunlukIzinIstek istek, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayit = new SakinGunlukIzin { SakinId = sakinId, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.SakinGunlukIzinleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return Map(kayit);
    }

    public async Task GuncelleAsync(int sakinId, int id, SakinGunlukIzinIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.SakinGunlukIzinleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Günlük izin kaydı bulunamadı.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int sakinId, int id, CancellationToken ct = default)
    {
        var kayit = await _db.SakinGunlukIzinleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Günlük izin kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private static void Doldur(SakinGunlukIzin kayit, SakinGunlukIzinIstek istek)
    {
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.Yer = istek.Yer.Trim();
        kayit.CikisSaati = SakinIzinEmanetKurallari.SaatNormalize(istek.CikisSaati);
        kayit.DonusSaati = SakinIzinEmanetKurallari.SaatNormalize(istek.DonusSaati);
        kayit.AktifMi = istek.AktifMi;
    }

    private static SakinGunlukIzinDto Map(SakinGunlukIzin x) =>
        new(x.Id, x.SakinId, x.Tarih, x.Yer, x.CikisSaati, x.DonusSaati, x.AktifMi);
}

public class SakinEmanetServisi : ISakinEmanetServisi
{
    private readonly IUygulamaDbContext _db;
    public SakinEmanetServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<SakinEmanetDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayitlar = await _db.SakinEmanetleri.AsNoTracking().Where(x => x.SakinId == sakinId)
            .OrderByDescending(x => x.Tarih).ThenByDescending(x => x.Saat).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<SakinEmanetDto> OlusturAsync(int sakinId, SakinEmanetIstek istek, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayit = new SakinEmanet { SakinId = sakinId, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.SakinEmanetleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return Map(kayit);
    }

    public async Task GuncelleAsync(int sakinId, int id, SakinEmanetIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.SakinEmanetleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Emanet kaydı bulunamadı.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int sakinId, int id, CancellationToken ct = default)
    {
        var kayit = await _db.SakinEmanetleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Emanet kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private static void Doldur(SakinEmanet kayit, SakinEmanetIstek istek)
    {
        kayit.IslemTuru = istek.IslemTuru.Trim();
        kayit.EmanetTuru = istek.EmanetTuru.Trim();
        kayit.Durum = istek.Durum.Trim();
        kayit.Adet = istek.Adet;
        kayit.Deger = istek.Deger;
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.Saat = SakinIzinEmanetKurallari.SaatNormalize(istek.Saat);
        kayit.TeslimEden = istek.TeslimEden.Trim();
        kayit.TeslimAlan = istek.TeslimAlan.Trim();
        kayit.Aciklama = string.IsNullOrWhiteSpace(istek.Aciklama) ? null : istek.Aciklama.Trim();
    }

    private static SakinEmanetDto Map(SakinEmanet x) =>
        new(x.Id, x.SakinId, x.IslemTuru, x.EmanetTuru, x.Durum, x.Adet, x.Deger, x.Tarih, x.Saat, x.TeslimEden, x.TeslimAlan, x.Aciklama);
}
