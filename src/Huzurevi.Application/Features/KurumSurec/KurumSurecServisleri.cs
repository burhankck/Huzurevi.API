using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.KurumSurec;

public interface IIzinSureciServisi
{
    Task<List<IzinSureciDto>> GetirAsync(int? sakinId, string? onayDurumu, DateTime? baslangic, DateTime? bitis, CancellationToken ct = default);
    Task<IzinSureciDto> OlusturAsync(IzinSureciIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, IzinSureciIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task OnaylaAsync(int id, OnayIstek istek, string kim, CancellationToken ct = default);
}

public interface IEsyaTespitServisi
{
    Task<List<EsyaTespitDto>> GetirAsync(int? sakinId, string? onayDurumu, CancellationToken ct = default);
    Task<EsyaTespitDto> OlusturAsync(EsyaTespitIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, EsyaTespitIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task OnaylaAsync(int id, OnayIstek istek, string kim, CancellationToken ct = default);
    Task<byte[]> PdfUretAsync(int sakinId, CancellationToken ct = default);
}

public interface IMirasciTeslimServisi
{
    Task<List<MirasciTeslimDto>> GetirAsync(int? sakinId, CancellationToken ct = default);
    Task<MirasciTeslimDto> OlusturAsync(MirasciTeslimIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, MirasciTeslimIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task OnaylaAsync(int id, OnayIstek istek, string kim, CancellationToken ct = default);
    Task<byte[]> PdfUretAsync(int id, CancellationToken ct = default);
    Task EvrakYukleAsync(int id, string dosyaAdi, string icerikTipi, Stream icerik, CancellationToken ct = default);
    Task<KayitliDosya> EvrakGetirAsync(int id, CancellationToken ct = default);
}

public interface ISosyalIncelemeServisi
{
    Task<List<SosyalIncelemeDto>> GetirAsync(string? onayDurumu, DateTime? baslangic, DateTime? bitis, string? arama, CancellationToken ct = default);
    Task<SosyalIncelemeDto> OlusturAsync(SosyalIncelemeIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, SosyalIncelemeIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task OnaylaAsync(int id, OnayIstek istek, string kim, CancellationToken ct = default);
}

public interface IPsikolojikDegerlendirmeServisi
{
    Task<List<PsikolojikDegerlendirmeDto>> GetirAsync(int? sakinId, string? tur, CancellationToken ct = default);
    Task<PsikolojikDegerlendirmeDto> OlusturAsync(PsikolojikDegerlendirmeIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, PsikolojikDegerlendirmeIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task OnaylaAsync(int id, OnayIstek istek, string kim, CancellationToken ct = default);
}

public class IzinSureciServisi : IIzinSureciServisi
{
    private readonly IUygulamaDbContext _db;
    public IzinSureciServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<IzinSureciDto>> GetirAsync(int? sakinId, string? onayDurumu, DateTime? baslangic, DateTime? bitis, CancellationToken ct = default)
    {
        var sorgu = _db.IzinSurecleri.AsNoTracking().Include(x => x.Sakin).AsQueryable();
        if (sakinId is not null) sorgu = sorgu.Where(x => x.SakinId == sakinId);
        if (!string.IsNullOrWhiteSpace(onayDurumu)) sorgu = sorgu.Where(x => x.OnayDurumu == onayDurumu);
        if (baslangic is not null) sorgu = sorgu.Where(x => x.BaslangicTarihi >= SakinVarlikYardimcisi.ToUtc(baslangic.Value));
        if (bitis is not null) sorgu = sorgu.Where(x => x.BitisTarihi <= SakinVarlikYardimcisi.ToUtc(bitis.Value).AddDays(1));
        var kayitlar = await sorgu.OrderByDescending(x => x.BaslangicTarihi).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<IzinSureciDto> OlusturAsync(IzinSureciIstek istek, CancellationToken ct = default)
    {
        var sakin = await SakiniGetir(istek.SakinId, ct);
        var kayit = new IzinSureci { SakinId = sakin.Id, OnayDurumu = OnayDurumlari.Bekliyor, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.IzinSurecleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Sakin = sakin;
        return Map(kayit);
    }

    public async Task GuncelleAsync(int id, IzinSureciIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.IzinSurecleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("İzin kaydı bulunamadı.");
        if (kayit.OnayDurumu == OnayDurumlari.Onaylandi) throw new GecersizIstekHatasi("Onaylanmış kayıt düzenlenemez.");
        await SakiniGetir(istek.SakinId, ct);
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.IzinSurecleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("İzin kaydı bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task OnaylaAsync(int id, OnayIstek istek, string kim, CancellationToken ct = default)
    {
        var kayit = await _db.IzinSurecleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("İzin kaydı bulunamadı.");
        OnayUygulayici.Uygula(v => kayit.OnayDurumu = v, v => kayit.Onaylayan = v, v => kayit.OnayTarihi = v, v => kayit.OnayNotu = v, istek, kim);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<Sakin> SakiniGetir(int id, CancellationToken ct) =>
        await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");

    private static void Doldur(IzinSureci kayit, IzinSureciIstek istek)
    {
        kayit.SakinId = istek.SakinId;
        kayit.IzinTuru = istek.IzinTuru.Trim();
        kayit.BaslangicTarihi = SakinVarlikYardimcisi.ToUtc(istek.BaslangicTarihi);
        kayit.BitisTarihi = SakinVarlikYardimcisi.ToUtc(istek.BitisTarihi);
        kayit.TeslimAlan = Metin(istek.TeslimAlan);
        kayit.Notlar = Metin(istek.Notlar);
    }

    private static string? Metin(string? deger) => string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();

    private static IzinSureciDto Map(IzinSureci x) => new(
        x.Id, x.SakinId, Ad(x.Sakin), x.IzinTuru, x.BaslangicTarihi, x.BitisTarihi, x.TeslimAlan, x.Notlar,
        x.OnayDurumu, x.Onaylayan, x.OnayTarihi, x.OnayNotu);

    private static string Ad(Sakin? s) => s is null ? "" : $"{s.Ad} {s.Soyad}";
}

public class EsyaTespitServisi : IEsyaTespitServisi
{
    private readonly IUygulamaDbContext _db;
    private readonly IKurumSurecPdfUretici _pdf;
    public EsyaTespitServisi(IUygulamaDbContext db, IKurumSurecPdfUretici pdf)
    {
        _db = db;
        _pdf = pdf;
    }

    public async Task<List<EsyaTespitDto>> GetirAsync(int? sakinId, string? onayDurumu, CancellationToken ct = default)
    {
        var sorgu = _db.EsyaTespitleri.AsNoTracking().Include(x => x.Sakin).AsQueryable();
        if (sakinId is not null) sorgu = sorgu.Where(x => x.SakinId == sakinId);
        if (!string.IsNullOrWhiteSpace(onayDurumu)) sorgu = sorgu.Where(x => x.OnayDurumu == onayDurumu);
        var kayitlar = await sorgu.OrderByDescending(x => x.TespitTarihi).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<EsyaTespitDto> OlusturAsync(EsyaTespitIstek istek, CancellationToken ct = default)
    {
        var sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == istek.SakinId, ct) ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        var kayit = new EsyaTespit { SakinId = sakin.Id, OnayDurumu = OnayDurumlari.Bekliyor, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.EsyaTespitleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Sakin = sakin;
        return Map(kayit);
    }

    public async Task GuncelleAsync(int id, EsyaTespitIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.EsyaTespitleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Eşya tespiti bulunamadı.");
        if (kayit.OnayDurumu == OnayDurumlari.Onaylandi) throw new GecersizIstekHatasi("Onaylanmış kayıt düzenlenemez.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.EsyaTespitleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Eşya tespiti bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task OnaylaAsync(int id, OnayIstek istek, string kim, CancellationToken ct = default)
    {
        var kayit = await _db.EsyaTespitleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Eşya tespiti bulunamadı.");
        OnayUygulayici.Uygula(v => kayit.OnayDurumu = v, v => kayit.Onaylayan = v, v => kayit.OnayTarihi = v, v => kayit.OnayNotu = v, istek, kim);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<byte[]> PdfUretAsync(int sakinId, CancellationToken ct = default)
    {
        var sakin = await _db.Sakinler.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sakinId, ct) ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        var kayitlar = await GetirAsync(sakinId, null, ct);
        return _pdf.EsyaTespitUret($"{sakin.Ad} {sakin.Soyad}", DateTime.UtcNow, kayitlar);
    }

    private static void Doldur(EsyaTespit kayit, EsyaTespitIstek istek)
    {
        kayit.SakinId = istek.SakinId;
        kayit.Kategori = istek.Kategori.Trim();
        kayit.Adet = istek.Adet;
        kayit.Aciklama = string.IsNullOrWhiteSpace(istek.Aciklama) ? null : istek.Aciklama.Trim();
        kayit.TespitTarihi = SakinVarlikYardimcisi.ToUtc(istek.TespitTarihi);
    }

    private static EsyaTespitDto Map(EsyaTespit x) => new(
        x.Id, x.SakinId, x.Sakin is null ? "" : $"{x.Sakin.Ad} {x.Sakin.Soyad}", x.Kategori, x.Adet, x.Aciklama, x.TespitTarihi,
        x.OnayDurumu, x.Onaylayan, x.OnayTarihi, x.OnayNotu);
}

public class MirasciTeslimServisi : IMirasciTeslimServisi
{
    private readonly IUygulamaDbContext _db;
    private readonly IKurumSurecPdfUretici _pdf;
    private readonly IDosyaDepolama _dosya;

    public MirasciTeslimServisi(IUygulamaDbContext db, IKurumSurecPdfUretici pdf, IDosyaDepolama dosya)
    {
        _db = db;
        _pdf = pdf;
        _dosya = dosya;
    }

    public async Task<List<MirasciTeslimDto>> GetirAsync(int? sakinId, CancellationToken ct = default)
    {
        var sorgu = _db.MirasciTeslimleri.AsNoTracking().Include(x => x.Sakin).Include(x => x.Mirasci).AsQueryable();
        if (sakinId is not null) sorgu = sorgu.Where(x => x.SakinId == sakinId);
        var kayitlar = await sorgu.OrderByDescending(x => x.TeslimTarihi).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<MirasciTeslimDto> OlusturAsync(MirasciTeslimIstek istek, CancellationToken ct = default)
    {
        var sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == istek.SakinId, ct) ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        Mirasci? mirasci = null;
        if (istek.MirasciId is not null)
        {
            mirasci = await _db.Mirascilar.FirstOrDefaultAsync(m => m.Id == istek.MirasciId && m.SakinId == istek.SakinId, ct)
                ?? throw new KayitBulunamadiHatasi("Mirasçı bulunamadı.");
        }
        var kayit = new MirasciTeslim { SakinId = sakin.Id, OnayDurumu = OnayDurumlari.Bekliyor, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.MirasciTeslimleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Sakin = sakin;
        kayit.Mirasci = mirasci;
        return Map(kayit);
    }

    public async Task GuncelleAsync(int id, MirasciTeslimIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.MirasciTeslimleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Teslim kaydı bulunamadı.");
        if (kayit.OnayDurumu == OnayDurumlari.Onaylandi) throw new GecersizIstekHatasi("Onaylanmış kayıt düzenlenemez.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.MirasciTeslimleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Teslim kaydı bulunamadı.");
        await _dosya.SilAsync(kayit.EvrakYolu, ct);
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task OnaylaAsync(int id, OnayIstek istek, string kim, CancellationToken ct = default)
    {
        var kayit = await _db.MirasciTeslimleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Teslim kaydı bulunamadı.");
        OnayUygulayici.Uygula(v => kayit.OnayDurumu = v, v => kayit.Onaylayan = v, v => kayit.OnayTarihi = v, v => kayit.OnayNotu = v, istek, kim);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<byte[]> PdfUretAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.MirasciTeslimleri.AsNoTracking().Include(x => x.Sakin).Include(x => x.Mirasci)
            .FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Teslim kaydı bulunamadı.");
        return _pdf.TeslimTutanagiUret(Map(kayit));
    }

    public async Task EvrakYukleAsync(int id, string dosyaAdi, string icerikTipi, Stream icerik, CancellationToken ct = default)
    {
        if (icerikTipi is not ("application/pdf" or "image/jpeg" or "image/png"))
            throw new GecersizIstekHatasi("Evrak PDF, JPEG veya PNG olmalıdır.");
        var kayit = await _db.MirasciTeslimleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Teslim kaydı bulunamadı.");
        await _dosya.SilAsync(kayit.EvrakYolu, ct);
        var uzanti = Path.GetExtension(dosyaAdi);
        if (string.IsNullOrWhiteSpace(uzanti)) uzanti = icerikTipi == "application/pdf" ? ".pdf" : ".jpg";
        kayit.EvrakYolu = await _dosya.KaydetAsync($"teslim/{id}", uzanti, icerik, ct);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<KayitliDosya> EvrakGetirAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.MirasciTeslimleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Teslim kaydı bulunamadı.");
        if (string.IsNullOrWhiteSpace(kayit.EvrakYolu)) throw new KayitBulunamadiHatasi("İmzalı evrak yok.");
        return await _dosya.AcAsync(kayit.EvrakYolu, "application/octet-stream", Path.GetFileName(kayit.EvrakYolu), ct)
            ?? throw new KayitBulunamadiHatasi("Evrak dosyası bulunamadı.");
    }

    private static void Doldur(MirasciTeslim kayit, MirasciTeslimIstek istek)
    {
        kayit.SakinId = istek.SakinId;
        kayit.MirasciId = istek.MirasciId;
        kayit.TeslimAlan = istek.TeslimAlan.Trim();
        kayit.TeslimAlanTelefon = string.IsNullOrWhiteSpace(istek.TeslimAlanTelefon) ? null : istek.TeslimAlanTelefon.Trim();
        kayit.TeslimTarihi = SakinVarlikYardimcisi.ToUtc(istek.TeslimTarihi);
        kayit.EsyaOzeti = string.IsNullOrWhiteSpace(istek.EsyaOzeti) ? null : istek.EsyaOzeti.Trim();
        kayit.Notlar = string.IsNullOrWhiteSpace(istek.Notlar) ? null : istek.Notlar.Trim();
    }

    private static MirasciTeslimDto Map(MirasciTeslim x) => new(
        x.Id, x.SakinId, x.Sakin is null ? "" : $"{x.Sakin.Ad} {x.Sakin.Soyad}", x.MirasciId,
        x.Mirasci is null ? null : $"{x.Mirasci.Ad} {x.Mirasci.Soyad}", x.TeslimAlan, x.TeslimAlanTelefon, x.TeslimTarihi,
        x.EsyaOzeti, x.Notlar, !string.IsNullOrWhiteSpace(x.EvrakYolu), x.OnayDurumu, x.Onaylayan, x.OnayTarihi, x.OnayNotu);
}

public class SosyalIncelemeServisi : ISosyalIncelemeServisi
{
    private readonly IUygulamaDbContext _db;
    public SosyalIncelemeServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<SosyalIncelemeDto>> GetirAsync(string? onayDurumu, DateTime? baslangic, DateTime? bitis, string? arama, CancellationToken ct = default)
    {
        var sorgu = _db.SosyalIncelemeler.AsNoTracking().Include(x => x.Sakin).AsQueryable();
        if (!string.IsNullOrWhiteSpace(onayDurumu)) sorgu = sorgu.Where(x => x.OnayDurumu == onayDurumu);
        if (baslangic is not null) sorgu = sorgu.Where(x => x.Tarih >= SakinVarlikYardimcisi.ToUtc(baslangic.Value));
        if (bitis is not null) sorgu = sorgu.Where(x => x.Tarih < SakinVarlikYardimcisi.ToUtc(bitis.Value).AddDays(1));
        if (!string.IsNullOrWhiteSpace(arama))
        {
            var q = arama.Trim();
            sorgu = sorgu.Where(x => x.AdSoyad.Contains(q) || (x.TcKimlikNo != null && x.TcKimlikNo.Contains(q)));
        }
        var kayitlar = await sorgu.OrderByDescending(x => x.Tarih).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<SosyalIncelemeDto> OlusturAsync(SosyalIncelemeIstek istek, CancellationToken ct = default)
    {
        await SakiniKontrol(istek.SakinId, ct);
        var kayit = new SosyalInceleme { OnayDurumu = OnayDurumlari.Bekliyor, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.SosyalIncelemeler.Add(kayit);
        await _db.SaveChangesAsync(ct);
        if (kayit.SakinId is not null)
        {
            kayit.Sakin = await _db.Sakinler.AsNoTracking().FirstOrDefaultAsync(s => s.Id == kayit.SakinId, ct);
        }
        return Map(kayit);
    }

    public async Task GuncelleAsync(int id, SosyalIncelemeIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.SosyalIncelemeler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Sosyal inceleme bulunamadı.");
        if (kayit.OnayDurumu == OnayDurumlari.Onaylandi) throw new GecersizIstekHatasi("Onaylanmış kayıt düzenlenemez.");
        await SakiniKontrol(istek.SakinId, ct);
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.SosyalIncelemeler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Sosyal inceleme bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task OnaylaAsync(int id, OnayIstek istek, string kim, CancellationToken ct = default)
    {
        var kayit = await _db.SosyalIncelemeler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Sosyal inceleme bulunamadı.");
        OnayUygulayici.Uygula(v => kayit.OnayDurumu = v, v => kayit.Onaylayan = v, v => kayit.OnayTarihi = v, v => kayit.OnayNotu = v, istek, kim);
        await _db.SaveChangesAsync(ct);
    }

    private async Task SakiniKontrol(int? sakinId, CancellationToken ct)
    {
        if (sakinId is null) return;
        var varMi = await _db.Sakinler.AnyAsync(s => s.Id == sakinId, ct);
        if (!varMi) throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
    }

    private static void Doldur(SosyalInceleme kayit, SosyalIncelemeIstek istek)
    {
        kayit.SakinId = istek.SakinId;
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.Durum = istek.Durum.Trim();
        kayit.BasvuruYapan = Metin(istek.BasvuruYapan);
        kayit.TcKimlikNo = Metin(istek.TcKimlikNo);
        kayit.AdSoyad = istek.AdSoyad.Trim();
        kayit.DogumTarihi = SakinVarlikYardimcisi.ToUtc(istek.DogumTarihi);
        kayit.DogumYeri = Metin(istek.DogumYeri);
        kayit.Cinsiyet = Metin(istek.Cinsiyet);
        kayit.MedeniDurum = Metin(istek.MedeniDurum);
        kayit.EgitimDurumu = Metin(istek.EgitimDurumu);
        kayit.Telefon = Metin(istek.Telefon);
        kayit.Eposta = Metin(istek.Eposta);
        kayit.Adres = Metin(istek.Adres);
        kayit.GelirKaynagi = Metin(istek.GelirKaynagi);
        kayit.AylikGelir = Metin(istek.AylikGelir);
        kayit.SosyalGuvence = Metin(istek.SosyalGuvence);
        kayit.Mulk = Metin(istek.Mulk);
        kayit.AileUyeleri = Metin(istek.AileUyeleri);
        kayit.YakinlikDereceleri = Metin(istek.YakinlikDereceleri);
        kayit.Iletisim = Metin(istek.Iletisim);
        kayit.KronikHastaliklar = Metin(istek.KronikHastaliklar);
        kayit.KullanilanIlaclar = Metin(istek.KullanilanIlaclar);
        kayit.EngelDurumu = Metin(istek.EngelDurumu);
        kayit.BakimIhtiyaci = Metin(istek.BakimIhtiyaci);
        kayit.UzmanGorusu = Metin(istek.UzmanGorusu);
        kayit.Oneri = Metin(istek.Oneri);
        kayit.Sonuc = Metin(istek.Sonuc);
    }

    private static string? Metin(string? deger) => string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();

    private static SosyalIncelemeDto Map(SosyalInceleme x) => new(
        x.Id, x.SakinId, x.Sakin is null ? null : $"{x.Sakin.Ad} {x.Sakin.Soyad}", x.Tarih, x.Durum, x.BasvuruYapan, x.TcKimlikNo, x.AdSoyad,
        x.DogumTarihi, x.DogumYeri, x.Cinsiyet, x.MedeniDurum, x.EgitimDurumu, x.Telefon, x.Eposta, x.Adres, x.GelirKaynagi, x.AylikGelir,
        x.SosyalGuvence, x.Mulk, x.AileUyeleri, x.YakinlikDereceleri, x.Iletisim, x.KronikHastaliklar, x.KullanilanIlaclar, x.EngelDurumu,
        x.BakimIhtiyaci, x.UzmanGorusu, x.Oneri, x.Sonuc, x.OnayDurumu, x.Onaylayan, x.OnayTarihi, x.OnayNotu);
}

public class PsikolojikDegerlendirmeServisi : IPsikolojikDegerlendirmeServisi
{
    private readonly IUygulamaDbContext _db;
    public PsikolojikDegerlendirmeServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<PsikolojikDegerlendirmeDto>> GetirAsync(int? sakinId, string? tur, CancellationToken ct = default)
    {
        var sorgu = _db.PsikolojikDegerlendirmeler.AsNoTracking().Include(x => x.Sakin).AsQueryable();
        if (sakinId is not null) sorgu = sorgu.Where(x => x.SakinId == sakinId);
        if (!string.IsNullOrWhiteSpace(tur)) sorgu = sorgu.Where(x => x.Tur == tur);
        var kayitlar = await sorgu.OrderByDescending(x => x.Tarih).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<PsikolojikDegerlendirmeDto> OlusturAsync(PsikolojikDegerlendirmeIstek istek, CancellationToken ct = default)
    {
        var sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == istek.SakinId, ct) ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        var kayit = new PsikolojikDegerlendirme { SakinId = sakin.Id, OnayDurumu = OnayDurumlari.Bekliyor, OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.PsikolojikDegerlendirmeler.Add(kayit);
        await _db.SaveChangesAsync(ct);
        kayit.Sakin = sakin;
        return Map(kayit);
    }

    public async Task GuncelleAsync(int id, PsikolojikDegerlendirmeIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.PsikolojikDegerlendirmeler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Değerlendirme bulunamadı.");
        if (kayit.OnayDurumu == OnayDurumlari.Onaylandi) throw new GecersizIstekHatasi("Onaylanmış kayıt düzenlenemez.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.PsikolojikDegerlendirmeler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Değerlendirme bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task OnaylaAsync(int id, OnayIstek istek, string kim, CancellationToken ct = default)
    {
        var kayit = await _db.PsikolojikDegerlendirmeler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Değerlendirme bulunamadı.");
        OnayUygulayici.Uygula(v => kayit.OnayDurumu = v, v => kayit.Onaylayan = v, v => kayit.OnayTarihi = v, v => kayit.OnayNotu = v, istek, kim);
        await _db.SaveChangesAsync(ct);
    }

    private static void Doldur(PsikolojikDegerlendirme kayit, PsikolojikDegerlendirmeIstek istek)
    {
        kayit.SakinId = istek.SakinId;
        kayit.Tarih = SakinVarlikYardimcisi.ToUtc(istek.Tarih);
        kayit.Tur = istek.Tur;
        kayit.Puan = istek.Puan;
        kayit.SosyalDurum = string.IsNullOrWhiteSpace(istek.SosyalDurum) ? null : istek.SosyalDurum.Trim();
        kayit.UzmanGorusu = string.IsNullOrWhiteSpace(istek.UzmanGorusu) ? null : istek.UzmanGorusu.Trim();
        kayit.Oneriler = string.IsNullOrWhiteSpace(istek.Oneriler) ? null : istek.Oneriler.Trim();
    }

    private static PsikolojikDegerlendirmeDto Map(PsikolojikDegerlendirme x) => new(
        x.Id, x.SakinId, x.Sakin is null ? "" : $"{x.Sakin.Ad} {x.Sakin.Soyad}", x.Tarih, x.Tur, x.Puan, x.SosyalDurum,
        x.UzmanGorusu, x.Oneriler, x.OnayDurumu, x.Onaylayan, x.OnayTarihi, x.OnayNotu);
}
