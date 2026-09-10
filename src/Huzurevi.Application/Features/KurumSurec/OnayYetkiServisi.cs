using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.KurumSurec;

public interface IOnayYetkiServisi
{
    Task<List<OnayYetkisiDto>> GetirAsync(CancellationToken ct = default);
    Task<OnayYetkisiDto> OlusturAsync(OnayYetkisiIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, OnayYetkisiIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task YetkiDogrulaAsync(int kullaniciId, bool yoneticiMi, string alan, CancellationToken ct = default);
}

public class OnayYetkiServisi : IOnayYetkiServisi
{
    private readonly IUygulamaDbContext _db;
    public OnayYetkiServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<OnayYetkisiDto>> GetirAsync(CancellationToken ct = default)
    {
        var kayitlar = await _db.OnayYetkileri.AsNoTracking().Include(x => x.Kullanici).OrderBy(x => x.Alan).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<OnayYetkisiDto> OlusturAsync(OnayYetkisiIstek istek, CancellationToken ct = default)
    {
        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(x => x.Id == istek.KullaniciId, ct)
            ?? throw new KayitBulunamadiHatasi("Kullanıcı bulunamadı.");
        var ayni = await _db.OnayYetkileri.AnyAsync(x => x.KullaniciId == istek.KullaniciId && x.Alan == istek.Alan, ct);
        if (ayni) throw new GecersizIstekHatasi("Bu kullanıcı için bu alan zaten tanımlı.");
        var kayit = new OnayYetkisi
        {
            KullaniciId = kullanici.Id,
            Alan = istek.Alan,
            AktifMi = istek.AktifMi,
            OlusturulmaTarihi = DateTime.UtcNow
        };
        _db.OnayYetkileri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Kullanici = kullanici;
        return Map(kayit);
    }

    public async Task GuncelleAsync(int id, OnayYetkisiIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.OnayYetkileri.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Yetki kaydı bulunamadı.");
        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(x => x.Id == istek.KullaniciId, ct)
            ?? throw new KayitBulunamadiHatasi("Kullanıcı bulunamadı.");
        kayit.KullaniciId = kullanici.Id;
        kayit.Alan = istek.Alan;
        kayit.AktifMi = istek.AktifMi;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.OnayYetkileri.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Yetki kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task YetkiDogrulaAsync(int kullaniciId, bool yoneticiMi, string alan, CancellationToken ct = default)
    {
        if (yoneticiMi) return;
        var varMi = await _db.OnayYetkileri.AnyAsync(x => x.KullaniciId == kullaniciId && x.Alan == alan && x.AktifMi, ct);
        if (!varMi) throw new GecersizIstekHatasi("Bu süreç için onay yetkiniz yok.");
    }

    private static OnayYetkisiDto Map(OnayYetkisi x) => new(
        x.Id, x.KullaniciId, x.Kullanici is null ? "" : $"{x.Kullanici.Ad} {x.Kullanici.Soyad}", x.Alan, x.AktifMi);
}

internal static class OnayUygulayici
{
    public static void Uygula(
        Action<string> durumYaz,
        Action<string?> onaylayanYaz,
        Action<DateTime?> tarihYaz,
        Action<string?> notYaz,
        OnayIstek istek,
        string kim)
    {
        durumYaz(istek.OnaylandiMi ? OnayDurumlari.Onaylandi : OnayDurumlari.Reddedildi);
        onaylayanYaz(kim);
        tarihYaz(DateTime.UtcNow);
        notYaz(string.IsNullOrWhiteSpace(istek.Not) ? null : istek.Not.Trim());
    }
}
