using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Kurum;

public interface IPersonelServisi
{
    Task<List<PersonelListDto>> ListeleAsync(int? kurulusId, CancellationToken ct = default);
    Task<PersonelDto> GetirAsync(int id, CancellationToken ct = default);
    Task<PersonelDto> OlusturAsync(PersonelIstek istek, CancellationToken ct = default);
    Task GuncelleAsync(int id, PersonelIstek istek, CancellationToken ct = default);
    Task SilAsync(int id, CancellationToken ct = default);
    Task FotoYukleAsync(int id, string dosyaAdi, string icerikTipi, Stream icerik, CancellationToken ct = default);
    Task<KayitliDosya> FotoGetirAsync(int id, CancellationToken ct = default);

    Task<List<PersonelYakinDto>> YakinlarAsync(int personelId, CancellationToken ct = default);
    Task<PersonelYakinDto> YakinOlusturAsync(int personelId, PersonelYakinIstek istek, CancellationToken ct = default);
    Task YakinGuncelleAsync(int personelId, int id, PersonelYakinIstek istek, CancellationToken ct = default);
    Task YakinSilAsync(int personelId, int id, CancellationToken ct = default);

    Task<List<PersonelBelgeDto>> BelgelerAsync(int personelId, CancellationToken ct = default);
    Task<PersonelBelgeDto> BelgeYukleAsync(int personelId, string belgeTuru, string? aciklama, string dosyaAdi, string icerikTipi, long boyut, Stream icerik, CancellationToken ct = default);
    Task BelgeSilAsync(int personelId, int id, CancellationToken ct = default);
    Task<KayitliDosya> BelgeGetirAsync(int personelId, int id, CancellationToken ct = default);
}

public class PersonelServisi : IPersonelServisi
{
    private readonly IUygulamaDbContext _db;
    private readonly IDosyaDepolama _dosya;
    private readonly IOturumBaglami _oturum;
    public PersonelServisi(IUygulamaDbContext db, IDosyaDepolama dosya, IOturumBaglami oturum)
    {
        _db = db;
        _dosya = dosya;
        _oturum = oturum;
    }

    public async Task<List<PersonelListDto>> ListeleAsync(int? kurulusId, CancellationToken ct = default)
    {
        var sorgu = _db.Personeller.AsNoTracking().Include(x => x.Kurulus).AsQueryable();
        kurulusId = _oturum.ListeFiltresi(kurulusId);
        if (kurulusId is not null) sorgu = sorgu.Where(x => x.KurulusId == kurulusId);
        var kayitlar = await sorgu.OrderBy(x => x.Ad).ThenBy(x => x.Soyad).ToListAsync(ct);
        return kayitlar.Select(x => new PersonelListDto(x.Id, x.SicilNo, x.Ad, x.Soyad, x.Unvan, x.Gorev, x.Durum, x.Kurulus?.Ad ?? "", !string.IsNullOrWhiteSpace(x.FotoYolu))).ToList();
    }

    public async Task<PersonelDto> GetirAsync(int id, CancellationToken ct = default) => Map(await Kayit(id, ct));

    public async Task<PersonelDto> OlusturAsync(PersonelIstek istek, CancellationToken ct = default)
    {
        var kurulusId = _oturum.YazmaKurulusId(istek.KurulusId);
        await KurulusVar(kurulusId, ct);
        if (await _db.Personeller.AnyAsync(x => x.KurulusId == kurulusId && x.SicilNo == istek.SicilNo.Trim(), ct))
            throw new CakismaHatasi("Bu sicil numarası bu kuruluşta kayıtlı.");
        var kayit = new Personel { OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek with { KurulusId = kurulusId });
        _db.Personeller.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return Map(kayit);
    }

