using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Kutuphane;

public interface IKutuphaneServisi
{
    Task<List<DolapDto>> DolaplariGetirAsync(CancellationToken ct = default);
    Task<DolapDto> DolapOlusturAsync(DolapIstek istek, CancellationToken ct = default);
    Task DolapGuncelleAsync(int id, DolapIstek istek, CancellationToken ct = default);
    Task DolapSilAsync(int id, CancellationToken ct = default);

    Task<List<RafDto>> RaflariGetirAsync(int? dolapId, CancellationToken ct = default);
    Task<RafDto> RafOlusturAsync(RafIstek istek, CancellationToken ct = default);
    Task RafGuncelleAsync(int id, RafIstek istek, CancellationToken ct = default);
    Task RafSilAsync(int id, CancellationToken ct = default);

    Task<List<KitapDto>> KitaplariGetirAsync(string? arama, CancellationToken ct = default);
    Task<KitapDto> KitapOlusturAsync(KitapIstek istek, CancellationToken ct = default);
    Task KitapGuncelleAsync(int id, KitapIstek istek, CancellationToken ct = default);
    Task KitapSilAsync(int id, CancellationToken ct = default);
    Task KapakYukleAsync(int id, string dosyaAdi, string icerikTipi, Stream icerik, CancellationToken ct = default);
    Task<KayitliDosya> KapakGetirAsync(int id, CancellationToken ct = default);

    Task<List<KitapKopyaDto>> KopyalariGetirAsync(int? kitapId, string? durum, CancellationToken ct = default);
    Task<KitapKopyaDto> KopyaOlusturAsync(KitapKopyaIstek istek, CancellationToken ct = default);
    Task KopyaGuncelleAsync(int id, KitapKopyaIstek istek, CancellationToken ct = default);
    Task KopyaSilAsync(int id, CancellationToken ct = default);
    Task<byte[]> EtiketUretAsync(int id, CancellationToken ct = default);

    Task<List<KitapOduncDto>> OduncleriGetirAsync(bool? gecikenler, CancellationToken ct = default);
    Task<KitapOduncDto> OduncOlusturAsync(KitapOduncIstek istek, CancellationToken ct = default);
    Task IadeEtAsync(int id, KitapIadeIstek istek, CancellationToken ct = default);
}

public class KutuphaneServisi : IKutuphaneServisi
{
    private readonly IUygulamaDbContext _db;
    private readonly IDosyaDepolama _dosya;
    private readonly IBarkodEtiketUretici _etiket;

    public KutuphaneServisi(IUygulamaDbContext db, IDosyaDepolama dosya, IBarkodEtiketUretici etiket)
    {
        _db = db;
        _dosya = dosya;
        _etiket = etiket;
    }

    public async Task<List<DolapDto>> DolaplariGetirAsync(CancellationToken ct = default)
    {
        var kayitlar = await _db.KutuphaneDolaplari.AsNoTracking().OrderBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(x => new DolapDto(x.Id, x.Ad, x.Konum, x.AktifMi)).ToList();
    }

    public async Task<DolapDto> DolapOlusturAsync(DolapIstek istek, CancellationToken ct = default)
    {
        var kayit = new KutuphaneDolap { Ad = istek.Ad.Trim(), Konum = Metin(istek.Konum), AktifMi = istek.AktifMi, OlusturulmaTarihi = DateTime.UtcNow };
        _db.KutuphaneDolaplari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return new DolapDto(kayit.Id, kayit.Ad, kayit.Konum, kayit.AktifMi);
    }

