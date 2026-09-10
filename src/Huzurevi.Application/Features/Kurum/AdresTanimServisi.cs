using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Kurum;

public interface IAdresTanimServisi
{
    Task<List<UlkeDto>> UlkelerAsync(CancellationToken ct = default);
    Task<UlkeDto> UlkeOlusturAsync(UlkeIstek istek, CancellationToken ct = default);
    Task UlkeGuncelleAsync(int id, UlkeIstek istek, CancellationToken ct = default);
    Task UlkeSilAsync(int id, CancellationToken ct = default);

    Task<List<IlDto>> IllerAsync(int? ulkeId, CancellationToken ct = default);
    Task<IlDto> IlOlusturAsync(IlIstek istek, CancellationToken ct = default);
    Task IlGuncelleAsync(int id, IlIstek istek, CancellationToken ct = default);
    Task IlSilAsync(int id, CancellationToken ct = default);

    Task<List<IlceDto>> IlcelerAsync(int? ilId, CancellationToken ct = default);
    Task<IlceDto> IlceOlusturAsync(IlceIstek istek, CancellationToken ct = default);
    Task IlceGuncelleAsync(int id, IlceIstek istek, CancellationToken ct = default);
    Task IlceSilAsync(int id, CancellationToken ct = default);

    Task<List<MahalleDto>> MahallelerAsync(int? ilceId, CancellationToken ct = default);
    Task<MahalleDto> MahalleOlusturAsync(MahalleIstek istek, CancellationToken ct = default);
    Task MahalleGuncelleAsync(int id, MahalleIstek istek, CancellationToken ct = default);
    Task MahalleSilAsync(int id, CancellationToken ct = default);

    Task<List<GlobalTanimDto>> TanimlarAsync(string? kategori, CancellationToken ct = default);
    Task<GlobalTanimDto> TanimOlusturAsync(GlobalTanimIstek istek, CancellationToken ct = default);
    Task TanimGuncelleAsync(int id, GlobalTanimIstek istek, CancellationToken ct = default);
    Task TanimSilAsync(int id, CancellationToken ct = default);
}

public class AdresTanimServisi : IAdresTanimServisi
{
    private readonly IUygulamaDbContext _db;
    public AdresTanimServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<UlkeDto>> UlkelerAsync(CancellationToken ct = default) =>
        (await _db.Ulkeler.AsNoTracking().OrderBy(x => x.Ad).ToListAsync(ct)).Select(x => new UlkeDto(x.Id, x.Ad, x.Kod)).ToList();

    public async Task<UlkeDto> UlkeOlusturAsync(UlkeIstek istek, CancellationToken ct = default)
    {
        var kayit = new Ulke { Ad = istek.Ad.Trim(), Kod = istek.Kod.Trim().ToUpperInvariant(), OlusturulmaTarihi = DateTime.UtcNow };
        _db.Ulkeler.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return new UlkeDto(kayit.Id, kayit.Ad, kayit.Kod);
    }

