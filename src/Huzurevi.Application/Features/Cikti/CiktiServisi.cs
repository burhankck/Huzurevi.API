using FluentValidation;
using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Odalar;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.Application.Features.Ziyaretler;
using Huzurevi.Application.Features.Kurum;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Cikti;

public record PdfSablonDto(int Id, string Kod, string Ad, string Baslik, string Icerik, bool AktifMi);
public record PdfSablonIstek(string Kod, string Ad, string Baslik, string Icerik, bool AktifMi);
public record PdfUretIstek(int? SakinId);
public record PdfArsivDto(int Id, string SablonAd, string DosyaAdi, string Olusturan, DateTime OlusturulmaTarihi, string? SakinAd);

public class PdfSablonIstekDogrulayici : AbstractValidator<PdfSablonIstek>
{
    public PdfSablonIstekDogrulayici()
    {
        RuleFor(x => x.Kod).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Baslik).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Icerik).NotEmpty();
    }
}

public interface ICiktiServisi
{
    Task<TabloCikti> SakinAsync(string format, string? q, CancellationToken ct = default);
    Task<TabloCikti> OdaAsync(string format, string? blok, int? kat, string? durum, CancellationToken ct = default);
    Task<TabloCikti> ZiyaretAsync(string format, bool? iceride, CancellationToken ct = default);
    Task<TabloCikti> PersonelAsync(string format, CancellationToken ct = default);
}

public interface IPdfSablonServisi
{
    Task<List<PdfSablonDto>> ListeleAsync(CancellationToken ct = default);
    Task<PdfSablonDto> GetirAsync(int id, CancellationToken ct = default);
    Task<PdfSablonDto> OlusturAsync(PdfSablonIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, PdfSablonIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task<TabloCikti> UretVeArsivleAsync(int sablonId, int? sakinId, string olusturan, CancellationToken ct = default);
    Task<List<PdfArsivDto>> ArsivAsync(CancellationToken ct = default);
    Task<KayitliDosya> ArsivIndirAsync(int id, CancellationToken ct = default);
}

public class CiktiServisi : ICiktiServisi
{
    private readonly ISakinServisi _sakin;
    private readonly IOdaServisi _oda;
    private readonly IZiyaretServisi _ziyaret;
    private readonly IPersonelServisi _personel;
    private readonly ITabloCiktiUretici _uretici;

    public CiktiServisi(ISakinServisi sakin, IOdaServisi oda, IZiyaretServisi ziyaret, IPersonelServisi personel, ITabloCiktiUretici uretici)
    {
        _sakin = sakin;
        _oda = oda;
        _ziyaret = ziyaret;
        _personel = personel;
        _uretici = uretici;
    }

    public async Task<TabloCikti> SakinAsync(string format, string? q, CancellationToken ct = default)
    {
        var kayitlar = await _sakin.TumunuGetirAsync(ct);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var metin = q.Trim().ToLowerInvariant();
            kayitlar = kayitlar.Where(s =>
                $"{s.KayitNo} {s.Ad} {s.Soyad} {s.TcKimlikNo} {s.Telefon} {s.YatakBilgisi} {s.Durum}"
                    .ToLowerInvariant().Contains(metin)).ToList();
        }

        return Uret(format, "Sakin listesi",
            ["Kayıt No", "Ad", "Soyad", "T.C.", "Telefon", "Yatak", "Durum"],
            kayitlar.Select(s => (IReadOnlyList<string>)[s.KayitNo ?? "", s.Ad, s.Soyad, s.TcKimlikNo, s.Telefon ?? "", s.YatakBilgisi ?? "", s.Durum]).ToList());
    }

    public async Task<TabloCikti> OdaAsync(string format, string? blok, int? kat, string? durum, CancellationToken ct = default)
    {
        var kayitlar = await _oda.TumunuGetirAsync(ct);
        if (!string.IsNullOrWhiteSpace(blok))
            kayitlar = kayitlar.Where(o => (o.Blok ?? "").Contains(blok, StringComparison.CurrentCultureIgnoreCase)).ToList();
        if (kat is not null)
            kayitlar = kayitlar.Where(o => o.Kat == kat).ToList();
        if (!string.IsNullOrWhiteSpace(durum))
            kayitlar = kayitlar.Where(o => o.Durum == durum).ToList();

        return Uret(format, "Oda listesi",
            ["Oda", "Blok", "Kat", "Tip", "Durum", "Kapasite", "Dolu"],
            kayitlar.Select(o => (IReadOnlyList<string>)[o.OdaNo, o.Blok, o.Kat.ToString(), o.OdaTipi ?? "", o.Durum, o.Kapasite.ToString(), $"{o.DoluYatakSayisi}/{o.YatakSayisi}"]).ToList());
    }

    public async Task<TabloCikti> ZiyaretAsync(string format, bool? iceride, CancellationToken ct = default)
    {
        var kayitlar = await _ziyaret.TumunuGetirAsync(null, iceride, ct);
        return Uret(format, iceride == true ? "İçerideki ziyaretçiler" : "Ziyaret kayıtları",
            ["Sakin", "Ziyaretçi", "T.C.", "Yakınlık", "Giriş", "Çıkış", "Durum"],
            kayitlar.Select(z => (IReadOnlyList<string>)[z.SakinAdSoyad, $"{z.ZiyaretciAd} {z.ZiyaretciSoyad}", z.TcKimlikNo ?? "", z.Yakinlik ?? "", z.GirisTarihi.ToString("dd.MM.yyyy HH:mm"), z.CikisTarihi?.ToString("dd.MM.yyyy HH:mm") ?? "", z.IcerideMi ? "İçeride" : "Çıktı"]).ToList());
    }

