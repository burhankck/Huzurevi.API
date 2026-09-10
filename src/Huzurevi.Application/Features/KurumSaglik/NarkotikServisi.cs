using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.KurumSaglik;

public interface INarkotikServisi
{
    Task<List<NarkotikIlacDto>> IlaclariGetirAsync(CancellationToken ct = default);
    Task<NarkotikIlacDto> IlacOlusturAsync(NarkotikIlacIstek istek, CancellationToken ct = default);
    Task IlacGuncelleAsync(int id, NarkotikIlacIstek istek, CancellationToken ct = default);
    Task IlacSilAsync(int id, CancellationToken ct = default);
    Task<List<NarkotikHareketDto>> HareketleriGetirAsync(int? ilacId, DateTime? baslangic, DateTime? bitis, CancellationToken ct = default);
    Task<NarkotikHareketDto> HareketOlusturAsync(NarkotikHareketIstek istek, CancellationToken ct = default);
}

public class NarkotikServisi : INarkotikServisi
{
    private readonly IUygulamaDbContext _db;
    public NarkotikServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<NarkotikIlacDto>> IlaclariGetirAsync(CancellationToken ct = default)
    {
        var kayitlar = await _db.NarkotikIlaclari.AsNoTracking().OrderBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(x => new NarkotikIlacDto(x.Id, x.Ad, x.Stok, x.Birim, x.AktifMi)).ToList();
    }

    public async Task<NarkotikIlacDto> IlacOlusturAsync(NarkotikIlacIstek istek, CancellationToken ct = default)
    {
        var kayit = new NarkotikIlac
        {
            Ad = istek.Ad.Trim(),
            Birim = string.IsNullOrWhiteSpace(istek.Birim) ? "adet" : istek.Birim.Trim(),
            AktifMi = istek.AktifMi,
            Stok = 0,
            OlusturulmaTarihi = DateTime.UtcNow
        };
        _db.NarkotikIlaclari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return new NarkotikIlacDto(kayit.Id, kayit.Ad, kayit.Stok, kayit.Birim, kayit.AktifMi);
    }

    public async Task IlacGuncelleAsync(int id, NarkotikIlacIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.NarkotikIlaclari.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("İlaç bulunamadı.");
        kayit.Ad = istek.Ad.Trim();
        kayit.Birim = string.IsNullOrWhiteSpace(istek.Birim) ? kayit.Birim : istek.Birim.Trim();
        kayit.AktifMi = istek.AktifMi;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task IlacSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.NarkotikIlaclari.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("İlaç bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<NarkotikHareketDto>> HareketleriGetirAsync(int? ilacId, DateTime? baslangic, DateTime? bitis, CancellationToken ct = default)
    {
        var sorgu = _db.NarkotikHareketleri.AsNoTracking().Include(x => x.Ilac).Include(x => x.Sakin).AsQueryable();
        if (ilacId is not null) sorgu = sorgu.Where(x => x.NarkotikIlacId == ilacId);
        if (baslangic is not null) sorgu = sorgu.Where(x => x.Tarih >= SakinVarlikYardimcisi.ToUtc(baslangic.Value));
        if (bitis is not null) sorgu = sorgu.Where(x => x.Tarih < SakinVarlikYardimcisi.ToUtc(bitis.Value).AddDays(1));
        var kayitlar = await sorgu.OrderByDescending(x => x.Tarih).ToListAsync(ct);
        return kayitlar.Select(x => new NarkotikHareketDto(
            x.Id, x.NarkotikIlacId, x.Ilac?.Ad ?? "", x.HareketTuru, x.Miktar, x.Tarih, x.SakinId,
            x.Sakin is null ? null : $"{x.Sakin.Ad} {x.Sakin.Soyad}", x.Notlar)).ToList();
    }

    public async Task<NarkotikHareketDto> HareketOlusturAsync(NarkotikHareketIstek istek, CancellationToken ct = default)
    {
        var ilac = await _db.NarkotikIlaclari.FirstOrDefaultAsync(x => x.Id == istek.NarkotikIlacId, ct)
            ?? throw new KayitBulunamadiHatasi("İlaç bulunamadı.");
        if (istek.HareketTuru == "Kullanim" && ilac.Stok < istek.Miktar)
        {
            throw new GecersizIstekHatasi("Stok yetersiz.");
        }

        Sakin? sakin = null;
        if (istek.SakinId is not null)
        {
            sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == istek.SakinId, ct)
                ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        }

        var hareket = new NarkotikHareket
        {
            NarkotikIlacId = ilac.Id,
            HareketTuru = istek.HareketTuru,
            Miktar = istek.Miktar,
            Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih),
            SakinId = istek.SakinId,
            Notlar = string.IsNullOrWhiteSpace(istek.Notlar) ? null : istek.Notlar.Trim(),
            OlusturulmaTarihi = DateTime.UtcNow
        };
        ilac.Stok += istek.HareketTuru == "Giris" ? istek.Miktar : -istek.Miktar;
        _db.NarkotikHareketleri.Add(hareket);
        await _db.SaveChangesAsync(ct);
        return new NarkotikHareketDto(hareket.Id, ilac.Id, ilac.Ad, hareket.HareketTuru, hareket.Miktar, hareket.Tarih, hareket.SakinId,
            sakin is null ? null : $"{sakin.Ad} {sakin.Soyad}", hareket.Notlar);
    }
}
