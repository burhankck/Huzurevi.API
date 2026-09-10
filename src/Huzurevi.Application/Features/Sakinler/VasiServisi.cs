using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Sakinler;

public interface IVasiServisi
{
    Task<List<VasiDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default);
    Task<VasiDto> OlusturAsync(int sakinId, VasiOlusturIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int sakinId, int vasiId, VasiGuncelleIstek istek, CancellationToken ct = default);
    Task SilAsync(int sakinId, int vasiId, CancellationToken ct = default);
}

public class VasiServisi : IVasiServisi
{
    private readonly IUygulamaDbContext _db;

    public VasiServisi(IUygulamaDbContext db)
    {
        _db = db;
    }

    public async Task<List<VasiDto>> TumunuGetirAsync(int sakinId, CancellationToken ct = default)
    {
        await SakiniDogrulaAsync(sakinId, ct);

        var kayitlar = await _db.Vasiler
            .AsNoTracking()
            .Where(v => v.SakinId == sakinId)
            .OrderByDescending(v => v.Durum == "Aktif")
            .ThenBy(v => v.Ad)
            .ToListAsync(ct);

        return kayitlar.Select(Map).ToList();
    }

    public async Task<VasiDto> OlusturAsync(int sakinId, VasiOlusturIstek istek, CancellationToken ct = default)
    {
        await SakiniDogrulaAsync(sakinId, ct);

        var vasi = new Vasi
        {
            SakinId = sakinId,
            OlusturulmaTarihi = DateTime.UtcNow
        };
        Doldur(vasi, istek.Ad, istek.Soyad, istek.TcKimlikNo, istek.DogumTarihi, istek.Telefon, istek.Eposta,
            istek.Adres, istek.Yakinlik, istek.MahkemeAdi, istek.KararNo, istek.KararTarihi, istek.BaslangicTarihi,
            istek.BitisTarihi, istek.Kapsam, istek.VasiTuru, istek.Sebep, istek.Durum, istek.Aciklama);

        _db.Vasiler.Add(vasi);
        await _db.SaveChangesAsync(ct);
        return Map(vasi);
    }

    public async Task GuncelleAsync(int sakinId, int vasiId, VasiGuncelleIstek istek, CancellationToken ct = default)
    {
        var vasi = await _db.Vasiler.FirstOrDefaultAsync(v => v.Id == vasiId && v.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Vasi kaydı bulunamadı.");

        Doldur(vasi, istek.Ad, istek.Soyad, istek.TcKimlikNo, istek.DogumTarihi, istek.Telefon, istek.Eposta,
            istek.Adres, istek.Yakinlik, istek.MahkemeAdi, istek.KararNo, istek.KararTarihi, istek.BaslangicTarihi,
            istek.BitisTarihi, istek.Kapsam, istek.VasiTuru, istek.Sebep, istek.Durum, istek.Aciklama);
        vasi.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int sakinId, int vasiId, CancellationToken ct = default)
    {
        var vasi = await _db.Vasiler.FirstOrDefaultAsync(v => v.Id == vasiId && v.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Vasi kaydı bulunamadı.");

        var simdi = DateTime.UtcNow;
        vasi.SilindiMi = true;
        vasi.SilinmeTarihi = simdi;
        vasi.GuncellenmeTarihi = simdi;
        await _db.SaveChangesAsync(ct);
    }

    private async Task SakiniDogrulaAsync(int sakinId, CancellationToken ct)
    {
        if (!await _db.Sakinler.AnyAsync(s => s.Id == sakinId, ct))
        {
            throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        }
    }

    private static void Doldur(
        Vasi vasi,
        string ad,
        string soyad,
        string? tc,
        DateTime? dogum,
        string? telefon,
        string? eposta,
        string? adres,
        string? yakinlik,
        string? mahkeme,
        string? kararNo,
        DateTime? kararTarihi,
        DateTime? baslangic,
        DateTime? bitis,
        string? kapsam,
        string? tur,
        string? sebep,
        string? durum,
        string? aciklama)
    {
        vasi.Ad = ad.Trim();
        vasi.Soyad = soyad.Trim();
        vasi.TcKimlikNo = Metin(tc);
        vasi.DogumTarihi = ToUtc(dogum);
        vasi.Telefon = Metin(telefon);
        vasi.Eposta = Metin(eposta);
        vasi.Adres = Metin(adres);
        vasi.Yakinlik = Metin(yakinlik);
        vasi.MahkemeAdi = Metin(mahkeme);
        vasi.KararNo = Metin(kararNo);
        vasi.KararTarihi = ToUtc(kararTarihi);
        vasi.BaslangicTarihi = ToUtc(baslangic);
        vasi.BitisTarihi = ToUtc(bitis);
        vasi.Kapsam = Metin(kapsam);
        vasi.VasiTuru = string.IsNullOrWhiteSpace(tur) ? "Yasal Vasi" : tur.Trim();
        vasi.Sebep = Metin(sebep);
        vasi.Durum = string.IsNullOrWhiteSpace(durum) ? "Aktif" : durum.Trim();
        vasi.Aciklama = Metin(aciklama);
    }

    private static VasiDto Map(Vasi v) => new(
        v.Id, v.SakinId, v.Ad, v.Soyad, v.TcKimlikNo, v.DogumTarihi, v.Telefon, v.Eposta, v.Adres,
        v.Yakinlik, v.MahkemeAdi, v.KararNo, v.KararTarihi, v.BaslangicTarihi, v.BitisTarihi,
        v.Kapsam, v.VasiTuru, v.Sebep, v.Durum, v.Aciklama);

    private static string? Metin(string? deger) =>
        string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();

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
