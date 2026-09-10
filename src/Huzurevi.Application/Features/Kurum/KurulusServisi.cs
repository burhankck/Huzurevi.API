using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Kurum;

public interface IKurulusServisi
{
    Task<List<KurulusDto>> ListeleAsync(CancellationToken ct = default);
    Task<KurulusDto> OlusturAsync(KurulusIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, KurulusIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task<AdresVarsayilanDto> AdresVarsayilanAsync(int? kurulusId, CancellationToken ct = default);
}

public class KurulusServisi : IKurulusServisi
{
    private readonly IUygulamaDbContext _db;
    public KurulusServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<KurulusDto>> ListeleAsync(CancellationToken ct = default)
    {
        var kayitlar = await _db.Kuruluslar.AsNoTracking().OrderBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<KurulusDto> OlusturAsync(KurulusIstek istek, CancellationToken ct = default)
    {
        var kayit = new Kurulus { OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek);
        _db.Kuruluslar.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return Map(kayit);
    }

    public async Task GuncelleAsync(int id, KurulusIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.Kuruluslar.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Kuruluş bulunamadı.");
        Doldur(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.Kuruluslar.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Kuruluş bulunamadı.");
        var uye = await _db.KullaniciKuruluslari.AnyAsync(x => x.KurulusId == id, ct);
        if (uye) throw new GecersizIstekHatasi("Bu kuruluşa bağlı kullanıcı varken silinemez.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<AdresVarsayilanDto> AdresVarsayilanAsync(int? kurulusId, CancellationToken ct = default)
    {
        var kurulus = kurulusId is null
            ? await _db.Kuruluslar.AsNoTracking().Where(x => x.AktifMi).OrderBy(x => x.Id).FirstOrDefaultAsync(ct)
            : await _db.Kuruluslar.AsNoTracking().FirstOrDefaultAsync(x => x.Id == kurulusId, ct);
        if (kurulus is null) return new AdresVarsayilanDto(null, null, null, null, "");
        var il = await _db.Iller.AsNoTracking().Include(x => x.Ulke).FirstOrDefaultAsync(x => x.PlakaKodu == kurulus.PlakaKodu, ct);
        return new AdresVarsayilanDto(il?.UlkeId, il?.Ulke?.Ad, il?.Id, il?.Ad, kurulus.PlakaKodu);
    }

    private static void Doldur(Kurulus kayit, KurulusIstek istek)
    {
        kayit.Ad = istek.Ad.Trim();
        kayit.KisaAd = istek.KisaAd.Trim();
        kayit.PlakaKodu = istek.PlakaKodu.Trim();
        kayit.Adres = string.IsNullOrWhiteSpace(istek.Adres) ? null : istek.Adres.Trim();
        kayit.Telefon = string.IsNullOrWhiteSpace(istek.Telefon) ? null : istek.Telefon.Trim();
        kayit.Dahili = string.IsNullOrWhiteSpace(istek.Dahili) ? null : istek.Dahili.Trim();
        kayit.AktifMi = istek.AktifMi;
    }

    private static KurulusDto Map(Kurulus x) => new(x.Id, x.Ad, x.KisaAd, x.PlakaKodu, x.Adres, x.Telefon, x.Dahili, x.AktifMi);
}
