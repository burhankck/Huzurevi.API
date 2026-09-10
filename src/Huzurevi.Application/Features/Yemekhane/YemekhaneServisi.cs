using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Yemekhane;

public interface IYemekhaneServisi
{
    Task<List<YemekDto>> YemekleriGetirAsync(string? kategori, string? arama, CancellationToken ct = default);
    Task<YemekDto> YemekOlusturAsync(YemekIstek istek, CancellationToken ct = default);
    Task YemekGuncelleAsync(int id, YemekIstek istek, CancellationToken ct = default);
    Task YemekSilAsync(int id, CancellationToken ct = default);

    Task<List<BeslenmeProfiliDto>> ProfilleriGetirAsync(CancellationToken ct = default);
    Task<BeslenmeProfiliDto> ProfilOlusturAsync(BeslenmeProfiliIstek istek, CancellationToken ct = default);
    Task ProfilGuncelleAsync(int id, BeslenmeProfiliIstek istek, CancellationToken ct = default);
    Task ProfilSilAsync(int id, CancellationToken ct = default);

    Task<List<GunlukMenuDto>> MenuleriGetirAsync(DateTime? tarih, CancellationToken ct = default);
    Task<List<GunlukMenuDto>> MenuOlusturAsync(GunlukMenuIstek istek, CancellationToken ct = default);
    Task MenuSilAsync(int id, CancellationToken ct = default);
    Task GunSilAsync(DateTime tarih, CancellationToken ct = default);

    Task<List<OzelMenuDto>> OzelMenuleriGetirAsync(int? sakinId, DateTime? tarih, CancellationToken ct = default);
    Task<OzelMenuDto> OzelMenuOlusturAsync(OzelMenuIstek istek, CancellationToken ct = default);
    Task OzelMenuGuncelleAsync(int id, OzelMenuIstek istek, CancellationToken ct = default);
    Task OzelMenuSilAsync(int id, CancellationToken ct = default);

    Task<List<YemekTuketimDto>> TuketimleriGetirAsync(DateTime? tarih, string? ogunTipi, int? sakinId, CancellationToken ct = default);
    Task<YemekTuketimDto> TuketimOlusturAsync(YemekTuketimIstek istek, CancellationToken ct = default);
    Task TuketimGuncelleAsync(int id, YemekTuketimIstek istek, CancellationToken ct = default);
    Task TuketimSilAsync(int id, CancellationToken ct = default);
    Task ServisDoldurAsync(DateTime tarih, string ogunTipi, CancellationToken ct = default);

    Task<List<SiviAlimiDto>> SivlariGetirAsync(int? sakinId, DateTime? tarih, CancellationToken ct = default);
    Task<SiviAlimiDto> SiviOlusturAsync(SiviAlimiIstek istek, CancellationToken ct = default);
    Task SiviGuncelleAsync(int id, SiviAlimiIstek istek, CancellationToken ct = default);
    Task SiviSilAsync(int id, CancellationToken ct = default);

    Task<List<BeslenmeOzetDto>> OzetGetirAsync(DateTime tarih, CancellationToken ct = default);
}

