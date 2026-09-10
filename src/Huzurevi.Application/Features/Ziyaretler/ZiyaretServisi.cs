using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Ziyaretler;

public class ZiyaretServisi : IZiyaretServisi
{
    private readonly IUygulamaDbContext _db;

    public ZiyaretServisi(IUygulamaDbContext db)
    {
        _db = db;
    }

    public async Task<List<ZiyaretDto>> TumunuGetirAsync(int? sakinId, bool? iceride, CancellationToken ct = default)
    {
        var sorgu = _db.Ziyaretler.AsNoTracking().Include(z => z.Sakin).AsQueryable();

        if (sakinId is not null)
        {
            sorgu = sorgu.Where(z => z.SakinId == sakinId);
        }

        if (iceride == true)
        {
            sorgu = sorgu.Where(z => z.CikisTarihi == null);
        }
        else if (iceride == false)
        {
            sorgu = sorgu.Where(z => z.CikisTarihi != null);
        }

        var kayitlar = await sorgu
            .OrderByDescending(z => z.GirisTarihi)
            .Take(200)
            .ToListAsync(ct);

        return kayitlar.Select(Map).ToList();
    }

    public Task<List<ZiyaretDto>> IceridekileriGetirAsync(CancellationToken ct = default) =>
        TumunuGetirAsync(null, true, ct);

    public async Task<ZiyaretDto> GirisYapAsync(ZiyaretGirisIstek istek, CancellationToken ct = default)
    {
        var sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == istek.SakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Ziyaret edilecek sakin bulunamadı.");

        if (sakin.Durum != "Aktif")
        {
            throw new GecersizIstekHatasi("Yalnızca aktif sakinler ziyaret edilebilir.");
        }

        var tc = string.IsNullOrWhiteSpace(istek.TcKimlikNo) ? null : istek.TcKimlikNo.Trim();
        if (tc is not null)
        {
            var ayniKisiIcerideMi = await _db.Ziyaretler.AnyAsync(
                z => z.CikisTarihi == null && z.TcKimlikNo == tc,
                ct);

            if (ayniKisiIcerideMi)
            {
                throw new CakismaHatasi("Bu T.C. kimlik numarası ile henüz çıkış yapmamış bir ziyaret var.");
            }
        }

        var ziyaret = new Ziyaret
        {
            SakinId = sakin.Id,
            ZiyaretciAd = istek.ZiyaretciAd.Trim(),
            ZiyaretciSoyad = istek.ZiyaretciSoyad.Trim(),
            TcKimlikNo = tc,
            Telefon = string.IsNullOrWhiteSpace(istek.Telefon) ? null : istek.Telefon.Trim(),
            Yakinlik = string.IsNullOrWhiteSpace(istek.Yakinlik) ? null : istek.Yakinlik.Trim(),
            Notlar = string.IsNullOrWhiteSpace(istek.Notlar) ? null : istek.Notlar.Trim(),
            GirisTarihi = DateTime.UtcNow,
            OlusturulmaTarihi = DateTime.UtcNow
        };

        _db.Ziyaretler.Add(ziyaret);
        await _db.SaveChangesAsync(ct);

        ziyaret.Sakin = sakin;
        return Map(ziyaret);
    }

    public async Task CikisYapAsync(int id, CancellationToken ct = default)
    {
        var ziyaret = await _db.Ziyaretler.FirstOrDefaultAsync(z => z.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Ziyaret kaydı bulunamadı.");

        if (ziyaret.CikisTarihi is not null)
        {
            throw new GecersizIstekHatasi("Bu ziyaret için çıkış zaten alınmış.");
        }

        ziyaret.CikisTarihi = DateTime.UtcNow;
        ziyaret.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private static ZiyaretDto Map(Ziyaret z) => new(
        z.Id,
        z.SakinId,
        z.Sakin is null ? string.Empty : $"{z.Sakin.Ad} {z.Sakin.Soyad}",
        z.ZiyaretciAd,
        z.ZiyaretciSoyad,
        z.TcKimlikNo,
        z.Telefon,
        z.Yakinlik,
        z.GirisTarihi,
        z.CikisTarihi,
        z.CikisTarihi is null,
        z.Notlar);
}
