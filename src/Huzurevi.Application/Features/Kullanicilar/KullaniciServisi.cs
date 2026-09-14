using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Ayarlar;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Kullanicilar;

public class KullaniciServisi : IKullaniciServisi
{
    private readonly IUygulamaDbContext _db;
    private readonly ISifreHasher _sifreHasher;
    private readonly IAyarServisi _ayar;
    private readonly IOturumBaglami _oturum;

    public KullaniciServisi(IUygulamaDbContext db, ISifreHasher sifreHasher, IAyarServisi ayar, IOturumBaglami oturum)
    {
        _db = db;
        _sifreHasher = sifreHasher;
        _ayar = ayar;
        _oturum = oturum;
    }

    public async Task<List<KullaniciListDto>> TumunuGetirAsync(CancellationToken ct = default)
    {
        var kayitlar = await _db.Kullanicilar
            .AsNoTracking()
            .Include(k => k.Personel)
            .OrderBy(k => k.Ad)
            .ThenBy(k => k.Soyad)
            .ToListAsync(ct);
        var uyeler = await _db.KullaniciKuruluslari.AsNoTracking().Include(x => x.Kurulus).ToListAsync(ct);
        if (!_oturum.YoneticiMi && _oturum.KurulusId is int kid)
        {
            var izinli = uyeler.Where(u => u.KurulusId == kid).Select(u => u.KullaniciId).ToHashSet();
            kayitlar = kayitlar.Where(k => izinli.Contains(k.Id)).ToList();
            uyeler = uyeler.Where(u => u.KurulusId == kid).ToList();
        }
        return kayitlar.Select(k => Map(k, uyeler.Where(u => u.KullaniciId == k.Id).ToList())).ToList();
    }