    public async Task<TabloCikti> PersonelAsync(string format, CancellationToken ct = default)
    {
        var kayitlar = await _personel.ListeleAsync(null, ct);
        return Uret(format, "Personel listesi",
            ["Sicil", "Ad", "Soyad", "Ünvan", "Görev", "Durum", "Kuruluş"],
            kayitlar.Select(p => (IReadOnlyList<string>)[p.SicilNo, p.Ad, p.Soyad, p.Unvan ?? "", p.Gorev ?? "", p.Durum, p.KurulusAd]).ToList());
    }

    private TabloCikti Uret(string format, string baslik, IReadOnlyList<string> kolonlar, List<IReadOnlyList<string>> satirlar) =>
        format.Equals("pdf", StringComparison.OrdinalIgnoreCase)
            ? _uretici.Pdf(baslik, kolonlar, satirlar)
            : _uretici.Excel(baslik, kolonlar, satirlar);
}

public class PdfSablonServisi : IPdfSablonServisi
{
    private readonly IUygulamaDbContext _db;
    private readonly ISakinServisi _sakin;
    private readonly ITabloCiktiUretici _uretici;
    private readonly IDosyaDepolama _dosya;

    public PdfSablonServisi(IUygulamaDbContext db, ISakinServisi sakin, ITabloCiktiUretici uretici, IDosyaDepolama dosya)
    {
        _db = db;
        _sakin = sakin;
        _uretici = uretici;
        _dosya = dosya;
    }

    public async Task<List<PdfSablonDto>> ListeleAsync(CancellationToken ct = default)
    {
        var kayitlar = await _db.PdfSablonlari.AsNoTracking().OrderBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<PdfSablonDto> GetirAsync(int id, CancellationToken ct = default) => Map(await Kayit(id, ct));

    public async Task<PdfSablonDto> OlusturAsync(PdfSablonIstek istek, CancellationToken ct = default)
    {
        var kayit = new PdfSablon { OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.PdfSablonlari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return Map(kayit);
    }

    public async Task GuncelleAsync(int id, PdfSablonIstek istek, CancellationToken ct = default)
    {
        var kayit = await Kayit(id, ct);
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await Kayit(id, ct);
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<TabloCikti> UretVeArsivleAsync(int sablonId, int? sakinId, string olusturan, CancellationToken ct = default)
    {
        var sablon = await Kayit(sablonId, ct);
        var alanlar = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Tarih"] = DateTime.Now.ToString("dd.MM.yyyy"),
            ["Saat"] = DateTime.Now.ToString("HH:mm")
        };
        SakinDetayDto? sakin = null;
        if (sakinId is not null)
        {
            sakin = await _sakin.GetirAsync(sakinId.Value, ct);
            alanlar["Ad"] = sakin.Ad;
            alanlar["Soyad"] = sakin.Soyad;
            alanlar["AdSoyad"] = $"{sakin.Ad} {sakin.Soyad}";
            alanlar["TcKimlikNo"] = sakin.TcKimlikNo;
            alanlar["Telefon"] = sakin.Telefon ?? "";
            alanlar["Durum"] = sakin.Durum;
            alanlar["KayitNo"] = sakin.KayitNo ?? "";
        }

        var govde = sablon.Icerik;
        foreach (var (k, v) in alanlar)
            govde = govde.Replace("{{" + k + "}}", v, StringComparison.OrdinalIgnoreCase);

        var pdf = _uretici.MetinPdf(sablon.Baslik, govde);
        await using var akis = new MemoryStream(pdf);
        var yol = await _dosya.KaydetAsync("pdf-arsiv", ".pdf", akis, ct);
        _db.PdfArsivleri.Add(new PdfArsiv
        {
            SablonId = sablon.Id,
            SakinId = sakinId,
            DosyaYolu = yol,
            DosyaAdi = $"{sablon.Kod}_{DateTime.Now:yyyyMMddHHmm}.pdf",
            Olusturan = olusturan,
            OlusturulmaTarihi = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(ct);
        return new TabloCikti(pdf, "application/pdf", $"{sablon.Kod}.pdf");
    }

    public async Task<List<PdfArsivDto>> ArsivAsync(CancellationToken ct = default)
    {
        var kayitlar = await _db.PdfArsivleri.AsNoTracking().Include(x => x.Sablon).Include(x => x.Sakin)
            .OrderByDescending(x => x.Id).Take(200).ToListAsync(ct);
        return kayitlar.Select(x => new PdfArsivDto(x.Id, x.Sablon?.Ad ?? "", x.DosyaAdi, x.Olusturan, x.OlusturulmaTarihi,
            x.Sakin is null ? null : $"{x.Sakin.Ad} {x.Sakin.Soyad}")).ToList();
    }

    public async Task<KayitliDosya> ArsivIndirAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.PdfArsivleri.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Arşiv kaydı bulunamadı.");
        return await _dosya.AcAsync(kayit.DosyaYolu, "application/pdf", kayit.DosyaAdi, ct)
            ?? throw new KayitBulunamadiHatasi("Dosya bulunamadı.");
    }

    private async Task<PdfSablon> Kayit(int id, CancellationToken ct) =>
        await _db.PdfSablonlari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Şablon bulunamadı.");

    private static void Doldur(PdfSablon kayit, PdfSablonIstek istek)
    {
        kayit.Kod = istek.Kod.Trim();
        kayit.Ad = istek.Ad.Trim();
        kayit.Baslik = istek.Baslik.Trim();
        kayit.Icerik = istek.Icerik;
        kayit.AktifMi = istek.AktifMi;
    }

    private static PdfSablonDto Map(PdfSablon x) => new(x.Id, x.Kod, x.Ad, x.Baslik, x.Icerik, x.AktifMi);
}
