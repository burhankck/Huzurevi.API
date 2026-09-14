using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Features.Kurum;

public interface IOrganizasyonServisi
{
    Task<List<OrganizasyonBirimDto>> BirimlerAsync(int? kurulusId, CancellationToken ct = default);
    Task<OrganizasyonBirimDto> BirimOlusturAsync(OrganizasyonBirimIstek istek, CancellationToken ct = default);
    Task BirimGuncelleAsync(int id, OrganizasyonBirimIstek istek, CancellationToken ct = default);
    Task BirimSilAsync(int id, CancellationToken ct = default);
    Task<List<PersonelAtamaDto>> AtamalarAsync(int? birimId, int? personelId, CancellationToken ct = default);
    Task<PersonelAtamaDto> AtamaOlusturAsync(PersonelAtamaIstek istek, CancellationToken ct = default);
    Task AtamaGuncelleAsync(int id, PersonelAtamaIstek istek, CancellationToken ct = default);
    Task AtamaSilAsync(int id, CancellationToken ct = default);
}

public class OrganizasyonServisi : IOrganizasyonServisi
{
    private readonly IUygulamaDbContext _db;
    private readonly IOturumBaglami _oturum;
    public OrganizasyonServisi(IUygulamaDbContext db, IOturumBaglami oturum)
    {
        _db = db;
        _oturum = oturum;
    }

    public async Task<List<OrganizasyonBirimDto>> BirimlerAsync(int? kurulusId, CancellationToken ct = default)
    {
        var sorgu = _db.OrganizasyonBirimleri.AsNoTracking().AsQueryable();
        kurulusId = _oturum.ListeFiltresi(kurulusId);
        if (kurulusId is not null) sorgu = sorgu.Where(x => x.KurulusId == kurulusId);
        var kayitlar = await sorgu.OrderBy(x => x.Ad).ToListAsync(ct);
        return kayitlar.Select(MapBirim).ToList();
    }

    public async Task<OrganizasyonBirimDto> BirimOlusturAsync(OrganizasyonBirimIstek istek, CancellationToken ct = default)
    {
        await DogrulaUst(istek, null, ct);
        var kayit = new OrganizasyonBirimi { OlusturulmaTarihi = DateTime.UtcNow };
        Doldur(kayit, istek with { KurulusId = _oturum.YazmaKurulusId(istek.KurulusId) });
        _db.OrganizasyonBirimleri.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return MapBirim(kayit);
    }

