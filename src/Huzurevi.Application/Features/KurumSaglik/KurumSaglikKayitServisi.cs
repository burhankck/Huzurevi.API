using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.KurumSaglik;

public interface IKurumSaglikKayitServisi
{
    Task<List<KurumSaglikKayitDto>> GetirAsync(string tur, int? sakinId, DateTime? baslangic, DateTime? bitis, CancellationToken ct = default);
    Task<KurumSaglikKayitDto> OlusturAsync(KurumSaglikKayitIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, KurumSaglikKayitIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task ImzalaAsync(int id, string imzalayan, CancellationToken ct = default);
}

public class KurumSaglikKayitServisi : IKurumSaglikKayitServisi
{
    private readonly IUygulamaDbContext _db;
    public KurumSaglikKayitServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<KurumSaglikKayitDto>> GetirAsync(string tur, int? sakinId, DateTime? baslangic, DateTime? bitis, CancellationToken ct = default)
    {
        if (!KurumSaglikTurleri.Tum.Contains(tur))
        {
            throw new GecersizIstekHatasi("Geçersiz kayıt türü.");
        }

        var sorgu = _db.KurumSaglikKayitlari.AsNoTracking().Include(x => x.Sakin).Where(x => x.Tur == tur);
        if (sakinId is not null) sorgu = sorgu.Where(x => x.SakinId == sakinId);
        if (baslangic is not null) sorgu = sorgu.Where(x => x.Tarih >= SakinVarlikYardimcisi.ToUtc(baslangic.Value));
        if (bitis is not null) sorgu = sorgu.Where(x => x.Tarih < SakinVarlikYardimcisi.ToUtc(bitis.Value).AddDays(1));

        var kayitlar = await sorgu.OrderByDescending(x => x.Tarih).ThenByDescending(x => x.Id).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<KurumSaglikKayitDto> OlusturAsync(KurumSaglikKayitIstek istek, CancellationToken ct = default)
    {
        await SakiniDogrulaAsync(istek, ct);
        var kayit = new KurumSaglikKayit { OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.KurumSaglikKayitlari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Sakin = istek.SakinId is null ? null : await _db.Sakinler.AsNoTracking().FirstOrDefaultAsync(s => s.Id == istek.SakinId, ct);
        return Map(kayit);
    }

    public async Task GuncelleAsync(int id, KurumSaglikKayitIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.KurumSaglikKayitlari.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Kayıt bulunamadı.");
        if (kayit.ImzalandiMi)
        {
            throw new GecersizIstekHatasi("İmzalanmış kayıt düzenlenemez.");
        }

        await SakiniDogrulaAsync(istek, ct);
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.KurumSaglikKayitlari.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Kayıt bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task ImzalaAsync(int id, string imzalayan, CancellationToken ct = default)
    {
        var kayit = await _db.KurumSaglikKayitlari.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Kayıt bulunamadı.");
        kayit.ImzalandiMi = true;
        kayit.Imzalayan = imzalayan;
        kayit.ImzaTarihi = DateTime.UtcNow;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private async Task SakiniDogrulaAsync(KurumSaglikKayitIstek istek, CancellationToken ct)
    {
        if (istek.Tur == KurumSaglikTurleri.Nobet) return;
        if (istek.SakinId is null) throw new GecersizIstekHatasi("Sakin seçiniz.");
        var varMi = await _db.Sakinler.AnyAsync(s => s.Id == istek.SakinId, ct);
        if (!varMi) throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
    }

    private static void Doldur(KurumSaglikKayit kayit, KurumSaglikKayitIstek istek)
    {
        kayit.Tur = istek.Tur;
        kayit.SakinId = istek.Tur == KurumSaglikTurleri.Nobet ? null : istek.SakinId;
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.Personel = Metin(istek.Personel);
        kayit.Notlar = Metin(istek.Notlar);
        kayit.KanSekeri = istek.KanSekeri;
        kayit.OlcumZamani = Metin(istek.OlcumZamani);
        kayit.Sistolik = istek.Sistolik;
        kayit.Diastolik = istek.Diastolik;
        kayit.Nabiz = istek.Nabiz;
        kayit.Tedaviler = Metin(istek.Tedaviler);
        kayit.DurumDegerlendirme = Metin(istek.DurumDegerlendirme);
        kayit.HareketKabiliyeti = Metin(istek.HareketKabiliyeti);
        kayit.GucDenge = Metin(istek.GucDenge);
        kayit.KayitTuru = Metin(istek.KayitTuru);
        kayit.YapilanIslemler = Metin(istek.YapilanIslemler);
        kayit.Malzeme = Metin(istek.Malzeme);
        kayit.Nobetci = Metin(istek.Nobetci);
        kayit.GenelDurum = Metin(istek.GenelDurum);
        kayit.OnemliOlaylar = Metin(istek.OnemliOlaylar);
        kayit.DevirTeslim = Metin(istek.DevirTeslim);
        kayit.Doktor = Metin(istek.Doktor);
        kayit.Bulgular = Metin(istek.Bulgular);
        kayit.FizikMuayene = Metin(istek.FizikMuayene);
        kayit.LabSonuclari = Metin(istek.LabSonuclari);
        kayit.Oneriler = Metin(istek.Oneriler);
    }

    private static string? Metin(string? deger) => string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();

    private static KurumSaglikKayitDto Map(KurumSaglikKayit x) => new(
        x.Id, x.Tur, x.SakinId, x.Sakin is null ? null : $"{x.Sakin.Ad} {x.Sakin.Soyad}", x.Tarih, x.Personel, x.Notlar,
        x.ImzalandiMi, x.Imzalayan, x.ImzaTarihi, x.KanSekeri, x.OlcumZamani, x.Sistolik, x.Diastolik, x.Nabiz,
        x.Tedaviler, x.DurumDegerlendirme, x.HareketKabiliyeti, x.GucDenge, x.KayitTuru, x.YapilanIslemler, x.Malzeme,
        x.Nobetci, x.GenelDurum, x.OnemliOlaylar, x.DevirTeslim, x.Doktor, x.Bulgular, x.FizikMuayene, x.LabSonuclari, x.Oneriler);
}
