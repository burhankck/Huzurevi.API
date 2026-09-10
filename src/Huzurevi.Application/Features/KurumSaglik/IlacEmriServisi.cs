using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.KurumSaglik;

public interface IIlacEmriServisi
{
    Task<List<IlacEmriDto>> EmirleriGetirAsync(int? sakinId, bool? aktifMi, CancellationToken ct = default);
    Task<IlacEmriDto> EmirOlusturAsync(IlacEmriIstek istek, CancellationToken ct = default);
    Task EmirGuncelleAsync(int id, IlacEmriIstek istek, CancellationToken ct = default);
    Task EmirSilAsync(int id, CancellationToken ct = default);
    Task<List<IlacUygulamaDto>> UygulamalariGetirAsync(DateTime? tarih, int? sakinId, CancellationToken ct = default);
    Task<IlacUygulamaDto> UygulamaOlusturAsync(IlacUygulamaIstek istek, CancellationToken ct = default);
    Task UygulamaGuncelleAsync(int id, IlacUygulamaIstek istek, CancellationToken ct = default);
    Task UygulamaSilAsync(int id, CancellationToken ct = default);
    Task<byte[]> GunlukPdfUretAsync(DateTime tarih, CancellationToken ct = default);
}

public class IlacEmriServisi : IIlacEmriServisi
{
    private readonly IUygulamaDbContext _db;
    private readonly IGunlukIlacPdfUretici _pdf;

    public IlacEmriServisi(IUygulamaDbContext db, IGunlukIlacPdfUretici pdf)
    {
        _db = db;
        _pdf = pdf;
    }

    public async Task<List<IlacEmriDto>> EmirleriGetirAsync(int? sakinId, bool? aktifMi, CancellationToken ct = default)
    {
        var sorgu = _db.IlacEmirleri.AsNoTracking().Include(x => x.Sakin).AsQueryable();
        if (sakinId is not null) sorgu = sorgu.Where(x => x.SakinId == sakinId);
        if (aktifMi is not null) sorgu = sorgu.Where(x => x.AktifMi == aktifMi);
        var kayitlar = await sorgu.OrderByDescending(x => x.AktifMi).ThenBy(x => x.IlacAdi).ToListAsync(ct);
        return kayitlar.Select(MapEmir).ToList();
    }

