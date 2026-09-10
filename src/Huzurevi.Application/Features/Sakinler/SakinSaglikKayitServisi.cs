using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Sakinler;

public interface ISakinSaglikKayitServisi
{
    Task<List<SakinSaglikKayitDto>> TumunuGetirAsync(int sakinId, string tur, CancellationToken ct = default);
    Task<SakinSaglikKayitDto> OlusturAsync(int sakinId, SakinSaglikKayitIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int sakinId, int id, SakinSaglikKayitIstek istek, CancellationToken ct = default);
    Task SilAsync(int sakinId, int id, CancellationToken ct = default);
}

public class SakinSaglikKayitServisi : ISakinSaglikKayitServisi
{
    private readonly IUygulamaDbContext _db;
    public SakinSaglikKayitServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<SakinSaglikKayitDto>> TumunuGetirAsync(int sakinId, string tur, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        if (!SakinSaglikKurallari.KayitTurleri.Contains(tur))
        {
            throw new GecersizIstekHatasi("Geçersiz sağlık kayıt türü.");
        }

        var kayitlar = await _db.SakinSaglikKayitlari.AsNoTracking()
            .Where(x => x.SakinId == sakinId && x.Tur == tur)
            .OrderByDescending(x => x.Tarih).ThenByDescending(x => x.Id).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<SakinSaglikKayitDto> OlusturAsync(int sakinId, SakinSaglikKayitIstek istek, CancellationToken ct = default)
    {
        await SakinVarlikYardimcisi.SakiniDogrulaAsync(_db, sakinId, ct);
        var kayit = new SakinSaglikKayit { SakinId = sakinId, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.SakinSaglikKayitlari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return Map(kayit);
    }

    public async Task GuncelleAsync(int sakinId, int id, SakinSaglikKayitIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.SakinSaglikKayitlari.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Sağlık kaydı bulunamadı.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int sakinId, int id, CancellationToken ct = default)
    {
        var kayit = await _db.SakinSaglikKayitlari.FirstOrDefaultAsync(x => x.Id == id && x.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Sağlık kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private static void Doldur(SakinSaglikKayit kayit, SakinSaglikKayitIstek istek)
    {
        kayit.Tur = istek.Tur.Trim();
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.Ad = istek.Ad.Trim();
        kayit.DurumTipi = Metin(istek.DurumTipi);
        kayit.BaslangicTarihi = SakinVarlikYardimcisi.ToUtc(istek.BaslangicTarihi);
        kayit.BitisTarihi = SakinVarlikYardimcisi.ToUtc(istek.BitisTarihi);
        kayit.AmeliyatTarihi = SakinVarlikYardimcisi.ToUtc(istek.AmeliyatTarihi);
        kayit.Hastane = Metin(istek.Hastane);
        kayit.Komplikasyon = Metin(istek.Komplikasyon);
        kayit.MarkaModel = Metin(istek.MarkaModel);
        kayit.SeriNo = Metin(istek.SeriNo);
        kayit.TeminTarihi = SakinVarlikYardimcisi.ToUtc(istek.TeminTarihi);
        kayit.KontrolTarihi = SakinVarlikYardimcisi.ToUtc(istek.KontrolTarihi);
        kayit.Taraf = Metin(istek.Taraf);
        kayit.Derece = Metin(istek.Derece);
        kayit.Bolge = Metin(istek.Bolge);
        kayit.Teshis = Metin(istek.Teshis);
        kayit.Doz = Metin(istek.Doz);
        kayit.KullanimSikligi = Metin(istek.KullanimSikligi);
        kayit.UygulamaYolu = Metin(istek.UygulamaYolu);
        kayit.ReceteliMi = istek.ReceteliMi;
        kayit.ReceteNo = Metin(istek.ReceteNo);
        kayit.ZamanlamaTipi = Metin(istek.ZamanlamaTipi);
        kayit.ZamanDilimleri = Metin(istek.ZamanDilimleri);
        kayit.ReaksiyonTipi = Metin(istek.ReaksiyonTipi);
        kayit.Aciklama = Metin(istek.Aciklama);
        kayit.AktifMi = istek.AktifMi;
    }

    private static string? Metin(string? deger) => string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();

    private static SakinSaglikKayitDto Map(SakinSaglikKayit x) =>
        new(x.Id, x.SakinId, x.Tur, x.Tarih, x.Ad, x.DurumTipi, x.BaslangicTarihi, x.BitisTarihi, x.AmeliyatTarihi,
            x.Hastane, x.Komplikasyon, x.MarkaModel, x.SeriNo, x.TeminTarihi, x.KontrolTarihi, x.Taraf, x.Derece,
            x.Bolge, x.Teshis, x.Doz, x.KullanimSikligi, x.UygulamaYolu, x.ReceteliMi, x.ReceteNo, x.ZamanlamaTipi,
            x.ZamanDilimleri, x.ReaksiyonTipi, x.Aciklama, x.AktifMi);
}
