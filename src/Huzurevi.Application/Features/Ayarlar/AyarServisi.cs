using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Ayarlar;

public interface IAyarServisi
{
    Task<UygulamaAyariDto> GetirAsync(CancellationToken ct = default);
    Task GuncelleAsync(UygulamaAyariIstek istek, CancellationToken ct = default);
    Task<UygulamaAyari> VarlikGetirAsync(CancellationToken ct = default);
    Task SifreUzunlugunuDogrulaAsync(string sifre, CancellationToken ct = default);
}

public class AyarServisi : IAyarServisi
{
    private readonly IUygulamaDbContext _db;
    public AyarServisi(IUygulamaDbContext db) => _db = db;

    public async Task<UygulamaAyariDto> GetirAsync(CancellationToken ct = default)
    {
        var kayit = await VarlikGetirAsync(ct);
        return new UygulamaAyariDto(kayit.BakimModu, kayit.SifreMinUzunluk, kayit.MaksBasarisizGiris, kayit.OturumDakika, kayit.SifreGecerlilikGun, kayit.LogSaklamaGun, kayit.YedeklemeAktifMi, kayit.YedekSaat, kayit.YedekSaklamaAdet);
    }

    public async Task GuncelleAsync(UygulamaAyariIstek istek, CancellationToken ct = default)
    {
        var kayit = await VarlikGetirAsync(ct);
        kayit.BakimModu = istek.BakimModu;
        kayit.SifreMinUzunluk = istek.SifreMinUzunluk;
        kayit.MaksBasarisizGiris = istek.MaksBasarisizGiris;
        kayit.OturumDakika = istek.OturumDakika;
        kayit.SifreGecerlilikGun = istek.SifreGecerlilikGun;
        kayit.LogSaklamaGun = istek.LogSaklamaGun;
        kayit.YedeklemeAktifMi = istek.YedeklemeAktifMi;
        kayit.YedekSaat = istek.YedekSaat;
        kayit.YedekSaklamaAdet = istek.YedekSaklamaAdet;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<UygulamaAyari> VarlikGetirAsync(CancellationToken ct = default)
    {
        var kayit = await _db.UygulamaAyarlari.FirstOrDefaultAsync(ct);
        if (kayit is not null) return kayit;
        kayit = new UygulamaAyari
        {
            Id = 1,
            BakimModu = false,
            SifreMinUzunluk = 6,
            MaksBasarisizGiris = 5,
            OturumDakika = 480,
            SifreGecerlilikGun = 0,
            LogSaklamaGun = 365,
            YedeklemeAktifMi = false,
            YedekSaat = 2,
            YedekSaklamaAdet = 14
        };
        _db.UygulamaAyarlari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return kayit;
    }

    public async Task SifreUzunlugunuDogrulaAsync(string sifre, CancellationToken ct = default)
    {
        var ayar = await VarlikGetirAsync(ct);
        if (string.IsNullOrWhiteSpace(sifre) || sifre.Length < ayar.SifreMinUzunluk)
        {
            throw new GecersizIstekHatasi($"Şifre en az {ayar.SifreMinUzunluk} karakter olmalıdır.");
        }
    }
}