    public async Task DolapGuncelleAsync(int id, DolapIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.KutuphaneDolaplari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Dolap bulunamadı.");
        kayit.Ad = istek.Ad.Trim();
        kayit.Konum = Metin(istek.Konum);
        kayit.AktifMi = istek.AktifMi;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DolapSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.KutuphaneDolaplari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Dolap bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<RafDto>> RaflariGetirAsync(int? dolapId, CancellationToken ct = default)
    {
        var sorgu = _db.KutuphaneRaflari.AsNoTracking().Include(x => x.Dolap).AsQueryable();
        if (dolapId is not null) sorgu = sorgu.Where(x => x.DolapId == dolapId);
        var kayitlar = await sorgu.OrderBy(x => x.Dolap!.Ad).ThenBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(x => new RafDto(x.Id, x.DolapId, x.Dolap?.Ad ?? "", x.Ad)).ToList();
    }

    public async Task<RafDto> RafOlusturAsync(RafIstek istek, CancellationToken ct = default)
    {
        var dolap = await _db.KutuphaneDolaplari.FirstOrDefaultAsync(x => x.Id == istek.DolapId, ct) ?? throw new KayitBulunamadiHatasi("Dolap bulunamadı.");
        var kayit = new KutuphaneRaf { DolapId = dolap.Id, Ad = istek.Ad.Trim(), OlusturulmaTarihi = DateTime.UtcNow };
        _db.KutuphaneRaflari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return new RafDto(kayit.Id, dolap.Id, dolap.Ad, kayit.Ad);
    }

    public async Task RafGuncelleAsync(int id, RafIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.KutuphaneRaflari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Raf bulunamadı.");
        var dolap = await _db.KutuphaneDolaplari.FirstOrDefaultAsync(x => x.Id == istek.DolapId, ct) ?? throw new KayitBulunamadiHatasi("Dolap bulunamadı.");
        kayit.DolapId = dolap.Id;
        kayit.Ad = istek.Ad.Trim();
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task RafSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.KutuphaneRaflari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Raf bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<KitapDto>> KitaplariGetirAsync(string? arama, CancellationToken ct = default)
    {
        var sorgu = _db.Kitaplar.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(arama))
        {
            var q = arama.Trim();
            sorgu = sorgu.Where(x => x.Ad.Contains(q) || (x.Yazar != null && x.Yazar.Contains(q)) || (x.Isbn != null && x.Isbn.Contains(q)));
        }
        var kayitlar = await sorgu.OrderBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(MapKitap).ToList();
    }

    public async Task<KitapDto> KitapOlusturAsync(KitapIstek istek, CancellationToken ct = default)
    {
        var kayit = new Kitap { OlusturulmaTarihi = DateTime.UtcNow };
        DoldurKitap(kayit, istek);
        _db.Kitaplar.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return MapKitap(kayit);
    }

    public async Task KitapGuncelleAsync(int id, KitapIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.Kitaplar.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Kitap bulunamadı.");
        DoldurKitap(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task KitapSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.Kitaplar.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Kitap bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task KapakYukleAsync(int id, string dosyaAdi, string icerikTipi, Stream icerik, CancellationToken ct = default)
    {
        if (icerikTipi is not ("image/jpeg" or "image/png" or "image/webp"))
            throw new GecersizIstekHatasi("Kapak JPEG, PNG veya WebP olmalıdır.");
        var kayit = await _db.Kitaplar.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Kitap bulunamadı.");
        await _dosya.SilAsync(kayit.KapakYolu, ct);
        var uzanti = Path.GetExtension(dosyaAdi);
        if (string.IsNullOrWhiteSpace(uzanti)) uzanti = ".jpg";
        kayit.KapakYolu = await _dosya.KaydetAsync($"kitaplar/{id}", uzanti, icerik, ct);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<KayitliDosya> KapakGetirAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.Kitaplar.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Kitap bulunamadı.");
        if (string.IsNullOrWhiteSpace(kayit.KapakYolu)) throw new KayitBulunamadiHatasi("Kapak yok.");
        return await _dosya.AcAsync(kayit.KapakYolu, "image/jpeg", Path.GetFileName(kayit.KapakYolu), ct)
            ?? throw new KayitBulunamadiHatasi("Kapak dosyası bulunamadı.");
    }

    public async Task<List<KitapKopyaDto>> KopyalariGetirAsync(int? kitapId, string? durum, CancellationToken ct = default)
    {
        var sorgu = _db.KitapKopyalari.AsNoTracking().Include(x => x.Kitap).Include(x => x.Raf).AsQueryable();
        if (kitapId is not null) sorgu = sorgu.Where(x => x.KitapId == kitapId);
        if (!string.IsNullOrWhiteSpace(durum)) sorgu = sorgu.Where(x => x.Durum == durum);
        var kayitlar = await sorgu.OrderBy(x => x.Barkod).ToListAsync(ct);
        return kayitlar.Select(MapKopya).ToList();
    }

    public async Task<KitapKopyaDto> KopyaOlusturAsync(KitapKopyaIstek istek, CancellationToken ct = default)
    {
        var kitap = await _db.Kitaplar.FirstOrDefaultAsync(x => x.Id == istek.KitapId, ct) ?? throw new KayitBulunamadiHatasi("Kitap bulunamadı.");
        var ayni = await _db.KitapKopyalari.AnyAsync(x => x.Barkod == istek.Barkod.Trim(), ct);
        if (ayni) throw new GecersizIstekHatasi("Bu barkod zaten kayıtlı.");
        await RafiKontrol(istek.RafId, ct);
        var kayit = new KitapKopya { KitapId = kitap.Id, OlusturulmaTarihi = DateTime.UtcNow };
        DoldurKopya(kayit, istek);
        _db.KitapKopyalari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Kitap = kitap;
        return MapKopya(kayit);
    }

    public async Task KopyaGuncelleAsync(int id, KitapKopyaIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.KitapKopyalari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Kopya bulunamadı.");
        await RafiKontrol(istek.RafId, ct);
        DoldurKopya(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task KopyaSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.KitapKopyalari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Kopya bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<byte[]> EtiketUretAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.KitapKopyalari.AsNoTracking().Include(x => x.Kitap).FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Kopya bulunamadı.");
        return _etiket.PngUret(kayit.Barkod, kayit.Kitap?.Ad ?? "Kitap");
    }

    public async Task<List<KitapOduncDto>> OduncleriGetirAsync(bool? gecikenler, CancellationToken ct = default)
    {
        var sorgu = _db.KitapOduncleri.AsNoTracking().Include(x => x.Sakin).Include(x => x.Kopya).ThenInclude(k => k!.Kitap).AsQueryable();
        var bugun = DateTime.UtcNow.Date;
        if (gecikenler == true) sorgu = sorgu.Where(x => x.IadeTarihi == null && x.PlanlananTeslim < bugun);
        var kayitlar = await sorgu.OrderByDescending(x => x.OduncTarihi).ToListAsync(ct);
        return kayitlar.Select(MapOdunc).ToList();
    }

    public async Task<KitapOduncDto> OduncOlusturAsync(KitapOduncIstek istek, CancellationToken ct = default)
    {
        var kopya = await _db.KitapKopyalari.Include(x => x.Kitap).FirstOrDefaultAsync(x => x.Id == istek.KopyaId, ct)
            ?? throw new KayitBulunamadiHatasi("Kopya bulunamadı.");
        if (kopya.Durum != "Rafta") throw new GecersizIstekHatasi("Bu kopya ödünç verilemez.");
        var sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == istek.SakinId, ct) ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        var kayit = new KitapOdunc
        {
            KopyaId = kopya.Id,
            SakinId = sakin.Id,
            OduncTarihi = SakinVarlikYardimcisi.ToUtc(istek.OduncTarihi),
            PlanlananTeslim = SakinVarlikYardimcisi.ToUtc(istek.PlanlananTeslim),
            DurumNotu = Metin(istek.DurumNotu),
            OlusturulmaTarihi = DateTime.UtcNow
        };
        kopya.Durum = "Odünçte";
        _db.KitapOduncleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Kopya = kopya;
        kayit.Sakin = sakin;
        return MapOdunc(kayit);
    }

    public async Task IadeEtAsync(int id, KitapIadeIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.KitapOduncleri.Include(x => x.Kopya).FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Ödünç kaydı bulunamadı.");
        if (kayit.IadeTarihi is not null) throw new GecersizIstekHatasi("Bu kayıt zaten iade edilmiş.");
        kayit.IadeTarihi = SakinVarlikYardimcisi.ToUtc(istek.IadeTarihi);
        kayit.DurumNotu = Metin(istek.DurumNotu);
        if (kayit.Kopya is not null) kayit.Kopya.Durum = istek.KopyaDurumu;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private async Task RafiKontrol(int? rafId, CancellationToken ct)
    {
        if (rafId is null) return;
        var varMi = await _db.KutuphaneRaflari.AnyAsync(x => x.Id == rafId, ct);
        if (!varMi) throw new KayitBulunamadiHatasi("Raf bulunamadı.");
    }

    private static void DoldurKitap(Kitap kayit, KitapIstek istek)
    {
        kayit.Isbn = Metin(istek.Isbn);
        kayit.Ad = istek.Ad.Trim();
        kayit.Yazar = Metin(istek.Yazar);
        kayit.Yayinevi = Metin(istek.Yayinevi);
        kayit.Aciklama = Metin(istek.Aciklama);
    }

    private static void DoldurKopya(KitapKopya kayit, KitapKopyaIstek istek)
    {
        kayit.KitapId = istek.KitapId;
        kayit.Barkod = istek.Barkod.Trim();
        kayit.Durum = istek.Durum;
        kayit.RafId = istek.RafId;
        kayit.Konum = Metin(istek.Konum);
    }

    private static string? Metin(string? deger) => string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();
    private static KitapDto MapKitap(Kitap x) => new(x.Id, x.Isbn, x.Ad, x.Yazar, x.Yayinevi, x.Aciklama, !string.IsNullOrWhiteSpace(x.KapakYolu));
    private static KitapKopyaDto MapKopya(KitapKopya x) => new(x.Id, x.KitapId, x.Kitap?.Ad ?? "", x.Barkod, x.Durum, x.RafId, x.Raf?.Ad, x.Konum);
    private static KitapOduncDto MapOdunc(KitapOdunc x) => new(
        x.Id, x.KopyaId, x.Kopya?.Barkod ?? "", x.Kopya?.Kitap?.Ad ?? "", x.SakinId,
        x.Sakin is null ? "" : $"{x.Sakin.Ad} {x.Sakin.Soyad}", x.OduncTarihi, x.PlanlananTeslim, x.IadeTarihi, x.DurumNotu,
        x.IadeTarihi is null && x.PlanlananTeslim.Date < DateTime.UtcNow.Date);
}
