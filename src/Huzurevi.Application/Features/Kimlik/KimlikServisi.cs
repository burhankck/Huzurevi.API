using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Ayarlar;
using Huzurevi.Application.Features.Kurum;
using Huzurevi.Application.Features.Yetkiler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Huzurevi.Application.Features.Kimlik;

public class KimlikServisi : IKimlikServisi
{
    private readonly IUygulamaDbContext _db;
    private readonly ISifreHasher _sifreHasher;
    private readonly ITokenUretici _tokenUretici;
    private readonly IAyarServisi _ayar;
    private readonly IYetkiServisi _yetki;
    private readonly ILogger<KimlikServisi> _gunluk;

    public KimlikServisi(
        IUygulamaDbContext db,
        ISifreHasher sifreHasher,
        ITokenUretici tokenUretici,
        IAyarServisi ayar,
        IYetkiServisi yetki,
        ILogger<KimlikServisi> gunluk)
    {
        _db = db;
        _sifreHasher = sifreHasher;
        _tokenUretici = tokenUretici;
        _ayar = ayar;
        _yetki = yetki;
        _gunluk = gunluk;
    }

    public async Task<GirisSonuc> GirisYapAsync(GirisIstek istek, CancellationToken ct = default)
    {
        var kullaniciAdi = istek.KullaniciAdi.Trim();
        var kullanici = await _db.Kullanicilar
            .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi, ct);

        if (kullanici is null)
        {
            throw new YetkisizHatasi("Kullanıcı adı veya şifre hatalı.");
        }

        var ayar = await _ayar.VarlikGetirAsync(ct);
        var uyeler = await UyeleriGetir(kullanici.Id, ct);
        var yoneticiMi = kullanici.Rol == "Yonetici" || uyeler.Any(x => x.Rol == "Yonetici");
        if (ayar.BakimModu && !yoneticiMi)
        {
            throw new BakimModuHatasi("Sistem bakımda. Yalnızca yöneticiler giriş yapabilir.");
        }

        if (!kullanici.AktifMi)
        {
            throw new YetkisizHatasi("Hesabınız pasif duruma alınmış. Yönetici ile iletişime geçin.");
        }

        if (kullanici.BasarisizGirisSayisi >= ayar.MaksBasarisizGiris)
        {
            throw new HesapKilitliHatasi("Hesap çok fazla hatalı deneme nedeniyle kilitlendi. Yönetici ile iletişime geçin.");
        }

        if (!_sifreHasher.Dogrula(istek.Sifre, kullanici.SifreHash))
        {
            kullanici.BasarisizGirisSayisi += 1;
            kullanici.GuncellenmeTarihi = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            throw new YetkisizHatasi("Kullanıcı adı veya şifre hatalı.");
        }

        if (ayar.SifreGecerlilikGun > 0)
        {
            var son = kullanici.SifreDegistirilmeTarihi ?? kullanici.OlusturulmaTarihi;
            if (son.AddDays(ayar.SifreGecerlilikGun) < DateTime.UtcNow)
            {
                throw new GecersizIstekHatasi("Şifre süresi doldu. Şifremi unuttum ile yenileyin veya yöneticiden şifre isteyin.");
            }
        }

