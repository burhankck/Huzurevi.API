using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Sakinler;

public class SakinServisi : ISakinServisi
{
    private readonly IUygulamaDbContext _db;

    public SakinServisi(IUygulamaDbContext db)
    {
        _db = db;
    }

    public async Task<List<SakinListDto>> TumunuGetirAsync(CancellationToken ct = default)
    {
        var yataklar = await YerlesimYataklariAsync(ct);
        var sakinler = await _db.Sakinler
            .AsNoTracking()
            .OrderBy(s => s.Ad)
            .ThenBy(s => s.Soyad)
            .ToListAsync(ct);

        return sakinler.Select(s => MapListe(s, yataklar.FirstOrDefault(y => y.SakinId == s.Id))).ToList();
    }

    public async Task<SakinDetayDto> GetirAsync(int id, CancellationToken ct = default)
    {
        var sakin = await _db.Sakinler.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");

        var yatak = await _db.Yataklar
            .AsNoTracking()
            .Include(y => y.Oda)
            .FirstOrDefaultAsync(y => y.SakinId == id, ct);

        var yakinlar = await _db.Yakinlar
            .AsNoTracking()
            .Where(y => y.SakinId == id)
            .OrderBy(y => y.Oncelik)
            .ThenBy(y => y.Ad)
            .ToListAsync(ct);

        var girisTarihi = await _db.SakinYerlesimleri.AsNoTracking()
            .Where(x => x.SakinId == id && x.CikisTarihi == null)
            .Select(x => (DateTime?)x.GirisTarihi)
            .FirstOrDefaultAsync(ct);

        var surec = await SurecOlusturAsync(sakin, yatak, ct);
        return MapDetay(sakin, yatak, yakinlar, girisTarihi, surec);
    }

    public async Task<SakinListDto> OlusturAsync(SakinOlusturIstek request, CancellationToken ct = default)
    {
        var tc = request.TcKimlikNo.Trim();
        var tcVarMi = await _db.Sakinler.AnyAsync(s => s.TcKimlikNo == tc, ct);
        if (tcVarMi)
        {
            throw new CakismaHatasi("Bu T.C. Kimlik Numarası ile kayıtlı bir sakin zaten mevcut.");
        }

        var sakin = new Sakin
        {
            Ad = request.Ad.Trim(),
            Soyad = request.Soyad.Trim(),
            TcKimlikNo = tc,
            Telefon = Metin(request.Telefon),
            Durum = string.IsNullOrWhiteSpace(request.Durum) ? "Aktif" : request.Durum.Trim(),
            DogumTarihi = ToUtc(request.DogumTarihi) ?? DateTime.SpecifyKind(new DateTime(1900, 1, 1), DateTimeKind.Utc),
            KabulTarihi = ToUtc(request.KabulTarihi) ?? DateTime.UtcNow,
            OlusturulmaTarihi = DateTime.UtcNow
        };

        _db.Sakinler.Add(sakin);
        await _db.SaveChangesAsync(ct);
        sakin.KayitNo = $"SKN-{sakin.Id:D6}";
        await _db.SaveChangesAsync(ct);

        return MapListe(sakin, null);
    }

