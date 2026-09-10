using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Ayarlar;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Denetim;

public record DenetimDto(
    long Id,
    DateTime OlusturulmaTarihi,
    string Tur,
    string Islem,
    string Aciklama,
    string? KullaniciAdi,
    string? IpAdresi,
    int? DurumKodu,
    string? Yol,
    string? HataTipi,
    string? TeknikDetay);

public record DenetimYazIstek(
    string Tur,
    string Islem,
    string Aciklama,
    string? KullaniciAdi,
    int? KullaniciId,
    string? IpAdresi,
    int? DurumKodu,
    string? Yol,
    string? HataTipi,
    string? TeknikDetay);

public interface IDenetimServisi
{
    Task YazAsync(DenetimYazIstek istek, CancellationToken ct = default);
    Task<List<DenetimDto>> ListeleAsync(string? tur, DateTime? baslangic, DateTime? bitis, string? q, CancellationToken ct = default);
    Task<int> TasfiyeAsync(CancellationToken ct = default);
}

public class DenetimServisi : IDenetimServisi
{
    private readonly IUygulamaDbContext _db;
    private readonly IAyarServisi _ayar;

    public DenetimServisi(IUygulamaDbContext db, IAyarServisi ayar)
    {
        _db = db;
        _ayar = ayar;
    }

    public async Task YazAsync(DenetimYazIstek istek, CancellationToken ct = default)
    {
        _db.DenetimKayitlari.Add(new DenetimKaydi
        {
            OlusturulmaTarihi = DateTime.UtcNow,
            Tur = istek.Tur,
            Islem = Kisalt(istek.Islem, 80) ?? "",
            Aciklama = Kisalt(istek.Aciklama, 400) ?? "",
            KullaniciAdi = Kisalt(istek.KullaniciAdi, 80),
            KullaniciId = istek.KullaniciId,
            IpAdresi = Kisalt(istek.IpAdresi, 64),
            DurumKodu = istek.DurumKodu,
            Yol = Kisalt(istek.Yol, 240),
            HataTipi = Kisalt(istek.HataTipi, 120),
            TeknikDetay = Kisalt(istek.TeknikDetay, 4000)
        });
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<DenetimDto>> ListeleAsync(string? tur, DateTime? baslangic, DateTime? bitis, string? q, CancellationToken ct = default)
    {
        var sorgu = _db.DenetimKayitlari.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(tur))
            sorgu = sorgu.Where(x => x.Tur == tur);
        if (baslangic is not null)
            sorgu = sorgu.Where(x => x.OlusturulmaTarihi >= baslangic);
        if (bitis is not null)
            sorgu = sorgu.Where(x => x.OlusturulmaTarihi < bitis.Value.Date.AddDays(1));
        if (!string.IsNullOrWhiteSpace(q))
        {
            var metin = q.Trim();
            sorgu = sorgu.Where(x =>
                (x.KullaniciAdi != null && x.KullaniciAdi.Contains(metin)) ||
                x.Islem.Contains(metin) ||
                x.Aciklama.Contains(metin) ||
                (x.IpAdresi != null && x.IpAdresi.Contains(metin)) ||
                (x.Yol != null && x.Yol.Contains(metin)));
        }

        var kayitlar = await sorgu.OrderByDescending(x => x.Id).Take(300).ToListAsync(ct);
        return kayitlar.Select(x => new DenetimDto(
            x.Id, x.OlusturulmaTarihi, x.Tur, x.Islem, x.Aciklama, x.KullaniciAdi, x.IpAdresi,
            x.DurumKodu, x.Yol, x.HataTipi, x.TeknikDetay)).ToList();
    }

    public async Task<int> TasfiyeAsync(CancellationToken ct = default)
    {
        var ayar = await _ayar.GetirAsync(ct);
        var sinir = DateTime.UtcNow.AddDays(-Math.Max(ayar.LogSaklamaGun, 1));
        var eski = await _db.DenetimKayitlari.Where(x => x.OlusturulmaTarihi < sinir).ToListAsync(ct);
        if (eski.Count == 0) return 0;
        _db.DenetimKayitlari.RemoveRange(eski);
        await _db.SaveChangesAsync(ct);
        return eski.Count;
    }

    private static string? Kisalt(string? deger, int max)
    {
        if (string.IsNullOrWhiteSpace(deger)) return deger;
        var temiz = deger.Trim();
        return temiz.Length <= max ? temiz : temiz[..max];
    }
}
