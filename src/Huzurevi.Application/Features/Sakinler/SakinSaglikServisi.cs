using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Sakinler;

public interface ISakinOlcumServisi
{
    Task<List<SakinOlcumDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default);
    Task<SakinOlcumDto> OlusturAsync(int sakinId, SakinOlcumIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int sakinId, int id, SakinOlcumIstek istek, CancellationToken ct = default);
    Task SilAsync(int sakinId, int id, CancellationToken ct = default);
}

public interface ISakinSaglikDegerlendirmeServisi
{
    Task<List<SakinSaglikDegerlendirmeDto>> TumunuGetirAsync(int sakinId, string tur, CancellationToken ct = default);
    Task<SakinSaglikDegerlendirmeDto> OlusturAsync(int sakinId, SakinSaglikDegerlendirmeIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int sakinId, int id, SakinSaglikDegerlendirmeIstek istek, CancellationToken ct = default);
    Task SilAsync(int sakinId, int id, CancellationToken ct = default);
}

public class SakinOlcumServisi : ISakinOlcumServisi
{
    private readonly IUygulamaDbContext _db;
    public SakinOlcumServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<SakinOlcumDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayitlar = await _db.SakinOlcumleri.AsNoTracking().Where(x => x.SakinId == sakinId)
            .OrderByDescending(x => x.Tarih).ThenByDescending(x => x.Id).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<SakinOlcumDto> OlusturAsync(int sakinId, SakinOlcumIstek istek, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayit = new SakinOlcum { SakinId = sakinId, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.SakinOlcumleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return Map(kayit);
    }

    public async Task GuncelleAsync(int sakinId, int id, SakinOlcumIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.SakinOlcumleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Ölçüm kaydı bulunamadı.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int sakinId, int id, CancellationToken ct = default)
    {
        var kayit = await _db.SakinOlcumleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Ölçüm kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private static void Doldur(SakinOlcum kayit, SakinOlcumIstek istek)
    {
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.BoyCm = istek.BoyCm;
        kayit.KiloKg = istek.KiloKg;
        kayit.KanGrubu = string.IsNullOrWhiteSpace(istek.KanGrubu) ? null : istek.KanGrubu.Trim();
        kayit.Aciklama = string.IsNullOrWhiteSpace(istek.Aciklama) ? null : istek.Aciklama.Trim();
        kayit.AktifMi = istek.AktifMi;
    }

    private static SakinOlcumDto Map(SakinOlcum x) =>
        new(x.Id, x.SakinId, x.Tarih, x.BoyCm, x.KiloKg, x.KanGrubu, x.Aciklama, x.AktifMi);
}

public class SakinSaglikDegerlendirmeServisi : ISakinSaglikDegerlendirmeServisi
{
    private readonly IUygulamaDbContext _db;
    public SakinSaglikDegerlendirmeServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<SakinSaglikDegerlendirmeDto>> TumunuGetirAsync(int sakinId, string tur, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        if (!SakinSaglikKurallari.Turler.Contains(tur))
        {
            throw new GecersizIstekHatasi("Geçersiz değerlendirme türü.");
        }

        var kayitlar = await _db.SakinSaglikDegerlendirmeleri.AsNoTracking()
            .Where(x => x.SakinId == sakinId && x.Tur == tur)
            .OrderByDescending(x => x.Tarih).ThenByDescending(x => x.Id).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<SakinSaglikDegerlendirmeDto> OlusturAsync(int sakinId, SakinSaglikDegerlendirmeIstek istek, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayit = new SakinSaglikDegerlendirme { SakinId = sakinId, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.SakinSaglikDegerlendirmeleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return Map(kayit);
    }

    public async Task GuncelleAsync(int sakinId, int id, SakinSaglikDegerlendirmeIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.SakinSaglikDegerlendirmeleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Değerlendirme kaydı bulunamadı.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int sakinId, int id, CancellationToken ct = default)
    {
        var kayit = await _db.SakinSaglikDegerlendirmeleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Değerlendirme kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private static void Doldur(SakinSaglikDegerlendirme kayit, SakinSaglikDegerlendirmeIstek istek)
    {
        kayit.Tur = istek.Tur.Trim();
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.Durum = istek.Durum.Trim();
        kayit.Seviye = string.IsNullOrWhiteSpace(istek.Seviye) ? null : istek.Seviye.Trim();
        kayit.Taraf = string.IsNullOrWhiteSpace(istek.Taraf) ? null : istek.Taraf.Trim();
        kayit.YardimciAracMi = istek.YardimciAracMi;
        kayit.Aciklama = string.IsNullOrWhiteSpace(istek.Aciklama) ? null : istek.Aciklama.Trim();
        kayit.AktifMi = istek.AktifMi;
    }

    private static SakinSaglikDegerlendirmeDto Map(SakinSaglikDegerlendirme x) =>
        new(x.Id, x.SakinId, x.Tur, x.Tarih, x.Durum, x.Seviye, x.Taraf, x.YardimciAracMi, x.Aciklama, x.AktifMi);
}