    public async Task<IlacEmriDto> EmirOlusturAsync(IlacEmriIstek istek, CancellationToken ct = default)
    {
        var sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == istek.SakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        var emir = new IlacEmri { SakinId = sakin.Id, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(emir, istek, ilkKayitKilitli: false);
        _db.IlacEmirleri.Add(emir);
        await _db.SaveChangesAsync(ct);
        emir.Sakin = sakin;
        return MapEmir(emir);
    }

    public async Task EmirGuncelleAsync(int id, IlacEmriIstek istek, CancellationToken ct = default)
    {
        var emir = await _db.IlacEmirleri.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("İlaç emri bulunamadı.");
        Doldur(emir, istek, ilkKayitKilitli: emir.KayitTuru == "IlkKayit");
        emir.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task EmirSilAsync(int id, CancellationToken ct = default)
    {
        var emir = await _db.IlacEmirleri.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("İlaç emri bulunamadı.");
        emir.SilindiMi = true;
        emir.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<IlacUygulamaDto>> UygulamalariGetirAsync(DateTime? tarih, int? sakinId, CancellationToken ct = default)
    {
        var sorgu = _db.IlacUygulamalari.AsNoTracking().Include(x => x.Sakin).Include(x => x.Emir).AsQueryable();
        if (sakinId is not null) sorgu = sorgu.Where(x => x.SakinId == sakinId);
        if (tarih is not null)
        {
            var gun = SakinVarlikYardimcisi.ToUtc(tarih.Value).Date;
            sorgu = sorgu.Where(x => x.Tarih >= gun && x.Tarih < gun.AddDays(1));
        }
        var kayitlar = await sorgu.OrderBy(x => x.Tarih).ToListAsync(ct);
        return kayitlar.Select(MapUygulama).ToList();
    }

    public async Task<IlacUygulamaDto> UygulamaOlusturAsync(IlacUygulamaIstek istek, CancellationToken ct = default)
    {
        var emir = await _db.IlacEmirleri.Include(x => x.Sakin).FirstOrDefaultAsync(x => x.Id == istek.IlacEmriId, ct)
            ?? throw new KayitBulunamadiHatasi("İlaç emri bulunamadı.");
        var kayit = new IlacUygulama
        {
            IlacEmriId = emir.Id,
            SakinId = emir.SakinId,
            Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih),
            Durum = istek.Durum,
            Personel = string.IsNullOrWhiteSpace(istek.Personel) ? null : istek.Personel.Trim(),
            Notlar = string.IsNullOrWhiteSpace(istek.Notlar) ? null : istek.Notlar.Trim(),
            OlusturulmaTarihi = DateTime.UtcNow
        };
        _db.IlacUygulamalari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Emir = emir;
        kayit.Sakin = emir.Sakin;
        return MapUygulama(kayit);
    }

    public async Task UygulamaGuncelleAsync(int id, IlacUygulamaIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.IlacUygulamalari.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Uygulama kaydı bulunamadı.");
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.Durum = istek.Durum;
        kayit.Personel = string.IsNullOrWhiteSpace(istek.Personel) ? null : istek.Personel.Trim();
        kayit.Notlar = string.IsNullOrWhiteSpace(istek.Notlar) ? null : istek.Notlar.Trim();
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task UygulamaSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.IlacUygulamalari.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Uygulama kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<byte[]> GunlukPdfUretAsync(DateTime tarih, CancellationToken ct = default)
    {
        var kayitlar = await UygulamalariGetirAsync(tarih, null, ct);
        return _pdf.Uret(tarih, kayitlar);
    }

    private static void Doldur(IlacEmri emir, IlacEmriIstek istek, bool ilkKayitKilitli)
    {
        if (ilkKayitKilitli)
        {
            emir.BitisTarihi = SakinVarlikYardimcisi.ToUtc(istek.BitisTarihi);
            emir.AktifMi = istek.AktifMi;
            return;
        }

        emir.SakinId = istek.SakinId;
        emir.IlacAdi = istek.IlacAdi.Trim();
        emir.Doz = string.IsNullOrWhiteSpace(istek.Doz) ? null : istek.Doz.Trim();
        emir.Birim = string.IsNullOrWhiteSpace(istek.Birim) ? null : istek.Birim.Trim();
        emir.KullanimSikligi = string.IsNullOrWhiteSpace(istek.KullanimSikligi) ? null : istek.KullanimSikligi.Trim();
        emir.Zamanlama = string.IsNullOrWhiteSpace(istek.Zamanlama) ? null : istek.Zamanlama.Trim();
        emir.BaslangicTarihi = SakinVarlikYardimcisi.ToUtc(istek.BaslangicTarihi);
        emir.BitisTarihi = SakinVarlikYardimcisi.ToUtc(istek.BitisTarihi);
        emir.AktifMi = istek.AktifMi;
        emir.KayitTuru = istek.KayitTuru;
    }

    private static IlacEmriDto MapEmir(IlacEmri x) => new(
        x.Id, x.SakinId, x.Sakin is null ? "" : $"{x.Sakin.Ad} {x.Sakin.Soyad}", x.IlacAdi, x.Doz, x.Birim,
        x.KullanimSikligi, x.Zamanlama, x.BaslangicTarihi, x.BitisTarihi, x.AktifMi, x.KayitTuru);

    private static IlacUygulamaDto MapUygulama(IlacUygulama x) => new(
        x.Id, x.IlacEmriId, x.SakinId, x.Sakin is null ? "" : $"{x.Sakin.Ad} {x.Sakin.Soyad}",
        x.Emir?.IlacAdi ?? "", x.Tarih, x.Durum, x.Personel, x.Notlar);
}
