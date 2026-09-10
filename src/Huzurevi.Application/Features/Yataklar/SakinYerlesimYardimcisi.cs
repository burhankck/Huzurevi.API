using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Yataklar;

internal static class SakinYerlesimYardimcisi
{
    public static async Task AciklariKapatAsync(IUygulamaDbContext db, int sakinId, DateTime cikisTarihi, CancellationToken ct)
    {
        var aciklar = await db.SakinYerlesimleri
            .Where(x => x.SakinId == sakinId && x.CikisTarihi == null)
            .ToListAsync(ct);

        foreach (var kayit in aciklar)
        {
            if (cikisTarihi.Date < kayit.GirisTarihi.Date)
            {
                throw new GecersizIstekHatasi("Çıkış tarihi, mevcut yerleşimin giriş tarihinden önce olamaz.");
            }

            kayit.CikisTarihi = cikisTarihi;
            kayit.GuncellenmeTarihi = DateTime.UtcNow;
        }
    }

    public static DateTime ToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    public static SakinYerlesim Olustur(Yatak yatak, int sakinId, DateTime girisTarihi)
    {
        return new SakinYerlesim
        {
            SakinId = sakinId,
            OdaId = yatak.OdaId,
            YatakId = yatak.Id,
            OdaNumarasi = yatak.Oda?.OdaNumarasi ?? string.Empty,
            YatakNumarasi = yatak.YatakNumarasi,
            Blok = yatak.Oda?.Blok,
            Kat = yatak.Oda?.Kat ?? 0,
            GirisTarihi = girisTarihi,
            OlusturulmaTarihi = DateTime.UtcNow
        };
    }
}