    public async Task BirimGuncelleAsync(int id, OrganizasyonBirimIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.OrganizasyonBirimleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Birim bulunamadı.");
        _oturum.KurulusDogrula(kayit.KurulusId);
        await DogrulaUst(istek, id, ct);
        Doldur(kayit, istek with { KurulusId = _oturum.YazmaKurulusId(istek.KurulusId) });
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task BirimSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.OrganizasyonBirimleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Birim bulunamadı.");
        _oturum.KurulusDogrula(kayit.KurulusId);
        if (await _db.OrganizasyonBirimleri.AnyAsync(x => x.UstBirimId == id, ct))
            throw new GecersizIstekHatasi("Alt birimi olan kayıt silinemez.");
        if (await _db.PersonelAtamalari.AnyAsync(x => x.BirimId == id && x.AktifMi, ct))
            throw new GecersizIstekHatasi("Aktif ataması olan birim silinemez.");
        kayit.SilindiMi = true;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<PersonelAtamaDto>> AtamalarAsync(int? birimId, int? personelId, CancellationToken ct = default)
    {
        var sorgu = _db.PersonelAtamalari.AsNoTracking().Include(x => x.Personel).Include(x => x.Birim).AsQueryable();
        var kurulusId = _oturum.ListeFiltresi(null);
        if (kurulusId is not null) sorgu = sorgu.Where(x => x.Birim!.KurulusId == kurulusId);
        if (birimId is not null) sorgu = sorgu.Where(x => x.BirimId == birimId);
        if (personelId is not null) sorgu = sorgu.Where(x => x.PersonelId == personelId);
        var kayitlar = await sorgu.OrderByDescending(x => x.BaslangicTarihi).ToListAsync(ct);
        return kayitlar.Select(MapAtama).ToList();
    }

    public async Task<PersonelAtamaDto> AtamaOlusturAsync(PersonelAtamaIstek istek, CancellationToken ct = default)
    {
        await PersonelVar(istek.PersonelId, ct);
        await BirimVar(istek.BirimId, ct);
        var kayit = new PersonelAtama { OlusturulmaTarihi = DateTime.UtcNow };
        DoldurAtama(kayit, istek);
        _db.PersonelAtamalari.Add(kayit);
        await _db.SaveChangesAsync(ct);
        return (await AtamalarAsync(null, null, ct)).First(x => x.Id == kayit.Id);
    }

    public async Task AtamaGuncelleAsync(int id, PersonelAtamaIstek istek, CancellationToken ct = default)
    {
        var kayit = await _db.PersonelAtamalari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Atama bulunamadı.");
        DoldurAtama(kayit, istek);
        kayit.GuncellenmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task AtamaSilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await _db.PersonelAtamalari.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Atama bulunamadı.");
        kayit.SilindiMi = true;
        kayit.AktifMi = false;
        kayit.SilinmeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private async Task DogrulaUst(OrganizasyonBirimIstek istek, int? kendisi, CancellationToken ct)
    {
        var kurulusId = _oturum.YazmaKurulusId(istek.KurulusId);
        if (!await _db.Kuruluslar.AnyAsync(x => x.Id == kurulusId, ct)) throw new KayitBulunamadiHatasi("Kuruluş bulunamadı.");
        if (istek.UstBirimId is null) return;
        if (istek.UstBirimId == kendisi) throw new GecersizIstekHatasi("Birim kendisinin üstü olamaz.");
        var ust = await _db.OrganizasyonBirimleri.FirstOrDefaultAsync(x => x.Id == istek.UstBirimId, ct) ?? throw new KayitBulunamadiHatasi("Üst birim bulunamadı.");
        if (ust.KurulusId != kurulusId) throw new GecersizIstekHatasi("Üst birim aynı kuruluşta olmalıdır.");
    }

    private async Task PersonelVar(int id, CancellationToken ct)
    {
        var kayit = await _db.Personeller.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Personel bulunamadı.");
        _oturum.KurulusDogrula(kayit.KurulusId);
    }

    private async Task BirimVar(int id, CancellationToken ct)
    {
        var kayit = await _db.OrganizasyonBirimleri.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KayitBulunamadiHatasi("Birim bulunamadı.");
        _oturum.KurulusDogrula(kayit.KurulusId);
    }

    private static void Doldur(OrganizasyonBirimi kayit, OrganizasyonBirimIstek istek)
    {
        kayit.KurulusId = istek.KurulusId;
        kayit.UstBirimId = istek.UstBirimId;
        kayit.Ad = istek.Ad.Trim();
        kayit.Kod = string.IsNullOrWhiteSpace(istek.Kod) ? null : istek.Kod.Trim();
        kayit.AktifMi = istek.AktifMi;
    }

    private static void DoldurAtama(PersonelAtama kayit, PersonelAtamaIstek istek)
    {
        kayit.PersonelId = istek.PersonelId;
        kayit.BirimId = istek.BirimId;
        kayit.Gorev = string.IsNullOrWhiteSpace(istek.Gorev) ? null : istek.Gorev.Trim();
        kayit.BaslangicTarihi = SakinVarlikYardimcisi.ToUtc(istek.BaslangicTarihi);
        kayit.BitisTarihi = istek.BitisTarihi is null ? null : SakinVarlikYardimcisi.ToUtc(istek.BitisTarihi.Value);
        kayit.AktifMi = istek.AktifMi;
    }

    private static OrganizasyonBirimDto MapBirim(OrganizasyonBirimi x) => new(x.Id, x.KurulusId, x.UstBirimId, x.Ad, x.Kod, x.AktifMi);
    private static PersonelAtamaDto MapAtama(PersonelAtama x) => new(
        x.Id, x.PersonelId, x.Personel is null ? "" : $"{x.Personel.Ad} {x.Personel.Soyad}", x.BirimId, x.Birim?.Ad ?? "", x.Gorev, x.BaslangicTarihi, x.BitisTarihi, x.AktifMi);
}
