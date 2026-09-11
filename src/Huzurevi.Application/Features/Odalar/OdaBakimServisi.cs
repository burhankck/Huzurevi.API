using FluentValidation;
using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Odalar;

public record OdaBakimDto(
    int Id,
    int OdaId,
    string OdaNumarasi,
    string BakimTuru,
    DateTime BakimTarihi,
    int? PersonelId,
    string? PersonelAdSoyad,
    int? SureDakika,
    decimal? Maliyet,
    string? Not);

public record OdaBakimIstek(
    int OdaId,
    string BakimTuru,
    DateTime BakimTarihi,
    int? PersonelId,
    int? SureDakika,
    decimal? Maliyet,
    string? Not);

public interface IOdaBakimServisi
{
    Task<List<OdaBakimDto>> ListeleAsync(int? odaId, CancellationToken ct = default);
    Task<OdaBakimDto> OlusturAsync(OdaBakimIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, OdaBakimIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
}

public class OdaBakimIstekDogrulayici : AbstractValidator<OdaBakimIstek>
{
    public static readonly string[] Turler = ["Periyodik", "Arıza", "Onarım", "Tadilat", "Temizlik", "Diğer"];

    public OdaBakimIstekDogrulayici()
    {
        RuleFor(x => x.OdaId).GreaterThan(0);
        RuleFor(x => x.BakimTuru).NotEmpty().Must(v => Turler.Contains(v)).WithMessage("Bakım türü geçersiz.");
        RuleFor(x => x.BakimTarihi).NotEmpty();
        RuleFor(x => x.SureDakika).GreaterThanOrEqualTo(0).When(x => x.SureDakika.HasValue);
        RuleFor(x => x.Maliyet).GreaterThanOrEqualTo(0).When(x => x.Maliyet.HasValue);
        RuleFor(x => x.Not).MaximumLength(500);
    }
}

public class OdaBakimServisi : IOdaBakimServisi
{
    private readonly IUygulamaDbContext _db;

    public OdaBakimServisi(IUygulamaDbContext db) => _db = db;

    public async Task<List<OdaBakimDto>> ListeleAsync(int? odaId, CancellationToken ct = default)
    {
        var sorgu = _db.OdaBakimlari.AsNoTracking().Include(x => x.Oda).Include(x => x.Personel).AsQueryable();
        if (odaId is not null)
            sorgu = sorgu.Where(x => x.OdaId == odaId);

        var kayitlar = await sorgu.OrderByDescending(x => x.BakimTarihi).ThenByDescending(x => x.Id).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<OdaBakimDto> OlusturAsync(OdaBakimIstek istek, CancellationToken ct = default)
    {
        await OdaVarMiAsync(istek.OdaId, ct);
        await PersonelKontrolAsync(istek.PersonelId, ct);

        var kayit = new OdaBakim
        {
            OdaId = istek.OdaId,
            BakimTuru = istek.BakimTuru.Trim(),
            BakimTarihi = ToUtc(istek.BakimTarihi),
            PersonelId = istek.PersonelId,
            SureDakika = istek.SureDakika,
            Maliyet = istek.Maliyet,
            Not = Metin(istek.Not),
            OlusturulmaTarihi = DateTime.UtcNow
        };

        _db.OdaBakimlari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return await GetirAsync(kayit.Id, ct);
    }

    public async Task GuncelleAsync(int id, OdaBakimIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.OdaBakimlari.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Bakım kaydı bulunamadı.");

        await OdaVarMiAsync(istek.OdaId, ct);
        await PersonelKontrolAsync(istek.PersonelId, ct);

        kayit.OdaId = istek.OdaId;
        kayit.BakimTuru = istek.BakimTuru.Trim();
        kayit.BakimTarihi = ToUtc(istek.BakimTarihi);
        kayit.PersonelId = istek.PersonelId;
        kayit.SureDakika = istek.SureDakika;
        kayit.Maliyet = istek.Maliyet;
        kayit.Not = Metin(istek.Not);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.OdaBakimlari.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Bakım kaydı bulunamadı.");

        var simdi = DateTime.UtcNow;
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = simdi;
        kayit.GuncellenmeTarihi = simdi;
        await _db.SaveChangesAsync(ct);
    }

    private async Task<OdaBakimDto> GetirAsync(int id, CancellationToken ct)
    {
        var kayit = await _db.OdaBakimlari.AsNoTracking().Include(x => x.Oda).Include(x => x.Personel)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Bakım kaydı bulunamadı.");
        return Map(kayit);
    }

    private async Task OdaVarMiAsync(int odaId, CancellationToken ct)
    {
        if (!await _db.Odalar.AnyAsync(o => o.Id == odaId, ct))
            throw new KayitBulunamadiHatasi("Oda bulunamadı.");
    }

    private async Task PersonelKontrolAsync(int? personelId, CancellationToken ct)
    {
        if (personelId is null) return;
        if (!await _db.Personeller.AnyAsync(p => p.Id == personelId, ct))
            throw new KayitBulunamadiHatasi("Personel bulunamadı.");
    }

    private static OdaBakimDto Map(OdaBakim x) => new(
        x.Id,
        x.OdaId,
        x.Oda?.OdaNumarasi ?? "",
        x.BakimTuru,
        x.BakimTarihi,
        x.PersonelId,
        x.Personel is null ? null : $"{x.Personel.Ad} {x.Personel.Soyad}",
        x.SureDakika,
        x.Maliyet,
        x.Not);

    private static DateTime ToUtc(DateTime deger) =>
        deger.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(deger, DateTimeKind.Utc)
            : deger.ToUniversalTime();

    private static string? Metin(string? deger) =>
        string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();
}
