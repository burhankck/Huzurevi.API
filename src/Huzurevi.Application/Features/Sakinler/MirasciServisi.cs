using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Sakinler;

public interface IMirasciServisi
{
    Task<List<MirasciDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default);
    Task<MirasciDto> OlusturAsync(int sakinId, MirasciOlusturIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int sakinId, int mirasciId, MirasciGuncelleIstek istek, CancellationToken ct = default);
    Task SilAsync(int sakinId, int mirasciId, CancellationToken ct = default);
}

public class MirasciServisi : IMirasciServisi
{
    private readonly IUygulamaDbContext _db;

    public MirasciServisi(IUygulamaDbContext db)
    {
        _db = db;
    }

    public async Task<List<MirasciDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default)
    {
        await SakiniDogrulaAsync(sakinId, ct);

        var kayitlar = await _db.Mirascilar
            .AsNoTracking()
            .Where(m => m.SakinId == sakinId)
            .OrderBy(m => m.Ad)
            .ThenBy(m => m.Soyad)
            .ToListAsync(ct);

        return kayitlar.Select(Map).ToList();
    }

    public async Task<MirasciDto> OlusturAsync(int sakinId, MirasciOlusturIstek istek, CancellationToken ct = default)
    {
        await SakiniDogrulaAsync(sakinId, ct);

        var kayit = new Mirasci
        {
            SakinId = sakinId,
            OlusturulmaTarihi = DateTime.UtcNow
        };
        Doldur(kayit, istek.Ad, istek.Soyad, istek.TcKimlikNo, istek.Telefon, istek.Yakinlik, istek.Adres);
        _db.Mirascilar.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return Map(kayit);
    }

    public async Task GuncelleAsync(int sakinId, int mirasciId, MirasciGuncelleIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.Mirascilar.FirstOrDefaultAsync(m => m.Id == mirasciId && m.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Mirasçı kaydı bulunamadı.");

        Doldur(kayit, istek.Ad, istek.Soyad, istek.TcKimlikNo, istek.Telefon, istek.Yakinlik, istek.Adres);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int sakinId, int mirasciId, CancellationToken ct = default)
    {
        var kayit = await _db.Mirascilar.FirstOrDefaultAsync(m => m.Id == mirasciId && m.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Mirasçı kaydı bulunamadı.");

        var simdi = DateTime.UtcNow;
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = simdi;
        kayit.GuncellenmeTarihi = simdi;
        await _db.SaveChangesAsync(ct);
    }

    private async Task SakiniDogrulaAsync(int sakinId, CancellationToken ct)
    {
        if (!await _db.Sakinler.AnyAsync(s => s.Id == sakinId, ct))
        {
            throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        }
    }

    private static void Doldur(Mirasci kayit, string ad, string soyad, string? tc, string? telefon, string yakinlik, string? adres)
    {
        kayit.Ad = ad.Trim();
        kayit.Soyad = soyad.Trim();
        kayit.TcKimlikNo = string.IsNullOrWhiteSpace(tc) ? null : tc.Trim();
        kayit.Telefon = string.IsNullOrWhiteSpace(telefon) ? null : telefon.Trim();
        kayit.Yakinlik = yakinlik.Trim();
        kayit.Adres = string.IsNullOrWhiteSpace(adres) ? null : adres.Trim();
    }

    private static MirasciDto Map(Mirasci m) =>
        new(m.Id, m.SakinId, m.Ad, m.Soyad, m.TcKimlikNo, m.Telefon, m.Yakinlik, m.Adres);
}