    public async Task GuncelleAsync(int id, SakinGuncelleIstek request, CancellationToken ct = default)
    {
        var sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");

        sakin.Ad = request.Ad.Trim();
        sakin.Soyad = request.Soyad.Trim();
        sakin.Telefon = Metin(request.Telefon);
        sakin.Durum = string.IsNullOrWhiteSpace(request.Durum) ? sakin.Durum : request.Durum.Trim();
        sakin.Cinsiyet = Metin(request.Cinsiyet);
        sakin.DogumYeri = Metin(request.DogumYeri);
        sakin.Uyruk = Metin(request.Uyruk);
        sakin.MedeniDurum = Metin(request.MedeniDurum);
        sakin.KanGrubu = Metin(request.KanGrubu);
        sakin.Adres = Metin(request.Adres);
        sakin.AcilTelefon = Metin(request.AcilTelefon);
        sakin.Notlar = Metin(request.Notlar);
        sakin.BabaAdi = Metin(request.BabaAdi);
        sakin.AnaAdi = Metin(request.AnaAdi);
        sakin.OgrenimDurumu = Metin(request.OgrenimDurumu);
        sakin.Meslek = Metin(request.Meslek);
        sakin.EngelDurumu = Metin(request.EngelDurumu);
        sakin.NeredenGeldigi = Metin(request.NeredenGeldigi);
        sakin.OncekiYasamYeri = Metin(request.OncekiYasamYeri);
        sakin.NufusKutukIli = Metin(request.NufusKutukIli);
        sakin.UcretDurumu = Metin(request.UcretDurumu);
        sakin.AylikGelir = request.AylikGelir;
        sakin.BasvuruDurumu = Metin(request.BasvuruDurumu);
        sakin.KayitTuru = Metin(request.KayitTuru);
        sakin.AyrilisDurumu = Metin(request.AyrilisDurumu);
        sakin.AyrilisTarihi = ToUtc(request.AyrilisTarihi);
        sakin.KabulNedeni = Metin(request.KabulNedeni);
        sakin.KabulSekli = Metin(request.KabulSekli);
        sakin.SonOturduguAdres = Metin(request.SonOturduguAdres);
        sakin.KimGetirdi = Metin(request.KimGetirdi);
        sakin.GuncellenmeTarihi = DateTime.UtcNow;

        if (request.DogumTarihi is not null)
        {
            sakin.DogumTarihi = ToUtc(request.DogumTarihi)!.Value;
        }

        if (request.KabulTarihi is not null)
        {
            sakin.KabulTarihi = ToUtc(request.KabulTarihi)!.Value;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var sakin = await _db.Sakinler.FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new KayitBulunamadiHatasi("Sakin bulunamadı.");

        var yatak = await _db.Yataklar.FirstOrDefaultAsync(y => y.SakinId == id, ct);
        var simdi = DateTime.UtcNow;
        if (yatak != null)
        {
            yatak.SakinId = null;
            yatak.DoluMu = false;
            yatak.GuncellenmeTarihi = simdi;
        }

        var aciklar = await _db.SakinYerlesimleri.Where(x => x.SakinId == id && x.CikisTarihi == null).ToListAsync(ct);
        foreach (var kayit in aciklar)
        {
            kayit.CikisTarihi = simdi;
            kayit.GuncellenmeTarihi = simdi;
        }

        sakin.SilindiMi = true;
        sakin.SilinmeTarihi = simdi;
        sakin.GuncellenmeTarihi = simdi;
        sakin.Durum = "Ayrıldı";
        sakin.YatakId = null;

        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<YakinDto>> YakinlariGetirAsync(int sakinId, CancellationToken ct = default)
    {
        await SakiniDogrulaAsync(sakinId, ct);

        var yakinlar = await _db.Yakinlar
            .AsNoTracking()
            .Where(y => y.SakinId == sakinId)
            .OrderBy(y => y.Oncelik)
            .ThenBy(y => y.Ad)
            .ToListAsync(ct);

        return yakinlar.Select(MapYakin).ToList();
    }

    public async Task<YakinDto> YakinOlusturAsync(int sakinId, YakinOlusturIstek request, CancellationToken ct = default)
    {
        await SakiniDogrulaAsync(sakinId, ct);

        if (request.AcilDurumKisisiMi)
        {
            await AcilKisinininTekOlmasiniSaglaAsync(sakinId, null, ct);
        }

        var yakin = new Yakin
        {
            SakinId = sakinId,
            Ad = request.Ad.Trim(),
            Soyad = request.Soyad.Trim(),
            Yakinlik = request.Yakinlik.Trim(),
            Telefon = Metin(request.Telefon),
            Eposta = Metin(request.Eposta),
            Adres = Metin(request.Adres),
            Meslek = Metin(request.Meslek),
            Aciklama = Metin(request.Aciklama),
            Oncelik = request.Oncelik < 1 ? 1 : request.Oncelik,
            AcilDurumKisisiMi = request.AcilDurumKisisiMi,
            OlusturulmaTarihi = DateTime.UtcNow
        };

        _db.Yakinlar.Add(yakin);
        await _db.SaveChangesAsync(ct);
        return MapYakin(yakin);
    }

    public async Task YakinGuncelleAsync(int sakinId, int yakinId, YakinGuncelleIstek request, CancellationToken ct = default)
    {
        var yakin = await _db.Yakinlar.FirstOrDefaultAsync(y => y.Id == yakinId && y.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Yakın kaydı bulunamadı.");

        if (request.AcilDurumKisisiMi)
        {
            await AcilKisinininTekOlmasiniSaglaAsync(sakinId, yakinId, ct);
        }

        yakin.Ad = request.Ad.Trim();
        yakin.Soyad = request.Soyad.Trim();
        yakin.Yakinlik = request.Yakinlik.Trim();
        yakin.Telefon = Metin(request.Telefon);
        yakin.Eposta = Metin(request.Eposta);
        yakin.Adres = Metin(request.Adres);
        yakin.Meslek = Metin(request.Meslek);
        yakin.Aciklama = Metin(request.Aciklama);
        yakin.Oncelik = request.Oncelik < 1 ? 1 : request.Oncelik;
        yakin.AcilDurumKisisiMi = request.AcilDurumKisisiMi;
        yakin.GuncellenmeTarihi = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
    }

    public async Task YakinSilAsync(int sakinId, int yakinId, CancellationToken ct = default)
    {
        var yakin = await _db.Yakinlar.FirstOrDefaultAsync(y => y.Id == yakinId && y.SakinId == sakinId, ct)
            ?? throw new KayitBulunamadiHatasi("Yakın kaydı bulunamadı.");

        var simdi = DateTime.UtcNow;
        yakin.SilindiMi = true;
        yakin.SilinmeTarihi = simdi;
        yakin.GuncellenmeTarihi = simdi;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<KabulMuayeneDto>> KabulMuayeneleriniGetirAsync(string? arama, CancellationToken ct = default)
    {
        var sakinler = await _db.Sakinler.AsNoTracking()
            .OrderByDescending(s => s.KabulTarihi)
            .ToListAsync(ct);

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var k = arama.Trim();
            sakinler = sakinler.Where(s =>
                ($"{s.Ad} {s.Soyad}".Contains(k, StringComparison.OrdinalIgnoreCase)) ||
                (s.KayitNo != null && s.KayitNo.Contains(k, StringComparison.OrdinalIgnoreCase)) ||
                s.TcKimlikNo.Contains(k)).ToList();
        }

        var ids = sakinler.Select(s => s.Id).ToList();
        var olcumler = await _db.SakinOlcumleri.AsNoTracking().Where(x => ids.Contains(x.SakinId)).Select(x => x.SakinId).Distinct().ToListAsync(ct);
        var degerlendirmeler = await _db.SakinSaglikDegerlendirmeleri.AsNoTracking()
            .Where(x => ids.Contains(x.SakinId))
            .Select(x => new { x.SakinId, x.Tur })
            .ToListAsync(ct);

        var olcumSet = olcumler.ToHashSet();
        var turSet = degerlendirmeler.Select(x => (x.SakinId, x.Tur)).ToHashSet();

        return sakinler.Select(s =>
        {
            var olcum = olcumSet.Contains(s.Id);
            var gelis = turSet.Contains((s.Id, "GelisSekli"));
            var konusma = turSet.Contains((s.Id, "Konusma"));
            var isitme = turSet.Contains((s.Id, "Isitme"));
            var gorme = turSet.Contains((s.Id, "Gorme"));
            var tamam = (olcum ? 1 : 0) + (gelis ? 1 : 0) + (konusma ? 1 : 0) + (isitme ? 1 : 0) + (gorme ? 1 : 0);
            return new KabulMuayeneDto(
                s.Id,
                s.KayitNo,
                $"{s.Ad} {s.Soyad}",
                s.TcKimlikNo,
                s.KabulTarihi,
                s.Durum,
                olcum,
                gelis,
                konusma,
                isitme,
                gorme,
                tamam,
                5);
        }).ToList();
    }

    private async Task SakiniDogrulaAsync(int sakinId, CancellationToken ct)
    {
        var varMi = await _db.Sakinler.AnyAsync(s => s.Id == sakinId, ct);
        if (!varMi)
        {
            throw new KayitBulunamadiHatasi("Sakin bulunamadı.");
        }
    }

    private async Task AcilKisinininTekOlmasiniSaglaAsync(int sakinId, int? haricYakinId, CancellationToken ct)
    {
        var digerleri = await _db.Yakinlar
            .Where(y => y.SakinId == sakinId && y.AcilDurumKisisiMi && (haricYakinId == null || y.Id != haricYakinId))
            .ToListAsync(ct);

        foreach (var kayit in digerleri)
        {
            kayit.AcilDurumKisisiMi = false;
            kayit.GuncellenmeTarihi = DateTime.UtcNow;
        }
    }

    private async Task<List<Yatak>> YerlesimYataklariAsync(CancellationToken ct) =>
        await _db.Yataklar
            .AsNoTracking()
            .Include(y => y.Oda)
            .Where(y => y.SakinId != null)
            .ToListAsync(ct);

    private static SakinListDto MapListe(Sakin sakin, Yatak? yatak)
    {
        var yatakBilgisi = yatak?.Oda == null
            ? null
            : $"Oda {yatak.Oda.OdaNumarasi} - Yatak {yatak.YatakNumarasi}";

        return new SakinListDto(
            sakin.Id,
            sakin.KayitNo,
            sakin.Ad,
            sakin.Soyad,
            sakin.TcKimlikNo,
            sakin.Telefon,
            sakin.Durum,
            sakin.Durum == "Aktif",
            !string.IsNullOrWhiteSpace(sakin.FotoYolu),
            yatak?.Id,
            yatakBilgisi);
    }

    private static SakinDetayDto MapDetay(Sakin sakin, Yatak? yatak, List<Yakin> yakinlar, DateTime? girisTarihi, List<SakinSurecAdimiDto> surec)
    {
        SakinYerlesimDto? yerlesim = null;
        if (yatak?.Oda is not null)
        {
            yerlesim = new SakinYerlesimDto(
                yatak.Id,
                yatak.Oda.Id,
                yatak.Oda.OdaNumarasi,
                yatak.Oda.Kat,
                yatak.Oda.Blok,
                yatak.YatakNumarasi,
                girisTarihi);
        }

        return new SakinDetayDto(
            sakin.Id,
            sakin.KayitNo,
            sakin.Ad,
            sakin.Soyad,
            sakin.TcKimlikNo,
            sakin.Telefon,
            sakin.DogumTarihi,
            sakin.KabulTarihi,
            sakin.Durum,
            sakin.Durum == "Aktif",
            sakin.Cinsiyet,
            sakin.DogumYeri,
            sakin.Uyruk,
            sakin.MedeniDurum,
            sakin.KanGrubu,
            sakin.Adres,
            sakin.AcilTelefon,
            sakin.Notlar,
            sakin.BabaAdi,
            sakin.AnaAdi,
            sakin.OgrenimDurumu,
            sakin.Meslek,
            sakin.EngelDurumu,
            sakin.NeredenGeldigi,
            sakin.OncekiYasamYeri,
            sakin.NufusKutukIli,
            sakin.UcretDurumu,
            sakin.AylikGelir,
            sakin.BasvuruDurumu,
            sakin.KayitTuru,
            sakin.AyrilisDurumu,
            sakin.AyrilisTarihi,
            sakin.KabulNedeni,
            sakin.KabulSekli,
            sakin.SonOturduguAdres,
            sakin.KimGetirdi,
            !string.IsNullOrWhiteSpace(sakin.FotoYolu),
            yerlesim,
            yakinlar.Select(MapYakin).ToList(),
            surec);
    }

    private async Task<List<SakinSurecAdimiDto>> SurecOlusturAsync(Sakin sakin, Yatak? yatak, CancellationToken ct)
    {
        var id = sakin.Id;
        var yerlesimSayisi = await _db.SakinYerlesimleri.CountAsync(x => x.SakinId == id, ct);
        var saglikVar = await _db.SakinOlcumleri.AnyAsync(x => x.SakinId == id, ct)
            || await _db.SakinSaglikDegerlendirmeleri.AnyAsync(x => x.SakinId == id, ct)
            || await _db.SakinSaglikKayitlari.AnyAsync(x => x.SakinId == id, ct);
        var ilacVar = await _db.IlacTakipleri.AnyAsync(x => x.SakinId == id, ct);
        var ziyaretVar = await _db.Ziyaretler.AnyAsync(x => x.SakinId == id, ct);
        var izinVar = await _db.SakinGunlukIzinleri.AnyAsync(x => x.SakinId == id, ct);

        var basvuruTamam = !string.IsNullOrWhiteSpace(sakin.BasvuruDurumu) || !string.IsNullOrWhiteSpace(sakin.KayitTuru);
        var kabulTamam = sakin.KabulTarihi != default;
        var yerlesimTamam = yatak != null;
        var cikisTamam = sakin.AyrilisTarihi != null || sakin.Durum == "Ayrıldı";
        var arsivTamam = sakin.Durum is "Ayrıldı" or "Arşiv";

        return
        [
            new("Basvuru", "Başvuru", basvuruTamam ? "Tamamlandı" : "Bekliyor", sakin.BasvuruDurumu ?? sakin.KayitTuru, 0),
            new("Kabul", "Kabul", kabulTamam ? "Tamamlandı" : "Bekliyor", sakin.KabulTarihi.ToString("dd.MM.yyyy"), 0),
            new("Yerlesim", "Oda yerleştirme", yerlesimTamam ? "Tamamlandı" : "Bekliyor", yerlesimTamam ? $"{yatak!.Oda?.OdaNumarasi} / {yatak.YatakNumarasi}" : "Yatak yok", 6),
            new("Bakim", "Bakım", sakin.Durum == "Aktif" ? "Devam ediyor" : "Bekliyor", sakin.Durum, 0),
            new("Saglik", "Sağlık", saglikVar ? "Tamamlandı" : "Bekliyor", saglikVar ? "Kayıt var" : "Henüz yok", 7),
            new("Ilac", "İlaç", ilacVar ? "Tamamlandı" : "Bekliyor", ilacVar ? "Plan var" : "Henüz yok", 7),
            new("Ziyaret", "Ziyaret", ziyaretVar ? "Tamamlandı" : "Bekliyor", ziyaretVar ? "Kayıt var" : "Henüz yok", null),
            new("Izin", "İzin", izinVar ? "Tamamlandı" : "Bekliyor", izinVar ? "Kayıt var" : "Henüz yok", 5),
            new("OdaDegisikligi", "Oda değişikliği", yerlesimSayisi > 1 ? "Tamamlandı" : "Bekliyor", $"{yerlesimSayisi} yerleşim", 6),
            new("Cikis", "Çıkış", cikisTamam ? "Tamamlandı" : "Bekliyor", sakin.AyrilisDurumu, 0),
            new("Arsiv", "Arşiv", arsivTamam ? "Tamamlandı" : "Bekliyor", sakin.Durum, 0)
        ];
    }

    private static YakinDto MapYakin(Yakin yakin) => new(
        yakin.Id,
        yakin.SakinId,
        yakin.Ad,
        yakin.Soyad,
        yakin.Yakinlik,
        yakin.Telefon,
        yakin.Eposta,
        yakin.Adres,
        yakin.Meslek,
        yakin.Aciklama,
        yakin.Oncelik < 1 ? 1 : yakin.Oncelik,
        yakin.AcilDurumKisisiMi);

    private static string? Metin(string? deger) =>
        string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();

    private static DateTime? ToUtc(DateTime? value)
    {
        if (value is null)
        {
            return null;
        }

        return value.Value.Kind switch
        {
            DateTimeKind.Utc => value.Value,
            DateTimeKind.Local => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
        };
    }
}
