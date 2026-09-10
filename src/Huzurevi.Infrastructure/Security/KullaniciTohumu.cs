using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Huzurevi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Infrastructure.Security;

public static class KullaniciTohumu
{
    public static async Task UygulaAsync(UygulamaDbContext db, ISifreHasher sifreHasher, CancellationToken ct = default)
    {
        var adminVarMi = await db.Kullanicilar
            .IgnoreQueryFilters()
            .AnyAsync(k => k.KullaniciAdi == "admin", ct);

        if (adminVarMi)
        {
            return;
        }

        db.Kullanicilar.Add(new Kullanici
        {
            KullaniciAdi = "admin",
            Ad = "Sistem",
            Soyad = "Yöneticisi",
            Eposta = "admin@huzurevi.local",
            SifreHash = sifreHasher.Hashle("Admin123!"),
            Rol = "Yonetici",
            AktifMi = true,
            OlusturulmaTarihi = DateTime.UtcNow,
            SifreDegistirilmeTarihi = DateTime.UtcNow
        });

        await db.SaveChangesAsync(ct);
    }
}
