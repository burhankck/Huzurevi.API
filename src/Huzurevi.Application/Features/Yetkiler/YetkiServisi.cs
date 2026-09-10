using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Yetkiler;

public interface IYetkiServisi
{
    Task<bool> IzinVarMiAsync(string? rolKodu, string izinKodu, CancellationToken ct = default);
    Task<List<string>> IzinleriGetirAsync(string? rolKodu, CancellationToken ct = default);
    Task<string?> RolAdiAsync(string? rolKodu, CancellationToken ct = default);
}

public interface IRolServisi
{
    Task<List<RolListDto>> ListeleAsync(CancellationToken ct = default);
    Task<List<IzinTanimDto>> KatalogAsync(CancellationToken ct = default);
    Task<RolDto> GetirAsync(int id, CancellationToken ct = default);
    Task<RolDto> OlusturAsync(RolIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, RolIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
}

public class YetkiServisi : IYetkiServisi
{
    private readonly IUygulamaDbContext _db;
    public YetkiServisi(IUygulamaDbContext db) => _db = db;

    public async Task<bool> IzinVarMiAsync(string? rolKodu, string izinKodu, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(rolKodu)) return false;
        if (rolKodu == IzinKatalogu.Yonetici) return true;
        var kodlar = await IzinleriGetirAsync(rolKodu, ct);
        return kodlar.Contains(izinKodu);
    }

    public async Task<List<string>> IzinleriGetirAsync(string? rolKodu, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(rolKodu)) return [];
        if (rolKodu == IzinKatalogu.Yonetici) return IzinKatalogu.TumKodlar.ToList();
        var rol = await _db.Roller.AsNoTracking().FirstOrDefaultAsync(x => x.Kod == rolKodu && x.AktifMi, ct);
        if (rol is null) return [];
        return await _db.RolIzinleri.AsNoTracking()
            .Where(x => x.RolId == rol.Id)
            .Select(x => x.IzinKodu)
            .ToListAsync(ct);
    }

    public async Task<string?> RolAdiAsync(string? rolKodu, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(rolKodu)) return null;
        return await _db.Roller.AsNoTracking().Where(x => x.Kod == rolKodu).Select(x => x.Ad).FirstOrDefaultAsync(ct);
    }
}

public class RolServisi : IRolServisi
{
    private readonly IUygulamaDbContext _db;
    public RolServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<RolListDto>> ListeleAsync(CancellationToken ct = default) =>
        await _db.Roller.AsNoTracking().OrderByDescending(x => x.SistemRoluMu).ThenBy(x => x.Ad)
            .Select(x => new RolListDto(x.Id, x.Kod, x.Ad, x.SistemRoluMu, x.AktifMi))
            .ToListAsync(ct);

    public Task<List<IzinTanimDto>> KatalogAsync(CancellationToken ct = default) =>
        Task.FromResult(IzinKatalogu.Tum.Select(x => new IzinTanimDto(x.Kod, x.Grup, x.Islem)).ToList());

    public async Task<RolDto> GetirAsync(int id, CancellationToken ct = default)
    {
        var rol = await Kayit(id, ct);
        var izinler = rol.Kod == IzinKatalogu.Yonetici
            ? IzinKatalogu.TumKodlar.ToList()
            : await _db.RolIzinleri.Where(x => x.RolId == id).Select(x => x.IzinKodu).ToListAsync(ct);
        return Map(rol, izinler);
    }

    public async Task<RolDto> OlusturAsync(RolIstek istek, CancellationToken ct = default)
    {
        var kod = RolKodUretici.Uret(istek.Kod, istek.Ad);
        if (await _db.Roller.AnyAsync(x => x.Kod == kod, ct))
            throw new CakismaHatasi("Bu rol kodu zaten kayıtlı.");
        var rol = new Rol
        {
            Kod = kod,
            Ad = istek.Ad.Trim(),
            AktifMi = istek.AktifMi,
            SistemRoluMu = false,
            OlusturulmaTarihi = DateTime.UtcNow
        };
        _db.Roller.Add(rol);
        await _db.SaveChangesAsync(ct);
        await IzinYaz(rol, istek.Izinler, ct);
        await _db.SaveChangesAsync(ct);
        return await GetirAsync(rol.Id, ct);
    }

    public async Task GuncelleAsync(int id, RolIstek istek, CancellationToken ct = default)
    {
        var rol = await Kayit(id, ct);
        if (rol.SistemRoluMu && !string.Equals(rol.Kod, istek.Kod.Trim(), StringComparison.Ordinal))
            throw new GecersizIstekHatasi("Sistem rollerinin kodu değiştirilemez.");
        if (!rol.SistemRoluMu)
        {
            var kod = RolKodUretici.Uret(istek.Kod, istek.Ad);
            if (await _db.Roller.AnyAsync(x => x.Kod == kod && x.Id != id, ct))
                throw new CakismaHatasi("Bu rol kodu zaten kayıtlı.");
            rol.Kod = kod;
        }

        rol.Ad = istek.Ad.Trim();
        if (rol.Kod != IzinKatalogu.Yonetici)
            rol.AktifMi = istek.AktifMi;
        rol.GuncellenmeTarihi = DateTime.UtcNow;
        if (rol.Kod != IzinKatalogu.Yonetici)
            await IzinYaz(rol, istek.Izinler, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var rol = await Kayit(id, ct);
        if (rol.SistemRoluMu)
            throw new GecersizIstekHatasi("Sistem rolleri silinemez.");
        if (await _db.Kullanicilar.AnyAsync(k => k.Rol == rol.Kod, ct)
            || await _db.KullaniciKuruluslari.AnyAsync(x => x.Rol == rol.Kod, ct))
            throw new GecersizIstekHatasi("Bu role bağlı kullanıcı varken silinemez.");
        rol.SilindiMi = true;
        rol.SilinmeTarihi = DateTime.UtcNow;
        rol.AktifMi = false;
        await _db.SaveChangesAsync(ct);
    }

    private async Task IzinYaz(Rol rol, List<string>? izinler, CancellationToken ct)
    {
        var gecerli = (izinler ?? []).Where(x => IzinKatalogu.TumKodlar.Contains(x)).Distinct().ToList();
        var mevcut = await _db.RolIzinleri.Where(x => x.RolId == rol.Id).ToListAsync(ct);
        foreach (var kayit in mevcut)
        {
            if (!gecerli.Contains(kayit.IzinKodu))
            {
                kayit.SilindiMi = true;
                kayit.SilinmeTarihi = DateTime.UtcNow;
            }
        }

        foreach (var kod in gecerli)
        {
            var kayit = mevcut.FirstOrDefault(x => x.IzinKodu == kod);
            if (kayit is null)
            {
                _db.RolIzinleri.Add(new RolIzin
                {
                    RolId = rol.Id,
                    IzinKodu = kod,
                    OlusturulmaTarihi = DateTime.UtcNow
                });
            }
            else
            {
                kayit.SilindiMi = false;
                kayit.SilinmeTarihi = null;
            }
        }
    }

    private async Task<Rol> Kayit(int id, CancellationToken ct) =>
        await _db.Roller.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Rol bulunamadı.");

    private static RolDto Map(Rol rol, List<string> izinler) =>
        new(rol.Id, rol.Kod, rol.Ad, rol.SistemRoluMu, rol.AktifMi, izinler);
}
