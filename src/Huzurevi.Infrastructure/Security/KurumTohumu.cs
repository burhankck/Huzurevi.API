using Huzurevi.Application.Features.Yetkiler;
using Huzurevi.Domain.Entities;
using Huzurevi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Infrastructure.Security;

public static class KurumTohumu
{
    private static readonly (string Plaka, string Ad)[] Iller =
    [
        ("01","Adana"),("02","Adıyaman"),("03","Afyonkarahisar"),("04","Ağrı"),("05","Amasya"),
        ("06","Ankara"),("07","Antalya"),("08","Artvin"),("09","Aydın"),("10","Balıkesir"),
        ("11","Bilecik"),("12","Bingöl"),("13","Bitlis"),("14","Bolu"),("15","Burdur"),
        ("16","Bursa"),("17","Çanakkale"),("18","Çankırı"),("19","Çorum"),("20","Denizli"),
        ("21","Diyarbakır"),("22","Edirne"),("23","Elazığ"),("24","Erzincan"),("25","Erzurum"),
        ("26","Eskişehir"),("27","Gaziantep"),("28","Giresun"),("29","Gümüşhane"),("30","Hakkari"),
        ("31","Hatay"),("32","Isparta"),("33","Mersin"),("34","İstanbul"),("35","İzmir"),
        ("36","Kars"),("37","Kastamonu"),("38","Kayseri"),("39","Kırklareli"),("40","Kırşehir"),
        ("41","Kocaeli"),("42","Konya"),("43","Kütahya"),("44","Malatya"),("45","Manisa"),
        ("46","Kahramanmaraş"),("47","Mardin"),("48","Muğla"),("49","Muş"),("50","Nevşehir"),
        ("51","Niğde"),("52","Ordu"),("53","Rize"),("54","Sakarya"),("55","Samsun"),
        ("56","Siirt"),("57","Sinop"),("58","Sivas"),("59","Tekirdağ"),("60","Tokat"),
        ("61","Trabzon"),("62","Tunceli"),("63","Şanlıurfa"),("64","Uşak"),("65","Van"),
        ("66","Yozgat"),("67","Zonguldak"),("68","Aksaray"),("69","Bayburt"),("70","Karaman"),
        ("71","Kırıkkale"),("72","Batman"),("73","Şırnak"),("74","Bartın"),("75","Ardahan"),
        ("76","Iğdır"),("77","Yalova"),("78","Karabük"),("79","Kilis"),("80","Osmaniye"),
        ("81","Düzce")
    ];

    public static async Task UygulaAsync(UygulamaDbContext db, CancellationToken ct = default)
    {
        if (!await db.Ulkeler.AnyAsync(ct))
        {
            var turkiye = new Ulke { Ad = "Türkiye", Kod = "TR", OlusturulmaTarihi = DateTime.UtcNow };
            db.Ulkeler.Add(turkiye);
            await db.SaveChangesAsync(ct);

            foreach (var (plaka, ad) in Iller)
            {
                db.Iller.Add(new Il { UlkeId = turkiye.Id, Ad = ad, PlakaKodu = plaka, OlusturulmaTarihi = DateTime.UtcNow });
            }
            await db.SaveChangesAsync(ct);

            var ankara = await db.Iller.FirstAsync(x => x.PlakaKodu == "06", ct);
            var cankaya = new Ilce { IlId = ankara.Id, Ad = "Çankaya", OlusturulmaTarihi = DateTime.UtcNow };
            var kecioren = new Ilce { IlId = ankara.Id, Ad = "Keçiören", OlusturulmaTarihi = DateTime.UtcNow };
            db.Ilceler.AddRange(cankaya, kecioren);
            await db.SaveChangesAsync(ct);
            db.Mahalleler.Add(new Mahalle { IlceId = cankaya.Id, Ad = "Kızılay", OlusturulmaTarihi = DateTime.UtcNow });
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Kuruluslar.AnyAsync(ct))
        {
            db.Kuruluslar.Add(new Kurulus
            {
                Ad = "İdare Huzurevi",
                KisaAd = "İdare",
                PlakaKodu = "06",
                Adres = "Ankara",
                Telefon = "03120000000",
                AktifMi = true,
                OlusturulmaTarihi = DateTime.UtcNow
            });
            await db.SaveChangesAsync(ct);
        }

        if (!await db.GlobalTanimlar.AnyAsync(ct))
        {
            string[][] ornekler =
            [
                ["Saglik", "Diyabet"], ["Saglik", "Hipertansiyon"],
                ["Bakim", "Tam bağımlı"], ["Bakim", "Yarı bağımlı"],
                ["Oda", "Tek kişilik"], ["Oda", "İki kişilik"],
                ["Personel", "Hemşire"], ["Personel", "Bakım elemanı"],
                ["Finans", "Maaş"], ["Finans", "Sosyal yardım"]
            ];
            foreach (var s in ornekler)
            {
                db.GlobalTanimlar.Add(new GlobalTanim { Kategori = s[0], Ad = s[1], AktifMi = true, OlusturulmaTarihi = DateTime.UtcNow });
            }
            await db.SaveChangesAsync(ct);
        }

        var kurulus = await db.Kuruluslar.OrderBy(x => x.Id).FirstAsync(ct);
        if (!await db.OrganizasyonBirimleri.AnyAsync(ct))
        {
            db.OrganizasyonBirimleri.Add(new OrganizasyonBirimi
            {
                KurulusId = kurulus.Id,
                Ad = "Genel müdürlük",
                Kod = "GM",
                AktifMi = true,
                OlusturulmaTarihi = DateTime.UtcNow
            });
            await db.SaveChangesAsync(ct);
        }

        var admin = await db.Kullanicilar.FirstOrDefaultAsync(k => k.KullaniciAdi == "admin", ct);
        if (admin is not null && !await db.KullaniciKuruluslari.AnyAsync(x => x.KullaniciId == admin.Id, ct))
        {
            db.KullaniciKuruluslari.Add(new KullaniciKurulus
            {
                KullaniciId = admin.Id,
                KurulusId = kurulus.Id,
                Rol = "Yonetici",
                AktifMi = true,
                OlusturulmaTarihi = DateTime.UtcNow
            });
            await db.SaveChangesAsync(ct);
        }

        var ayse = await db.Kullanicilar.FirstOrDefaultAsync(k => k.KullaniciAdi == "ayse", ct);
        if (ayse is not null && !await db.KullaniciKuruluslari.AnyAsync(x => x.KullaniciId == ayse.Id, ct))
        {
            db.KullaniciKuruluslari.Add(new KullaniciKurulus
            {
                KullaniciId = ayse.Id,
                KurulusId = kurulus.Id,
                Rol = "Personel",
                AktifMi = true,
                OlusturulmaTarihi = DateTime.UtcNow
            });
            await db.SaveChangesAsync(ct);
        }

        await RolleriTohumla(db, ct);
    }