public class YemekhaneServisi : IYemekhaneServisi
{
    private readonly IUygulamaDbContext _db;
    public YemekhaneServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<YemekDto>> YemekleriGetirAsync(string? kategori, string? arama, CancellationToken ct = default)
    {
        var sorgu = _db.Yemekler.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(kategori)) sorgu = sorgu.Where(x => x.Kategori == kategori);
        if (!string.IsNullOrWhiteSpace(arama))
        {
            var q = arama.Trim();
            sorgu = sorgu.Where(x => x.Ad.Contains(q) || (x.Alerjenler != null && x.Alerjenler.Contains(q)));
        }
        var kayitlar = await sorgu.OrderBy(x => x.Kategori).ThenBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(MapYemek).ToList();
    }

    public async Task<YemekDto> YemekOlusturAsync(YemekIstek istek, CancellationToken ct = default)
    {
        var kayit = new Yemek { OlusturulmaTarihi = DateTime.UtcNow };
        DoldurYemek(kayit, istek);
        _db.Yemekler.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return MapYemek(kayit);
    }

    public async Task YemekGuncelleAsync(int id, YemekIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.Yemekler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Yemek bulunamadı.");
        DoldurYemek(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task YemekSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.Yemekler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Yemek bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<BeslenmeProfiliDto>> ProfilleriGetirAsync(CancellationToken ct = default)
    {
        var kayitlar = await _db.BeslenmeProfilleri.AsNoTracking().Include(x => x.Sakin).OrderBy(x => x.Sakin!.Ad).ToListAsync(ct);
        return kayitlar.Select(MapProfil).ToList();
    }

    public async Task<BeslenmeProfiliDto> ProfilOlusturAsync(BeslenmeProfiliIstek istek, CancellationToken ct = default)
    {
        var sakin = await SakiniGetir(istek.SakinId, ct);
        var varMi = await _db.BeslenmeProfilleri.AnyAsync(x => x.SakinId == istek.SakinId, ct);
        if (varMi) throw new GecersizIstekHatasi("Bu sakin için profil zaten var.");
        var kayit = new BeslenmeProfili { SakinId = sakin.Id, OlusturulmaTarihi = DateTime.UtcNow };
        DoldurProfil(kayit, istek);
        _db.BeslenmeProfilleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Sakin = sakin;
        return MapProfil(kayit);
    }

    public async Task ProfilGuncelleAsync(int id, BeslenmeProfiliIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.BeslenmeProfilleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Profil bulunamadı.");
        await SakiniGetir(istek.SakinId, ct);
        DoldurProfil(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task ProfilSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.BeslenmeProfilleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Profil bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<GunlukMenuDto>> MenuleriGetirAsync(DateTime? tarih, CancellationToken ct = default)
    {
        var sorgu = _db.GunlukMenuler.AsNoTracking().Include(x => x.Yemek).AsQueryable();
        if (tarih is not null)
        {
            var gun = SakinVarlikYardimcisi.ToUtc(tarih.Value).Date;
            sorgu = sorgu.Where(x => x.Tarih >= gun && x.Tarih < gun.AddDays(1));
        }
        var kayitlar = await sorgu.OrderByDescending(x => x.Tarih).ThenBy(x => x.OgunTipi).ToListAsync(ct);
        return kayitlar.Select(MapMenu).ToList();
    }

    public async Task<List<GunlukMenuDto>> MenuOlusturAsync(GunlukMenuIstek istek, CancellationToken ct = default)
    {
        var tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih).Date;
        var yemekler = await _db.Yemekler.Where(x => istek.YemekIdleri.Contains(x.Id)).ToListAsync(ct);
        if (yemekler.Count != istek.YemekIdleri.Distinct().Count()) throw new KayitBulunamadiHatasi("Seçilen yemeklerden biri bulunamadı.");
        var kayitlar = istek.YemekIdleri.Distinct().Select(yemekId => new GunlukMenu
        {
            Tarih = tarih,
            OgunTipi = istek.OgunTipi,
            YemekId = yemekId,
            OlusturulmaTarihi = DateTime.UtcNow
        }).ToList();
        _db.GunlukMenuler.AddRange(kayitlar);
        await _db.SaveChangesAsync(ct);
        return kayitlar.Select(k => MapMenu(k, yemekler.First(y => y.Id == k.YemekId))).ToList();
    }

    public async Task MenuSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.GunlukMenuler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Menü kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task GunSilAsync(DateTime tarih, CancellationToken ct = default)
    {
        var gun = SakinVarlikYardimcisi.ToUtc(tarih).Date;
        var kayitlar = await _db.GunlukMenuler.Where(x => x.Tarih >= gun && x.Tarih < gun.AddDays(1)).ToListAsync(ct);
        foreach (var kayit in kayitlar)
        {
            kayit.SilindiMi = true;
            kayit.SilinmeTarihi = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<OzelMenuDto>> OzelMenuleriGetirAsync(int? sakinId, DateTime? tarih, CancellationToken ct = default)
    {
        var sorgu = _db.OzelMenuPlanlari.AsNoTracking().Include(x => x.Sakin).Include(x => x.Yemek).AsQueryable();
        if (sakinId is not null) sorgu = sorgu.Where(x => x.SakinId == sakinId);
        if (tarih is not null)
        {
            var gun = SakinVarlikYardimcisi.ToUtc(tarih.Value).Date;
            sorgu = sorgu.Where(x => x.Tarih >= gun && x.Tarih < gun.AddDays(1));
        }
        var kayitlar = await sorgu.OrderByDescending(x => x.Tarih).ToListAsync(ct);
        return kayitlar.Select(MapOzel).ToList();
    }

    public async Task<OzelMenuDto> OzelMenuOlusturAsync(OzelMenuIstek istek, CancellationToken ct = default)
    {
        var sakin = await SakiniGetir(istek.SakinId, ct);
        var yemek = await _db.Yemekler.FirstOrDefaultAsync(x => x.Id == istek.YemekId, ct) ?? throw new KayitBulunamadiHatasi("Yemek bulunamadı.");
        var kayit = new OzelMenuPlani { SakinId = sakin.Id, OlusturulmaTarihi = DateTime.UtcNow };
        DoldurOzel(kayit, istek);
        _db.OzelMenuPlanlari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Sakin = sakin;
        kayit.Yemek = yemek;
        return MapOzel(kayit);
    }

    public async Task OzelMenuGuncelleAsync(int id, OzelMenuIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.OzelMenuPlanlari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Özel menü bulunamadı.");
        await SakiniGetir(istek.SakinId, ct);
        DoldurOzel(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task OzelMenuSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.OzelMenuPlanlari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Özel menü bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<YemekTuketimDto>> TuketimleriGetirAsync(DateTime? tarih, string? ogunTipi, int? sakinId, CancellationToken ct = default)
    {
        var sorgu = _db.YemekTuketimleri.AsNoTracking().Include(x => x.Sakin).Include(x => x.Yemek).AsQueryable();
        if (sakinId is not null) sorgu = sorgu.Where(x => x.SakinId == sakinId);
        if (!string.IsNullOrWhiteSpace(ogunTipi)) sorgu = sorgu.Where(x => x.OgunTipi == ogunTipi);
        if (tarih is not null)
        {
            var gun = SakinVarlikYardimcisi.ToUtc(tarih.Value).Date;
            sorgu = sorgu.Where(x => x.Tarih >= gun && x.Tarih < gun.AddDays(1));
        }
        var kayitlar = await sorgu.OrderBy(x => x.Sakin!.Ad).ThenBy(x => x.OgunTipi).ToListAsync(ct);
        return kayitlar.Select(MapTuketim).ToList();
    }

    public async Task<YemekTuketimDto> TuketimOlusturAsync(YemekTuketimIstek istek, CancellationToken ct = default)
    {
        var sakin = await SakiniGetir(istek.SakinId, ct);
        var yemek = await _db.Yemekler.FirstOrDefaultAsync(x => x.Id == istek.YemekId, ct) ?? throw new KayitBulunamadiHatasi("Yemek bulunamadı.");
        var kayit = new YemekTuketim { SakinId = sakin.Id, OlusturulmaTarihi = DateTime.UtcNow };
        DoldurTuketim(kayit, istek);
        _db.YemekTuketimleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Sakin = sakin;
        kayit.Yemek = yemek;
        return MapTuketim(kayit);
    }

    public async Task TuketimGuncelleAsync(int id, YemekTuketimIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.YemekTuketimleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Tüketim kaydı bulunamadı.");
        DoldurTuketim(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task TuketimSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.YemekTuketimleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Tüketim kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task ServisDoldurAsync(DateTime tarih, string ogunTipi, CancellationToken ct = default)
    {
        if (!YemekhaneSabitleri.Ogunler.Contains(ogunTipi)) throw new GecersizIstekHatasi("Geçersiz öğün.");
        var gun = SakinVarlikYardimcisi.ToUtc(tarih).Date;
        var yemekler = await _db.GunlukMenuler.Where(x => x.Tarih >= gun && x.Tarih < gun.AddDays(1) && x.OgunTipi == ogunTipi)
            .Select(x => x.YemekId).Distinct().ToListAsync(ct);
        var ozel = await _db.OzelMenuPlanlari.Where(x => x.Tarih >= gun && x.Tarih < gun.AddDays(1) && x.OgunTipi == ogunTipi).ToListAsync(ct);
        var sakinler = await _db.Sakinler.AsNoTracking().Select(s => s.Id).ToListAsync(ct);
        var mevcut = await _db.YemekTuketimleri.Where(x => x.Tarih >= gun && x.Tarih < gun.AddDays(1) && x.OgunTipi == ogunTipi)
            .Select(x => new { x.SakinId, x.YemekId }).ToListAsync(ct);
        var eklenecek = new List<YemekTuketim>();
        foreach (var sakinId in sakinler)
        {
            var alternatif = ozel.Where(x => x.SakinId == sakinId).Select(x => x.YemekId).ToList();
            var liste = alternatif.Count > 0 ? alternatif : yemekler;
            foreach (var yemekId in liste)
            {
                if (mevcut.Any(m => m.SakinId == sakinId && m.YemekId == yemekId)) continue;
                eklenecek.Add(new YemekTuketim
                {
                    SakinId = sakinId,
                    Tarih = gun,
                    OgunTipi = ogunTipi,
                    YemekId = yemekId,
                    TuketildiMi = false,
                    OlusturulmaTarihi = DateTime.UtcNow
                });
            }
        }
        _db.YemekTuketimleri.AddRange(eklenecek);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<SiviAlimiDto>> SivlariGetirAsync(int? sakinId, DateTime? tarih, CancellationToken ct = default)
    {
        var sorgu = _db.SiviAlimlari.AsNoTracking().Include(x => x.Sakin).AsQueryable();
        if (sakinId is not null) sorgu = sorgu.Where(x => x.SakinId == sakinId);
        if (tarih is not null)
        {
            var gun = SakinVarlikYardimcisi.ToUtc(tarih.Value).Date;
            sorgu = sorgu.Where(x => x.Tarih >= gun && x.Tarih < gun.AddDays(1));
        }
        var kayitlar = await sorgu.OrderByDescending(x => x.Tarih).ToListAsync(ct);
        return kayitlar.Select(MapSivi).ToList();
    }

    public async Task<SiviAlimiDto> SiviOlusturAsync(SiviAlimiIstek istek, CancellationToken ct = default)
    {
        var sakin = await SakiniGetir(istek.SakinId, ct);
        var kayit = new SiviAlimi
        {
            SakinId = sakin.Id,
            Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih),
            SiviTuru = istek.SiviTuru,
            MiktarMl = istek.MiktarMl,
            OlusturulmaTarihi = DateTime.UtcNow
        };
        _db.SiviAlimlari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Sakin = sakin;
        return MapSivi(kayit);
    }

    public async Task SiviGuncelleAsync(int id, SiviAlimiIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.SiviAlimlari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Sıvı kaydı bulunamadı.");
        await SakiniGetir(istek.SakinId, ct);
        kayit.SakinId = istek.SakinId;
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.SiviTuru = istek.SiviTuru;
        kayit.MiktarMl = istek.MiktarMl;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SiviSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.SiviAlimlari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Sıvı kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<BeslenmeOzetDto>> OzetGetirAsync(DateTime tarih, CancellationToken ct = default)
    {
        var gun = SakinVarlikYardimcisi.ToUtc(tarih).Date;
        var sakinler = await _db.Sakinler.AsNoTracking().OrderBy(s => s.Ad).ToListAsync(ct);
        var tuketim = await _db.YemekTuketimleri.AsNoTracking().Include(x => x.Yemek)
            .Where(x => x.Tarih >= gun && x.Tarih < gun.AddDays(1)).ToListAsync(ct);
        var sivilar = await _db.SiviAlimlari.AsNoTracking().Where(x => x.Tarih >= gun && x.Tarih < gun.AddDays(1)).ToListAsync(ct);
        return sakinler.Select(s =>
        {
            var ogunler = tuketim.Where(t => t.SakinId == s.Id).ToList();
            var tuketilen = ogunler.Where(t => t.TuketildiMi).ToList();
            return new BeslenmeOzetDto(
                s.Id,
                $"{s.Ad} {s.Soyad}",
                tuketilen.Sum(t => t.Yemek?.Kalori ?? 0),
                tuketilen.Sum(t => t.Yemek?.Protein ?? 0),
                sivilar.Where(v => v.SakinId == s.Id).Sum(v => v.MiktarMl),
                ogunler.Select(t => new BeslenmeOzetKalemDto(t.OgunTipi, t.Yemek?.Ad ?? "", t.TuketildiMi, t.TuketilmemeNedeni, t.Yemek?.Kalori, t.Yemek?.Protein)).ToList());
        }).ToList();
    }

    private async Task<Sakin> SakiniGetir(int id, CancellationToken ct) =>
        await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");

    private static void DoldurYemek(Yemek kayit, YemekIstek istek)
    {
        kayit.Ad = istek.Ad.Trim();
        kayit.Kategori = istek.Kategori;
        kayit.Kalori = istek.Kalori;
        kayit.Protein = istek.Protein;
        kayit.Karbonhidrat = istek.Karbonhidrat;
        kayit.Yag = istek.Yag;
        kayit.Alerjenler = Metin(istek.Alerjenler);
        kayit.Tekstur = Metin(istek.Tekstur);
        kayit.AktifMi = istek.AktifMi;
    }

    private static void DoldurProfil(BeslenmeProfili kayit, BeslenmeProfiliIstek istek)
    {
        kayit.SakinId = istek.SakinId;
        kayit.TeksturTercihi = Metin(istek.TeksturTercihi);
        kayit.HedefKalori = istek.HedefKalori;
        kayit.HedefProtein = istek.HedefProtein;
        kayit.HedefSiviMl = istek.HedefSiviMl;
        kayit.Notlar = Metin(istek.Notlar);
    }

    private static void DoldurOzel(OzelMenuPlani kayit, OzelMenuIstek istek)
    {
        kayit.SakinId = istek.SakinId;
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.OgunTipi = istek.OgunTipi;
        kayit.YemekId = istek.YemekId;
        kayit.TeksturMod = Metin(istek.TeksturMod);
        kayit.SiviMl = istek.SiviMl;
        kayit.Notlar = Metin(istek.Notlar);
    }

    private static void DoldurTuketim(YemekTuketim kayit, YemekTuketimIstek istek)
    {
        kayit.SakinId = istek.SakinId;
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.OgunTipi = istek.OgunTipi;
        kayit.YemekId = istek.YemekId;
        kayit.TuketildiMi = istek.TuketildiMi;
        kayit.TuketilmemeNedeni = istek.TuketildiMi ? null : Metin(istek.TuketilmemeNedeni);
    }

    private static string? Metin(string? deger) => string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();
    private static string Ad(Sakin? s) => s is null ? "" : $"{s.Ad} {s.Soyad}";
    private static YemekDto MapYemek(Yemek x) => new(x.Id, x.Ad, x.Kategori, x.Kalori, x.Protein, x.Karbonhidrat, x.Yag, x.Alerjenler, x.Tekstur, x.AktifMi);
    private static BeslenmeProfiliDto MapProfil(BeslenmeProfili x) => new(x.Id, x.SakinId, Ad(x.Sakin), x.TeksturTercihi, x.HedefKalori, x.HedefProtein, x.HedefSiviMl, x.Notlar);
    private static GunlukMenuDto MapMenu(GunlukMenu x) => MapMenu(x, x.Yemek);
    private static GunlukMenuDto MapMenu(GunlukMenu x, Yemek? yemek) => new(x.Id, x.Tarih, x.OgunTipi, x.YemekId, yemek?.Ad ?? "");
    private static OzelMenuDto MapOzel(OzelMenuPlani x) => new(x.Id, x.SakinId, Ad(x.Sakin), x.Tarih, x.OgunTipi, x.YemekId, x.Yemek?.Ad ?? "", x.TeksturMod, x.SiviMl, x.Notlar);
    private static YemekTuketimDto MapTuketim(YemekTuketim x) => new(x.Id, x.SakinId, Ad(x.Sakin), x.Tarih, x.OgunTipi, x.YemekId, x.Yemek?.Ad ?? "", x.TuketildiMi, x.TuketilmemeNedeni);
    private static SiviAlimiDto MapSivi(SiviAlimi x) => new(x.Id, x.SakinId, Ad(x.Sakin), x.Tarih, x.SiviTuru, x.MiktarMl);
}
