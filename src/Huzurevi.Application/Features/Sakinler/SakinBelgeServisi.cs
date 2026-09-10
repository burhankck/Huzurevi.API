using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Sakinler;

public interface ISakinBelgeServisi
{
    Task FotoYukleAsync(int sakinId, string dosyaAdi, string icerikTipi, long boyut, Stream icerik, CancellationToken ct = default);
    Task FotoSilAsync(int sakinId, CancellationToken ct = default);
    Task<KayitliDosya> FotoGetirAsync(int sakinId, CancellationToken ct = default);

    Task<List<SakinBelgeDto>> BelgeleriGetirAsync(int sakinId, string? grup, CancellationToken ct = default);
    Task<SakinBelgeDto> BelgeYukleAsync(int sakinId, BelgeYukleIstek istek, string dosyaAdi, string icerikTipi, long boyut, Stream icerik, CancellationToken ct = default);
    Task BelgeGuncelleAsync(int sakinId, int belgeId, BelgeGuncelleIstek istek, CancellationToken ct = default);
    Task BelgeSilAsync(int sakinId, int belgeId, CancellationToken ct = default);
    Task<KayitliDosya> BelgeDosyasiGetirAsync(int sakinId, int belgeId, CancellationToken ct = default);
}

public class SakinBelgeServisi : ISakinBelgeServisi
{
    private static readonly HashSet<string> FotoTipleri = ["image/jpeg", "image/png", "image/webp"];
    private static readonly HashSet<string> BelgeTipleri = ["application/pdf", "image/jpeg", "image/png"];
    private const long FotoLimit = 2 * 1024 * 1024;
    private const long BelgeLimit = 10 * 1024 * 1024;

    private readonly IUygulamaDbContext _db;
    private readonly IDosyaDepolama _dosya;

    public SakinBelgeServisi(IUygulamaDbContext db, IDosyaDepolama dosya)
    {
        _db = db;
        _dosya = dosya;
    }