    public async Task<KullaniciListDto> GetirAsync(int id, CancellationToken ct = default)
    {
        var kullanici = await _db.Kullanicilar.AsNoTracking().Include(k => k.Personel).FirstOrDefaultAsync(k => k.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Kullanıcı bulunamadı.");
        var uyeler = await _db.KullaniciKuruluslari.AsNoTracking().Include(x => x.Kurulus).Where(x => x.KullaniciId == id).ToListAsync(ct);
        if (!_oturum.YoneticiMi && _oturum.KurulusId is int kid && uyeler.All(u => u.KurulusId != kid))
            throw new ErisimEngellendiHatasi("Bu kuruluş verisine erişim yetkiniz yok.");
        return Map(kullanici, uyeler);
    }

    public async Task<KullaniciListDto> OlusturAsync(KullaniciOlusturIstek istek, CancellationToken ct = default)
    {
        var kullaniciAdi = istek.KullaniciAdi.Trim();
        var eposta = istek.Eposta.Trim().ToLowerInvariant();

        if (await _db.Kullanicilar.AnyAsync(k => k.KullaniciAdi == kullaniciAdi, ct))
        {
            throw new CakismaHatasi("Bu kullanıcı adı zaten kayıtlı.");
        }

        if (await _db.Kullanicilar.AnyAsync(k => k.Eposta == eposta, ct))
        {
            throw new CakismaHatasi("Bu e-posta adresi zaten kayıtlı.");
        }

        await _ayar.SifreUzunlugunuDogrulaAsync(istek.Sifre, ct);
        await RolDogrula(istek.Rol, ct);

        var kullanici = new Kullanici
        {
            KullaniciAdi = kullaniciAdi,
            Ad = istek.Ad.Trim(),
            Soyad = istek.Soyad.Trim(),
            Eposta = eposta,
            Telefon = string.IsNullOrWhiteSpace(istek.Telefon) ? null : istek.Telefon.Trim(),
            TcKimlikNo = string.IsNullOrWhiteSpace(istek.TcKimlikNo) ? null : istek.TcKimlikNo.Trim(),
            PersonelId = istek.PersonelId,
            Rol = istek.Rol.Trim(),
            SifreHash = _sifreHasher.Hashle(istek.Sifre),
            AktifMi = true,
            OlusturulmaTarihi = DateTime.UtcNow,
            SifreDegistirilmeTarihi = DateTime.UtcNow
        };

        _db.Kullanicilar.Add(kullanici);
        await _db.SaveChangesAsync(ct);
        await UyelikleriYaz(kullanici.Id, istek.Uyelikler, istek.Rol, ct);
        await _db.SaveChangesAsync(ct);
        return await GetirAsync(kullanici.Id, ct);
    }

    public async Task GuncelleAsync(int id, KullaniciGuncelleIstek istek, int islemYapanId, CancellationToken ct = default)
    {
        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(k => k.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Kullanıcı bulunamadı.");

        var eposta = istek.Eposta.Trim().ToLowerInvariant();
        if (await _db.Kullanicilar.AnyAsync(k => k.Eposta == eposta && k.Id != id, ct))
        {
            throw new CakismaHatasi("Bu e-posta adresi zaten kayıtlı.");
        }

        if (kullanici.Rol == "Yonetici" && istek.Rol != "Yonetici")
        {
            await SonYoneticiyiKorunAsync(id, ct);
        }

        await RolDogrula(istek.Rol, ct);

        if (id == islemYapanId && !istek.AktifMi)
        {
            throw new GecersizIstekHatasi("Kendi hesabınızı pasif yapamazsınız.");
        }

        if (kullanici.AktifMi && !istek.AktifMi && kullanici.Rol == "Yonetici")
        {
            await SonYoneticiyiKorunAsync(id, ct);
        }

        kullanici.Ad = istek.Ad.Trim();
        kullanici.Soyad = istek.Soyad.Trim();
        kullanici.Eposta = eposta;
        kullanici.Telefon = string.IsNullOrWhiteSpace(istek.Telefon) ? null : istek.Telefon.Trim();
        kullanici.TcKimlikNo = string.IsNullOrWhiteSpace(istek.TcKimlikNo) ? null : istek.TcKimlikNo.Trim();
        kullanici.PersonelId = istek.PersonelId;
        kullanici.Rol = istek.Rol.Trim();
        kullanici.AktifMi = istek.AktifMi;
        kullanici.GuncellenmeTarihi = DateTime.UtcNow;
        await UyelikleriYaz(id, istek.Uyelikler, istek.Rol, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SifreAtaAsync(int id, KullaniciSifreIstek istek, CancellationToken ct = default)
    {
        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(k => k.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Kullanıcı bulunamadı.");

        await _ayar.SifreUzunlugunuDogrulaAsync(istek.YeniSifre, ct);
        kullanici.SifreHash = _sifreHasher.Hashle(istek.YeniSifre);
        kullanici.SifreDegistirilmeTarihi = DateTime.UtcNow;
        kullanici.SifreSifirlamaTokenHash = null;
        kullanici.SifreSifirlamaBitis = null;
        kullanici.BasarisizGirisSayisi = 0;
        kullanici.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task KilidiAcAsync(int id, CancellationToken ct = default)
    {
        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(k => k.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Kullanıcı bulunamadı.");

        kullanici.BasarisizGirisSayisi = 0;
        kullanici.AktifMi = true;
        kullanici.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, int islemYapanId, CancellationToken ct = default)
    {
        if (id == islemYapanId)
        {
            throw new GecersizIstekHatasi("Kendi hesabınızı silemezsiniz.");
        }

        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(k => k.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Kullanıcı bulunamadı.");

        if (kullanici.Rol == "Yonetici")
        {
            await SonYoneticiyiKorunAsync(id, ct);
        }

        var simdi = DateTime.UtcNow;
        kullanici.SilindiMi = true;
        kullanici.SilinmeTarihi = simdi;
        kullanici.AktifMi = false;
        kullanici.GuncellenmeTarihi = simdi;
        await _db.SaveChangesAsync(ct);
    }

    private async Task RolDogrula(string rol, CancellationToken ct)
    {
        if (!await _db.Roller.AnyAsync(x => x.Kod == rol.Trim() && x.AktifMi, ct))
            throw new GecersizIstekHatasi("Rol bulunamadı veya pasif.");
    }

    private async Task SonYoneticiyiKorunAsync(int haricId, CancellationToken ct)
    {
        var baskaYoneticiVarMi = await _db.Kullanicilar.AnyAsync(
            k => k.Id != haricId && k.Rol == "Yonetici" && k.AktifMi,
            ct);

        if (!baskaYoneticiVarMi)
        {
            throw new GecersizIstekHatasi("Sistemde en az bir aktif yönetici kalmalıdır.");
        }
    }

    private async Task UyelikleriYaz(int kullaniciId, List<KullaniciUyelikIstek>? uyeler, string varsayilanRol, CancellationToken ct)
    {
        var liste = uyeler?.Where(x => x.KurulusId > 0).ToList() ?? [];
        if (liste.Count == 0)
        {
            var kurulusId = _oturum.KurulusId
                ?? (await _db.Kuruluslar.OrderBy(x => x.Id).Select(x => (int?)x.Id).FirstOrDefaultAsync(ct));
            if (kurulusId is null)
                throw new GecersizIstekHatasi("Önce bir kuruluş tanımlayın.");
            liste = [new KullaniciUyelikIstek(kurulusId.Value, varsayilanRol, true)];
        }

        var mevcut = await _db.KullaniciKuruluslari.Where(x => x.KullaniciId == kullaniciId).ToListAsync(ct);
        foreach (var kayit in mevcut)
        {
            if (!liste.Any(x => x.KurulusId == kayit.KurulusId))
            {
                kayit.SilindiMi = true;
                kayit.SilinmeTarihi = DateTime.UtcNow;
            }
        }

        foreach (var uye in liste)
        {
            _oturum.KurulusDogrula(uye.KurulusId);
            if (!await _db.Kuruluslar.AnyAsync(x => x.Id == uye.KurulusId, ct))
            {
                throw new KayitBulunamadiHatasi("Kuruluş bulunamadı.");
            }

            var uyeRol = string.IsNullOrWhiteSpace(uye.Rol) ? varsayilanRol : uye.Rol;
            await RolDogrula(uyeRol, ct);

            var kayit = mevcut.FirstOrDefault(x => x.KurulusId == uye.KurulusId);
            if (kayit is null)
            {
                _db.KullaniciKuruluslari.Add(new KullaniciKurulus
                {
                    KullaniciId = kullaniciId,
                    KurulusId = uye.KurulusId,
                    Rol = string.IsNullOrWhiteSpace(uye.Rol) ? varsayilanRol : uye.Rol,
                    AktifMi = uye.AktifMi,
                    OlusturulmaTarihi = DateTime.UtcNow
                });
            }
            else
            {
                kayit.SilindiMi = false;
                kayit.SilinmeTarihi = null;
                kayit.Rol = string.IsNullOrWhiteSpace(uye.Rol) ? varsayilanRol : uye.Rol;
                kayit.AktifMi = uye.AktifMi;
                kayit.GuncellenmeTarihi = DateTime.UtcNow;
            }
        }
    }

    private static KullaniciListDto Map(Kullanici k, List<KullaniciKurulus> uyeler) => new(
        k.Id,
        k.KullaniciAdi,
        k.Ad,
        k.Soyad,
        k.Eposta,
        k.Telefon,
        k.TcKimlikNo,
        k.PersonelId,
        k.Personel is null ? null : $"{k.Personel.Ad} {k.Personel.Soyad}",
        k.Rol,
        k.AktifMi,
        k.BasarisizGirisSayisi,
        k.SonGirisTarihi,
        uyeler.Select(u => new KullaniciUyelikDto(u.KurulusId, u.Kurulus?.Ad ?? "", u.Rol, u.AktifMi)).ToList());
}