    public async Task UlkeGuncelleAsync(int id, UlkeIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.Ulkeler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Ülke bulunamadı.");
        kayit.Ad = istek.Ad.Trim();
        kayit.Kod = istek.Kod.Trim().ToUpperInvariant();
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task UlkeSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.Ulkeler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Ülke bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<IlDto>> IllerAsync(int? ulkeId, CancellationToken ct = default)
    {
        var sorgu = _db.Iller.AsNoTracking().Include(x => x.Ulke).AsQueryable();
        if (ulkeId is not null) sorgu = sorgu.Where(x => x.UlkeId == ulkeId);
        var kayitlar = await sorgu.OrderBy(x => x.PlakaKodu).ToListAsync(ct);
        return kayitlar.Select(x => new IlDto(x.Id, x.UlkeId, x.Ulke?.Ad ?? "", x.Ad, x.PlakaKodu)).ToList();
    }

    public async Task<IlDto> IlOlusturAsync(IlIstek istek, CancellationToken ct = default)
    {
        await UlkeVar(istek.UlkeId, ct);
        var kayit = new Il { UlkeId = istek.UlkeId, Ad = istek.Ad.Trim(), PlakaKodu = istek.PlakaKodu.Trim(), OlusturulmaTarihi = DateTime.UtcNow };
        _db.Iller.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return (await IllerAsync(null, ct)).First(x => x.Id == kayit.Id);
    }

    public async Task IlGuncelleAsync(int id, IlIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.Iller.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("İl bulunamadı.");
        await UlkeVar(istek.UlkeId, ct);
        kayit.UlkeId = istek.UlkeId;
        kayit.Ad = istek.Ad.Trim();
        kayit.PlakaKodu = istek.PlakaKodu.Trim();
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task IlSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.Iller.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("İl bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<IlceDto>> IlcelerAsync(int? ilId, CancellationToken ct = default)
    {
        var sorgu = _db.Ilceler.AsNoTracking().Include(x => x.Il).AsQueryable();
        if (ilId is not null) sorgu = sorgu.Where(x => x.IlId == ilId);
        var kayitlar = await sorgu.OrderBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(x => new IlceDto(x.Id, x.IlId, x.Il?.Ad ?? "", x.Ad)).ToList();
    }

    public async Task<IlceDto> IlceOlusturAsync(IlceIstek istek, CancellationToken ct = default)
    {
        if (!await _db.Iller.AnyAsync(x => x.Id == istek.IlId, ct)) throw new KayitBulunamadiHatasi("İl bulunamadı.");
        var kayit = new Ilce { IlId = istek.IlId, Ad = istek.Ad.Trim(), OlusturulmaTarihi = DateTime.UtcNow };
        _db.Ilceler.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return (await IlcelerAsync(null, ct)).First(x => x.Id == kayit.Id);
    }

    public async Task IlceGuncelleAsync(int id, IlceIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.Ilceler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("İlçe bulunamadı.");
        kayit.IlId = istek.IlId;
        kayit.Ad = istek.Ad.Trim();
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task IlceSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.Ilceler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("İlçe bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<MahalleDto>> MahallelerAsync(int? ilceId, CancellationToken ct = default)
    {
        var sorgu = _db.Mahalleler.AsNoTracking().Include(x => x.Ilce).AsQueryable();
        if (ilceId is not null) sorgu = sorgu.Where(x => x.IlceId == ilceId);
        var kayitlar = await sorgu.OrderBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(x => new MahalleDto(x.Id, x.IlceId, x.Ilce?.Ad ?? "", x.Ad)).ToList();
    }

    public async Task<MahalleDto> MahalleOlusturAsync(MahalleIstek istek, CancellationToken ct = default)
    {
        if (!await _db.Ilceler.AnyAsync(x => x.Id == istek.IlceId, ct)) throw new KayitBulunamadiHatasi("İlçe bulunamadı.");
        var kayit = new Mahalle { IlceId = istek.IlceId, Ad = istek.Ad.Trim(), OlusturulmaTarihi = DateTime.UtcNow };
        _db.Mahalleler.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return (await MahallelerAsync(null, ct)).First(x => x.Id == kayit.Id);
    }

    public async Task MahalleGuncelleAsync(int id, MahalleIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.Mahalleler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Mahalle bulunamadı.");
        kayit.IlceId = istek.IlceId;
        kayit.Ad = istek.Ad.Trim();
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task MahalleSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.Mahalleler.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Mahalle bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<GlobalTanimDto>> TanimlarAsync(string? kategori, CancellationToken ct = default)
    {
        var sorgu = _db.GlobalTanimlar.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(kategori)) sorgu = sorgu.Where(x => x.Kategori == kategori);
        var kayitlar = await sorgu.OrderBy(x => x.Kategori).ThenBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(x => new GlobalTanimDto(x.Id, x.Kategori, x.Ad, x.Kod, x.AktifMi)).ToList();
    }

    public async Task<GlobalTanimDto> TanimOlusturAsync(GlobalTanimIstek istek, CancellationToken ct = default)
    {
        var kayit = new GlobalTanim { Kategori = istek.Kategori, Ad = istek.Ad.Trim(), Kod = string.IsNullOrWhiteSpace(istek.Kod) ? null : istek.Kod.Trim(), AktifMi = istek.AktifMi, OlusturulmaTarihi = DateTime.UtcNow };
        _db.GlobalTanimlar.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return new GlobalTanimDto(kayit.Id, kayit.Kategori, kayit.Ad, kayit.Kod, kayit.AktifMi);
    }

    public async Task TanimGuncelleAsync(int id, GlobalTanimIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.GlobalTanimlar.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Tanım bulunamadı.");
        kayit.Kategori = istek.Kategori;
        kayit.Ad = istek.Ad.Trim();
        kayit.Kod = string.IsNullOrWhiteSpace(istek.Kod) ? null : istek.Kod.Trim();
        kayit.AktifMi = istek.AktifMi;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task TanimSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.GlobalTanimlar.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Tanım bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private async Task UlkeVar(int id, CancellationToken ct)
    {
        if (!await _db.Ulkeler.AnyAsync(x => x.Id == id, ct)) throw new KayitBulunamadiHatasi("Ülke bulunamadı.");
    }
}