    private static async Task RolleriTohumla(UygulamaDbContext db, CancellationToken ct)
    {
        if (!await db.Roller.AnyAsync(x => x.Kod == IzinKatalogu.Yonetici, ct))
        {
            db.Roller.Add(new Rol
            {
                Kod = IzinKatalogu.Yonetici,
                Ad = "Yönetici",
                SistemRoluMu = true,
                AktifMi = true,
                OlusturulmaTarihi = DateTime.UtcNow
            });
        }

        if (!await db.Roller.AnyAsync(x => x.Kod == IzinKatalogu.Personel, ct))
        {
            db.Roller.Add(new Rol
            {
                Kod = IzinKatalogu.Personel,
                Ad = "Personel",
                SistemRoluMu = true,
                AktifMi = true,
                OlusturulmaTarihi = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync(ct);

        var personel = await db.Roller.FirstAsync(x => x.Kod == IzinKatalogu.Personel, ct);
        var mevcut = await db.RolIzinleri.Where(x => x.RolId == personel.Id).Select(x => x.IzinKodu).ToListAsync(ct);
        foreach (var kod in IzinKatalogu.PersonelVarsayilan)
        {
            if (!mevcut.Contains(kod))
            {
                db.RolIzinleri.Add(new RolIzin
                {
                    RolId = personel.Id,
                    IzinKodu = kod,
                    OlusturulmaTarihi = DateTime.UtcNow
                });
            }
        }

        await db.SaveChangesAsync(ct);
        await PdfSablonTohumla(db, ct);
    }

    private static async Task PdfSablonTohumla(UygulamaDbContext db, CancellationToken ct)
    {
        if (await db.PdfSablonlari.AnyAsync(ct)) return;
        db.PdfSablonlari.Add(new PdfSablon
        {
            Kod = "SakinOzet",
            Ad = "Sakin özet belgesi",
            Baslik = "Sakin özet belgesi",
            Icerik = "Tarih: {{Tarih}}\n\nSakin: {{AdSoyad}}\nT.C.: {{TcKimlikNo}}\nKayıt no: {{KayitNo}}\nDurum: {{Durum}}\nTelefon: {{Telefon}}\n\nBu belge huzurevi kayıtlarından üretilmiştir.",
            AktifMi = true,
            OlusturulmaTarihi = DateTime.UtcNow
        });
        db.PdfSablonlari.Add(new PdfSablon
        {
            Kod = "SakinBildirim",
            Ad = "Sakin bildirim yazısı",
            Baslik = "Bildirim",
            Icerik = "{{Tarih}} tarihinde {{AdSoyad}} (T.C. {{TcKimlikNo}}) hakkında bildirim düzenlenmiştir.\n\nDurum: {{Durum}}",
            AktifMi = true,
            OlusturulmaTarihi = DateTime.UtcNow
        });
        await db.SaveChangesAsync(ct);
    }
}