        kullanici.BasarisizGirisSayisi = 0;
        kullanici.SonGirisTarihi = DateTime.UtcNow;
        kullanici.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return await OturumUret(kullanici, null, ayar.OturumDakika, ct);
    }

    public async Task<KullaniciOzetDto> BeniGetirAsync(int kullaniciId, int? kurulusId, CancellationToken ct = default)
    {
        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(k => k.Id == kullaniciId, ct)
            ?? throw new KayitBulunamadiHatasi("Oturum bilgisi bulunamadı.");

        return await Ozet(kullanici, kurulusId, ct);
    }

    public async Task<GirisSonuc> KurulusSecAsync(int kullaniciId, int kurulusId, CancellationToken ct = default)
    {
        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(k => k.Id == kullaniciId, ct)
            ?? throw new KayitBulunamadiHatasi("Oturum bilgisi bulunamadı.");
        var ayar = await _ayar.VarlikGetirAsync(ct);
        return await OturumUret(kullanici, kurulusId, ayar.OturumDakika, ct);
    }

    public async Task<KullaniciOzetDto> ProfilGuncelleAsync(int kullaniciId, ProfilGuncelleIstek istek, CancellationToken ct = default)
    {
        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(k => k.Id == kullaniciId, ct)
            ?? throw new KayitBulunamadiHatasi("Oturum bilgisi bulunamadı.");

        var eposta = istek.Eposta.Trim().ToLowerInvariant();
        if (await _db.Kullanicilar.AnyAsync(k => k.Eposta == eposta && k.Id != kullaniciId, ct))
        {
            throw new CakismaHatasi("Bu e-posta adresi zaten kayıtlı.");
        }

        if (!string.IsNullOrWhiteSpace(istek.YeniSifre))
        {
            if (string.IsNullOrWhiteSpace(istek.MevcutSifre) || !_sifreHasher.Dogrula(istek.MevcutSifre, kullanici.SifreHash))
            {
                throw new YetkisizHatasi("Mevcut şifre hatalı.");
            }
            await _ayar.SifreUzunlugunuDogrulaAsync(istek.YeniSifre, ct);
            kullanici.SifreHash = _sifreHasher.Hashle(istek.YeniSifre);
            kullanici.SifreDegistirilmeTarihi = DateTime.UtcNow;
            kullanici.SifreSifirlamaTokenHash = null;
            kullanici.SifreSifirlamaBitis = null;
        }

        kullanici.Eposta = eposta;
        kullanici.Telefon = string.IsNullOrWhiteSpace(istek.Telefon) ? null : istek.Telefon.Trim();
        kullanici.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return await Ozet(kullanici, null, ct);
    }

    public async Task<SifremiUnuttumSonuc> SifremiUnuttumAsync(SifremiUnuttumIstek istek, CancellationToken ct = default)
    {
        var genel = "Kayıtlı hesabınız varsa sıfırlama kodu oluşturuldu. Geliştirmede kod ekranda görünür; üretimde e-posta ile gider.";
        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(k => k.KullaniciAdi == istek.KullaniciAdi.Trim(), ct);
        if (kullanici is null)
        {
            return new SifremiUnuttumSonuc(genel, null);
        }

        var kod = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        kullanici.SifreSifirlamaTokenHash = _sifreHasher.Hashle(kod);
        kullanici.SifreSifirlamaBitis = DateTime.UtcNow.AddMinutes(30);
        kullanici.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        _gunluk.LogInformation("Şifre sıfırlama kodu üretildi. Kullanıcı: {Kullanici} Eposta: {Eposta}", kullanici.KullaniciAdi, kullanici.Eposta);
        return new SifremiUnuttumSonuc(genel, kod);
    }

    public async Task SifreSifirlaAsync(SifreSifirlaIstek istek, CancellationToken ct = default)
    {
        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(k => k.KullaniciAdi == istek.KullaniciAdi.Trim(), ct)
            ?? throw new GecersizIstekHatasi("Kod geçersiz veya süresi dolmuş.");

        if (string.IsNullOrWhiteSpace(kullanici.SifreSifirlamaTokenHash)
            || kullanici.SifreSifirlamaBitis is null
            || kullanici.SifreSifirlamaBitis < DateTime.UtcNow
            || !_sifreHasher.Dogrula(istek.Kod.Trim(), kullanici.SifreSifirlamaTokenHash))
        {
            throw new GecersizIstekHatasi("Kod geçersiz veya süresi dolmuş.");
        }

        await _ayar.SifreUzunlugunuDogrulaAsync(istek.YeniSifre, ct);
        kullanici.SifreHash = _sifreHasher.Hashle(istek.YeniSifre);
        kullanici.SifreDegistirilmeTarihi = DateTime.UtcNow;
        kullanici.SifreSifirlamaTokenHash = null;
        kullanici.SifreSifirlamaBitis = null;
        kullanici.BasarisizGirisSayisi = 0;
        kullanici.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private async Task<GirisSonuc> OturumUret(Domain.Entities.Kullanici kullanici, int? kurulusId, int oturumDakika, CancellationToken ct)
    {
        var ozet = await Ozet(kullanici, kurulusId, ct);
        var (token, bitis) = _tokenUretici.TokenUret(kullanici, oturumDakika, ozet.AktifKurulusId, ozet.Rol);
        return new GirisSonuc(token, bitis, ozet);
    }

    private async Task<KullaniciOzetDto> Ozet(Domain.Entities.Kullanici kullanici, int? kurulusId, CancellationToken ct)
    {
        var uyeler = await UyeleriGetir(kullanici.Id, ct);
        var secim = uyeler.Select(x => new KurulusSecimDto(x.KurulusId, x.Kurulus?.Ad ?? "", x.Rol)).ToList();
        var aktif = kurulusId is not null ? uyeler.FirstOrDefault(x => x.KurulusId == kurulusId) : uyeler.FirstOrDefault();
        if (kurulusId is not null && aktif is null)
        {
            throw new GecersizIstekHatasi("Bu kuruluşa yetkiniz yok.");
        }

        var rol = !string.IsNullOrWhiteSpace(kullanici.Rol)
            ? kullanici.Rol
            : (aktif?.Rol ?? "Personel");
        var rolAd = await _yetki.RolAdiAsync(rol, ct) ?? rol;
        var izinler = await _yetki.IzinleriGetirAsync(rol, ct);
        return new KullaniciOzetDto(
            kullanici.Id, kullanici.KullaniciAdi, kullanici.Ad, kullanici.Soyad, kullanici.Eposta, kullanici.Telefon,
            rol, kullanici.AktifMi, kullanici.TcKimlikNo, kullanici.PersonelId,
            aktif?.KurulusId, aktif?.Kurulus?.Ad, secim, rolAd, izinler);
    }

    private async Task<List<Domain.Entities.KullaniciKurulus>> UyeleriGetir(int kullaniciId, CancellationToken ct) =>
        await _db.KullaniciKuruluslari.Include(x => x.Kurulus)
            .Where(x => x.KullaniciId == kullaniciId && x.AktifMi)
            .ToListAsync(ct);
}
