using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.IlacTakipleri;

public class IlacTakipServisi : IIlacTakipServisi
{
    private readonly IUygulamaDbContext _db;

    public IlacTakipServisi(IUygulamaDbContext db)
    {
        _db = db;
    }

    public async Task<List<IlacTakipDto>> TumunuGetirAsync(CancellationToken ct = default)
    {
        var kayitlar = await _db.IlacTakipleri
            .AsNoTracking()
            .Include(i => i.Sakin)
            .OrderByDescending(i => i.Id)
            .ToListAsync(ct);

        return kayitlar.Select(Map).ToList();
    }

    public async Task<IlacTakipDto> OlusturAsync(IlacTakipOlusturIstek request, CancellationToken ct = default)
    {
        var sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == request.SakinId, ct)
            ?? throw new GecersizIstekHatasi("Seçilen sakin bulunamadı veya kurumdan ayrılmış.");

        var kayit = new IlacTakip
        {
            SakinId = sakin.Id,
            IlacAdi = request.IlacAdi.Trim(),
            Dozaj = request.Dozaj?.Trim() ?? string.Empty,
            Zaman = request.Zaman?.Trim() ?? string.Empty,
            Notlar = string.IsNullOrWhiteSpace(request.Notlar) ? null : request.Notlar.Trim(),
            VerildiMi = false,
            KayitTarihi = DateTime.UtcNow
        };

        _db.IlacTakipleri.Add(kayit);
        await _db.SaveChangesAsync(ct);

        kayit.Sakin = sakin;
        return Map(kayit);
    }

    public async Task<IlacTakipDurumDto> DurumDegistirAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.IlacTakipleri.FirstOrDefaultAsync(i => i.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("İlaç takip kaydı bulunamadı.");

        kayit.VerildiMi = !kayit.VerildiMi;
        await _db.SaveChangesAsync(ct);

        return new IlacTakipDurumDto(kayit.Id, kayit.VerildiMi);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.IlacTakipleri.FirstOrDefaultAsync(i => i.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Kayıt bulunamadı.");

        _db.IlacTakipleri.Remove(kayit);
        await _db.SaveChangesAsync(ct);
    }

    private static IlacTakipDto Map(IlacTakip i) => new(
        i.Id,
        i.SakinId,
        i.Sakin != null ? $"{i.Sakin.Ad} {i.Sakin.Soyad}" : "Bilinmiyor",
        i.IlacAdi,
        i.Dozaj,
        i.Zaman,
        i.VerildiMi,
        i.KayitTarihi,
        i.Notlar);
}
