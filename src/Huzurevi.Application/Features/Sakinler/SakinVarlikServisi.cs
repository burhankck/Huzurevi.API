using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Sakinler;

public interface ISakinMalServisi
{
    Task<List<SakinMalDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default);
    Task<SakinMalDto> OlusturAsync(int sakinId, SakinMalIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int sakinId, int id, SakinMalIstek istek, CancellationToken ct = default);
    Task SilAsync(int sakinId, int id, CancellationToken ct = default);
}

public interface ISakinGelirServisi
{
    Task<List<SakinGelirDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default);
    Task<SakinGelirDto> OlusturAsync(int sakinId, SakinGelirIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int sakinId, int id, SakinGelirIstek istek, CancellationToken ct = default);
    Task SilAsync(int sakinId, int id, CancellationToken ct = default);
}

public interface ISakinSosyalGuvenceServisi
{
    Task<List<SakinSosyalGuvenceDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default);
    Task<SakinSosyalGuvenceDto> OlusturAsync(int sakinId, SakinSosyalGuvenceIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int sakinId, int id, SakinSosyalGuvenceIstek istek, CancellationToken ct = default);
    Task SilAsync(int sakinId, int id, CancellationToken ct = default);
}

public class SakinMalServisi : ISakinMalServisi
{
    private readonly IUygulamaDbContext _db;
    public SakinMalServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<SakinMalDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayitlar = await _db.SakinMallari.AsNoTracking().Where(x => x.SakinId == sakinId)
            .OrderByDescending(x => x.AktifMi).ThenBy(x => x.MalTuru).ToListAsync(ct);
        return kayitlar.Select(x => new SakinMalDto(x.Id, x.SakinId, x.MalTuru, x.Adet, x.Deger, x.Adres, x.Aciklama, x.AktifMi)).ToList();
    }

    public async Task<SakinMalDto> OlusturAsync(int sakinId, SakinMalIstek istek, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayit = new SakinMal { SakinId = sakinId, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.SakinMallari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return new SakinMalDto(kayit.Id, kayit.SakinId, kayit.MalTuru, kayit.Adet, kayit.Deger, kayit.Adres, kayit.Aciklama, kayit.AktifMi);
    }

    public async Task GuncelleAsync(int sakinId, int id, SakinMalIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.SakinMallari.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Mal kaydı bulunamadı.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int sakinId, int id, CancellationToken ct = default)
    {
        var kayit = await _db.SakinMallari.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Mal kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private static void Doldur(SakinMal kayit, SakinMalIstek istek)
    {
        kayit.MalTuru = istek.MalTuru.Trim();
        kayit.Adet = istek.Adet;
        kayit.Deger = istek.Deger;
        kayit.Adres = string.IsNullOrWhiteSpace(istek.Adres) ? null : istek.Adres.Trim();
        kayit.Aciklama = string.IsNullOrWhiteSpace(istek.Aciklama) ? null : istek.Aciklama.Trim();
        kayit.AktifMi = istek.AktifMi;
    }
}

public class SakinGelirServisi : ISakinGelirServisi
{
    private readonly IUygulamaDbContext _db;
    public SakinGelirServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<SakinGelirDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayitlar = await _db.SakinGelirleri.AsNoTracking().Where(x => x.SakinId == sakinId)
            .OrderByDescending(x => x.AktifMi).ThenBy(x => x.GelirTuru).ToListAsync(ct);
        return kayitlar.Select(x => new SakinGelirDto(x.Id, x.SakinId, x.GelirTuru, x.Periyot, x.Deger, x.Aciklama, x.AktifMi)).ToList();
    }

    public async Task<SakinGelirDto> OlusturAsync(int sakinId, SakinGelirIstek istek, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayit = new SakinGelir { SakinId = sakinId, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.SakinGelirleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return new SakinGelirDto(kayit.Id, kayit.SakinId, kayit.GelirTuru, kayit.Periyot, kayit.Deger, kayit.Aciklama, kayit.AktifMi);
    }

    public async Task GuncelleAsync(int sakinId, int id, SakinGelirIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.SakinGelirleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Gelir kaydı bulunamadı.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int sakinId, int id, CancellationToken ct = default)
    {
        var kayit = await _db.SakinGelirleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Gelir kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private static void Doldur(SakinGelir kayit, SakinGelirIstek istek)
    {
        kayit.GelirTuru = istek.GelirTuru.Trim();
        kayit.Periyot = istek.Periyot.Trim();
        kayit.Deger = istek.Deger;
        kayit.Aciklama = string.IsNullOrWhiteSpace(istek.Aciklama) ? null : istek.Aciklama.Trim();
        kayit.AktifMi = istek.AktifMi;
    }
}

public class SakinSosyalGuvenceServisi : ISakinSosyalGuvenceServisi
{
    private readonly IUygulamaDbContext _db;
    public SakinSosyalGuvenceServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<SakinSosyalGuvenceDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayitlar = await _db.SakinSosyalGuvenceleri.AsNoTracking().Where(x => x.SakinId == sakinId)
            .OrderByDescending(x => x.AktifMi).ThenBy(x => x.GuvenceTuru).ToListAsync(ct);
        return kayitlar.Select(x => new SakinSosyalGuvenceDto(x.Id, x.SakinId, x.GuvenceTuru, x.BaslangicTarihi, x.BitisTarihi, x.Aciklama, x.AktifMi)).ToList();
    }

    public async Task<SakinSosyalGuvenceDto> OlusturAsync(int sakinId, SakinSosyalGuvenceIstek istek, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayit = new SakinSosyalGuvence { SakinId = sakinId, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.SakinSosyalGuvenceleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return new SakinSosyalGuvenceDto(kayit.Id, kayit.SakinId, kayit.GuvenceTuru, kayit.BaslangicTarihi, kayit.BitisTarihi, kayit.Aciklama, kayit.AktifMi);
    }

    public async Task GuncelleAsync(int sakinId, int id, SakinSosyalGuvenceIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.SakinSosyalGuvenceleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Sosyal güvence kaydı bulunamadı.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int sakinId, int id, CancellationToken ct = default)
    {
        var kayit = await _db.SakinSosyalGuvenceleri.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Sosyal güvence kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private static void Doldur(SakinSosyalGuvence kayit, SakinSosyalGuvenceIstek istek)
    {
        kayit.GuvenceTuru = istek.GuvenceTuru.Trim();
        kayit.BaslangicTarihi = SakinVarlikYardimcisi.ToUtc(istek.BaslangicTarihi);
        kayit.BitisTarihi = SakinVarlikYardimcisi.ToUtc(istek.BitisTarihi);
        kayit.Aciklama = string.IsNullOrWhiteSpace(istek.Aciklama) ? null : istek.Aciklama.Trim();
        kayit.AktifMi = istek.AktifMi;
    }
}

internal static class SakinVarlikYardimcisi
{
    public static async Task SakiniDogrulaAsync(IUygulamaDbContext db, int sakinId, CancellationToken ct)
    {
        if (!await db.Sakinler.AnyAsync(s => s.Id == sakinId, ct))
        {
            throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        }
    }

    public static DateTime? ToUtc(DateTime? value)
    {
        if (value is null) return null;
        return value.Value.Kind switch
        {
            DateTimeKind.Utc => value.Value,
            DateTimeKind.Local => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
        };
    }

    public static DateTime ToUtc(DateTime value) => ToUtc((DateTime?)value)!.Value;
}