    public async Task FotoYukleAsync(int sakinId, string dosyaAdi, string icerikTipi, long boyut, Stream icerik, CancellationToken ct = default)
    {
        var sakin = await SakiniGetirAsync(sakinId, ct);
        Dogrula(icerikTipi, boyut, FotoTipleri, FotoLimit, "Fotoğraf JPEG, PNG veya WebP olmalı ve 2 MB’ı geçmemelidir.");
        await _dosya.SilAsync(sakin.FotoYolu, ct);
        var yol = await _dosya.KaydetAsync($"sakinler/{sakinId}", Uzanti(dosyaAdi, icerikTipi), icerik, ct);
        sakin.FotoYolu = yol;
        sakin.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task FotoSilAsync(int sakinId, CancellationToken ct = default)
    {
        var sakin = await SakiniGetirAsync(sakinId, ct);
        await _dosya.SilAsync(sakin.FotoYolu, ct);
        sakin.FotoYolu = null;
        sakin.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<KayitliDosya> FotoGetirAsync(int sakinId, CancellationToken ct = default)
    {
        var sakin = await SakiniGetirAsync(sakinId, ct);
        if (string.IsNullOrWhiteSpace(sakin.FotoYolu))
        {
            throw new KayitBulunamadiHatasi("Bu sakine ait fotoğraf yok.");
        }

        var dosya = await _dosya.AcAsync(sakin.FotoYolu, IcerikTipi(sakin.FotoYolu), "foto" + Path.GetExtension(sakin.FotoYolu), ct);
        return dosya ?? throw new KayitBulunamadiHatasi("Fotoğraf dosyası bulunamadı.");
    }

    public async Task<List<SakinBelgeDto>> BelgeleriGetirAsync(int sakinId, string? grup, CancellationToken ct = default)
    {
        await SakiniGetirAsync(sakinId, ct);
        var sorgu = _db.SakinBelgeleri.AsNoTracking().Where(b => b.SakinId == sakinId);
        if (!string.IsNullOrWhiteSpace(grup))
        {
            if (grup == "Genel")
            {
                sorgu = sorgu.Where(b => b.Grup == "Genel" || b.Grup == "");
            }
            else
            {
                sorgu = sorgu.Where(b => b.Grup == grup);
            }
        }

        var kayitlar = await sorgu
            .OrderByDescending(b => b.AktifMi)
            .ThenByDescending(b => b.BelgeTarihi)
            .ToListAsync(ct);

        return kayitlar.Select(Map).ToList();
    }

    public async Task<SakinBelgeDto> BelgeYukleAsync(
        int sakinId,
        BelgeYukleIstek istek,
        string dosyaAdi,
        string icerikTipi,
        long boyut,
        Stream icerik,
        CancellationToken ct = default)
    {
        await SakiniGetirAsync(sakinId, ct);
        Dogrula(icerikTipi, boyut, BelgeTipleri, BelgeLimit, "Belge PDF, JPEG veya PNG olmalı ve 10 MB’ı geçmemelidir.");

        var yol = await _dosya.KaydetAsync($"sakinler/{sakinId}/belgeler", Uzanti(dosyaAdi, icerikTipi), icerik, ct);
        var belge = new SakinBelge
        {
            SakinId = sakinId,
            Grup = string.IsNullOrWhiteSpace(istek.Grup) ? "Genel" : istek.Grup.Trim(),
            BelgeTuru = istek.BelgeTuru.Trim(),
            BelgeTarihi = ToUtc(istek.BelgeTarihi),
            GecerlilikTarihi = ToUtc(istek.GecerlilikTarihi),
            Aciklama = string.IsNullOrWhiteSpace(istek.Aciklama) ? null : istek.Aciklama.Trim(),
            AktifMi = istek.AktifMi,
            OrijinalAd = Path.GetFileName(dosyaAdi),
            SaklamaYolu = yol,
            IcerikTipi = icerikTipi,
            Boyut = boyut,
            OlusturulmaTarihi = DateTime.UtcNow
        };

        _db.SakinBelgeleri.Add(belge);
        await _db.SaveChangesAsync(ct);
        return Map(belge);
    }

    public async Task BelgeGuncelleAsync(int sakinId, int belgeId, BelgeGuncelleIstek istek, CancellationToken ct = default)
    {
        var belge = await _db.SakinBelgeleri.FirstOrDefaultAsync(b => b.Id == belgeId && b.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Belge kaydı bulunamadı.");

        belge.Grup = string.IsNullOrWhiteSpace(istek.Grup) ? belge.Grup : istek.Grup.Trim();
        belge.BelgeTuru = istek.BelgeTuru.Trim();
        belge.BelgeTarihi = ToUtc(istek.BelgeTarihi);
        belge.GecerlilikTarihi = ToUtc(istek.GecerlilikTarihi);
        belge.Aciklama = string.IsNullOrWhiteSpace(istek.Aciklama) ? null : istek.Aciklama.Trim();
        belge.AktifMi = istek.AktifMi;
        belge.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task BelgeSilAsync(int sakinId, int belgeId, CancellationToken ct = default)
    {
        var belge = await _db.SakinBelgeleri.FirstOrDefaultAsync(b => b.Id == belgeId && b.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Belge kaydı bulunamadı.");

        await _dosya.SilAsync(belge.SaklamaYolu, ct);
        var simdi = DateTime.UtcNow;
        belge.SilindiMi = true;
        belge.SilinmeTarihi = simdi;
        belge.GuncellenmeTarihi = simdi;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<KayitliDosya> BelgeDosyasiGetirAsync(int sakinId, int belgeId, CancellationToken ct = default)
    {
        var belge = await _db.SakinBelgeleri.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == belgeId && b.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Belge kaydı bulunamadı.");

        var dosya = await _dosya.AcAsync(belge.SaklamaYolu, belge.IcerikTipi, belge.OrijinalAd, ct);
        return dosya ?? throw new KayitBulunamadiHatasi("Belge dosyası bulunamadı.");
    }

    private async Task<Sakin> SakiniGetirAsync(int sakinId, CancellationToken ct) =>
        await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");

    private static void Dogrula(string icerikTipi, long boyut, HashSet<string> izinli, long limit, string mesaj)
    {
        if (boyut <= 0 || boyut > limit || !izinli.Contains(icerikTipi.Split(';')[0].Trim().ToLowerInvariant()))
        {
            throw new GecersizIstekHatasi(mesaj);
        }
    }

    private static string Uzanti(string dosyaAdi, string icerikTipi)
    {
        var uzanti = Path.GetExtension(dosyaAdi);
        if (!string.IsNullOrWhiteSpace(uzanti))
        {
            return uzanti;
        }

        return icerikTipi.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "application/pdf" => ".pdf",
            _ => ".bin"
        };
    }

    private static string IcerikTipi(string yol) =>
        Path.GetExtension(yol).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };

    private static SakinBelgeDto Map(SakinBelge b) => new(
        b.Id,
        b.SakinId,
        b.Grup,
        b.BelgeTuru,
        b.BelgeTarihi,
        b.GecerlilikTarihi,
        b.Aciklama,
        b.AktifMi,
        b.OrijinalAd,
        b.IcerikTipi,
        b.Boyut,
        b.IcerikTipi == "application/pdf" || b.OrijinalAd.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase));

    private static DateTime? ToUtc(DateTime? value)
    {
        if (value is null)
        {
            return null;
        }

        return value.Value.Kind switch
        {
            DateTimeKind.Utc => value.Value,
            DateTimeKind.Local => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
        };
    }
}