    public async Task GuncelleAsync(int id, PersonelIstek istek, CancellationToken ct = default)
    {
        var kayit = await Kayit(id, ct);
        var kurulusId = _oturum.YazmaKurulusId(istek.KurulusId);
        if (await _db.Personeller.AnyAsync(x => x.KurulusId == kurulusId && x.SicilNo == istek.SicilNo.Trim() && x.Id != id, ct))
            throw new CakismaHatasi("Bu sicil numarası bu kuruluşta kayıtlı.");
        Doldur(kayit, istek);
        kayit.KurulusId = kurulusId;
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await Kayit(id, ct);
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task FotoYukleAsync(int id, string dosyaAdi, string icerikTipi, Stream icerik, CancellationToken ct = default)
    {
        var kayit = await Kayit(id, ct);
        await _dosya.SilAsync(kayit.FotoYolu, ct);
        kayit.FotoYolu = await _dosya.KaydetAsync($"personel/{id}", Path.GetExtension(dosyaAdi), icerik, ct);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<KayitliDosya> FotoGetirAsync(int id, CancellationToken ct = default)
    {
        var kayit = await Kayit(id, ct);
        if (string.IsNullOrWhiteSpace(kayit.FotoYolu)) throw new KayitBulunamadiHatasi("Fotoğraf yok.");
        return await _dosya.AcAsync(kayit.FotoYolu, "image/jpeg", "foto" + Path.GetExtension(kayit.FotoYolu), ct)
            ?? throw new KayitBulunamadiHatasi("Fotoğraf dosyası bulunamadı.");
    }

    public async Task<List<PersonelYakinDto>> YakinlarAsync(int personelId, CancellationToken ct = default)
    {
        await Kayit(personelId, ct);
        var kayitlar = await _db.PersonelYakinlari.AsNoTracking().Where(x => x.PersonelId == personelId).OrderBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(x => new PersonelYakinDto(x.Id, x.Ad, x.Soyad, x.Yakinlik, x.Telefon, x.Adres)).ToList();
    }

    public async Task<PersonelYakinDto> YakinOlusturAsync(int personelId, PersonelYakinIstek istek, CancellationToken ct = default)
    {
        await Kayit(personelId, ct);
        var kayit = new PersonelYakin { PersonelId = personelId, Ad = istek.Ad.Trim(), Soyad = istek.Soyad.Trim(), Yakinlik = istek.Yakinlik.Trim(), Telefon = Metin(istek.Telefon), Adres = Metin(istek.Adres), OlusturulmaTarihi = DateTime.UtcNow };
        _db.PersonelYakinlari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return new PersonelYakinDto(kayit.Id, kayit.Ad, kayit.Soyad, kayit.Yakinlik, kayit.Telefon, kayit.Adres);
    }

    public async Task YakinGuncelleAsync(int personelId, int id, PersonelYakinIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.PersonelYakinlari.FirstOrDefaultAsync(x => x.Id == id && x.PersonelId == personelId, ct) ?? throw new KayitBulunamadiHatasi("Yakın bulunamadı.");
        kayit.Ad = istek.Ad.Trim();
        kayit.Soyad = istek.Soyad.Trim();
        kayit.Yakinlik = istek.Yakinlik.Trim();
        kayit.Telefon = Metin(istek.Telefon);
        kayit.Adres = Metin(istek.Adres);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task YakinSilAsync(int personelId, int id, CancellationToken ct = default)
    {
        var kayit = await _db.PersonelYakinlari.FirstOrDefaultAsync(x => x.Id == id && x.PersonelId == personelId, ct) ?? throw new KayitBulunamadiHatasi("Yakın bulunamadı.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<PersonelBelgeDto>> BelgelerAsync(int personelId, CancellationToken ct = default)
    {
        await Kayit(personelId, ct);
        var kayitlar = await _db.PersonelBelgeleri.AsNoTracking().Where(x => x.PersonelId == personelId).OrderByDescending(x => x.Id).ToListAsync(ct);
        return kayitlar.Select(x => new PersonelBelgeDto(x.Id, x.BelgeTuru, x.BelgeTarihi, x.Aciklama, x.OrijinalAd)).ToList();
    }

    public async Task<PersonelBelgeDto> BelgeYukleAsync(int personelId, string belgeTuru, string? aciklama, string dosyaAdi, string icerikTipi, long boyut, Stream icerik, CancellationToken ct = default)
    {
        await Kayit(personelId, ct);
        var yol = await _dosya.KaydetAsync($"personel/{personelId}/belgeler", Path.GetExtension(dosyaAdi), icerik, ct);
        var kayit = new PersonelBelge
        {
            PersonelId = personelId, BelgeTuru = belgeTuru.Trim(), Aciklama = Metin(aciklama), OrijinalAd = dosyaAdi, SaklamaYolu = yol, IcerikTipi = icerikTipi, Boyut = boyut, OlusturulmaTarihi = DateTime.UtcNow
        };
        _db.PersonelBelgeleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return new PersonelBelgeDto(kayit.Id, kayit.BelgeTuru, kayit.BelgeTarihi, kayit.Aciklama, kayit.OrijinalAd);
    }

    public async Task BelgeSilAsync(int personelId, int id, CancellationToken ct = default)
    {
        var kayit = await _db.PersonelBelgeleri.FirstOrDefaultAsync(x => x.Id == id && x.PersonelId == personelId, ct) ?? throw new KayitBulunamadiHatasi("Belge bulunamadı.");
        await _dosya.SilAsync(kayit.SaklamaYolu, ct);
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<KayitliDosya> BelgeGetirAsync(int personelId, int id, CancellationToken ct = default)
    {
        var kayit = await _db.PersonelBelgeleri.FirstOrDefaultAsync(x => x.Id == id && x.PersonelId == personelId, ct) ?? throw new KayitBulunamadiHatasi("Belge bulunamadı.");
        return await _dosya.AcAsync(kayit.SaklamaYolu, kayit.IcerikTipi, kayit.OrijinalAd, ct) ?? throw new KayitBulunamadiHatasi("Dosya bulunamadı.");
    }

    private async Task<Personel> Kayit(int id, CancellationToken ct)
    {
        var kayit = await _db.Personeller.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Personel bulunamadı.");
        _oturum.KurulusDogrula(kayit.KurulusId);
        return kayit;
    }

    private async Task KurulusVar(int id, CancellationToken ct)
    {
        if (!await _db.Kuruluslar.AnyAsync(x => x.Id == id, ct)) throw new KayitBulunamadiHatasi("Kuruluş bulunamadı.");
    }

    private static void Doldur(Personel kayit, PersonelIstek istek)
    {
        kayit.KurulusId = istek.KurulusId;
        kayit.SicilNo = istek.SicilNo.Trim();
        kayit.TcKimlikNo = Metin(istek.TcKimlikNo);
        kayit.Ad = istek.Ad.Trim();
        kayit.Soyad = istek.Soyad.Trim();
        kayit.Unvan = Metin(istek.Unvan);
        kayit.Meslek = Metin(istek.Meslek);
        kayit.Gorev = Metin(istek.Gorev);
        kayit.Durum = istek.Durum;
        kayit.DogumTarihi = istek.DogumTarihi is null ? null : SakinVarlikYardimcisi.ToUtc(istek.DogumTarihi.Value);
        kayit.Cinsiyet = Metin(istek.Cinsiyet);
        kayit.KanGrubu = Metin(istek.KanGrubu);
        kayit.Eposta = Metin(istek.Eposta);
        kayit.Telefon = Metin(istek.Telefon);
        kayit.Adres = Metin(istek.Adres);
        kayit.UlkeId = istek.UlkeId;
        kayit.IlId = istek.IlId;
        kayit.IlceId = istek.IlceId;
        kayit.MahalleId = istek.MahalleId;
        kayit.IseBaslamaTarihi = istek.IseBaslamaTarihi is null ? null : SakinVarlikYardimcisi.ToUtc(istek.IseBaslamaTarihi.Value);
        kayit.AyrilisTarihi = istek.AyrilisTarihi is null ? null : SakinVarlikYardimcisi.ToUtc(istek.AyrilisTarihi.Value);
        kayit.Notlar = Metin(istek.Notlar);
    }

    private static PersonelDto Map(Personel x) => new(
        x.Id, x.KurulusId, x.SicilNo, x.TcKimlikNo, x.Ad, x.Soyad, x.Unvan, x.Meslek, x.Gorev, x.Durum,
        x.DogumTarihi, x.Cinsiyet, x.KanGrubu, x.Eposta, x.Telefon, x.Adres,
        x.UlkeId, x.IlId, x.IlceId, x.MahalleId, x.IseBaslamaTarihi, x.AyrilisTarihi, x.Notlar, !string.IsNullOrWhiteSpace(x.FotoYolu));

    private static string? Metin(string? deger) => string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();
}
