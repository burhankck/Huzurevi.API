using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Application.Common.Interfaces;

public interface IUygulamaDbContext
{
    DbSet<Oda> Odalar { get; }
    DbSet<Yatak> Yataklar { get; }
    DbSet<Sakin> Sakinler { get; }
    DbSet<IlacTakip> IlacTakipleri { get; }
    DbSet<Kullanici> Kullanicilar { get; }
    DbSet<UygulamaAyari> UygulamaAyarlari { get; }
    DbSet<Yakin> Yakinlar { get; }
    DbSet<Ziyaret> Ziyaretler { get; }
    DbSet<Vasi> Vasiler { get; }
    DbSet<Mirasci> Mirascilar { get; }
    DbSet<SakinBelge> SakinBelgeleri { get; }
    DbSet<SakinMal> SakinMallari { get; }
    DbSet<SakinGelir> SakinGelirleri { get; }
    DbSet<SakinSosyalGuvence> SakinSosyalGuvenceleri { get; }
    DbSet<SakinGunlukIzin> SakinGunlukIzinleri { get; }
    DbSet<SakinEmanet> SakinEmanetleri { get; }
    DbSet<SakinYerlesim> SakinYerlesimleri { get; }
    DbSet<SakinOlcum> SakinOlcumleri { get; }
    DbSet<SakinSaglikDegerlendirme> SakinSaglikDegerlendirmeleri { get; }
    DbSet<SakinSaglikKayit> SakinSaglikKayitlari { get; }
    DbSet<KurumSaglikKayit> KurumSaglikKayitlari { get; }
    DbSet<NarkotikIlac> NarkotikIlaclari { get; }
    DbSet<NarkotikHareket> NarkotikHareketleri { get; }
    DbSet<IlacEmri> IlacEmirleri { get; }
    DbSet<IlacUygulama> IlacUygulamalari { get; }
    DbSet<OnayYetkisi> OnayYetkileri { get; }
    DbSet<IzinSureci> IzinSurecleri { get; }
    DbSet<EsyaTespit> EsyaTespitleri { get; }
    DbSet<MirasciTeslim> MirasciTeslimleri { get; }
    DbSet<SosyalInceleme> SosyalIncelemeler { get; }
    DbSet<PsikolojikDegerlendirme> PsikolojikDegerlendirmeler { get; }
    DbSet<Yemek> Yemekler { get; }
    DbSet<BeslenmeProfili> BeslenmeProfilleri { get; }
    DbSet<GunlukMenu> GunlukMenuler { get; }
    DbSet<OzelMenuPlani> OzelMenuPlanlari { get; }
    DbSet<YemekTuketim> YemekTuketimleri { get; }
    DbSet<SiviAlimi> SiviAlimlari { get; }
    DbSet<KutuphaneDolap> KutuphaneDolaplari { get; }
    DbSet<KutuphaneRaf> KutuphaneRaflari { get; }
    DbSet<Kitap> Kitaplar { get; }
    DbSet<KitapKopya> KitapKopyalari { get; }
    DbSet<KitapOdunc> KitapOduncleri { get; }
    DbSet<Kurulus> Kuruluslar { get; }
    DbSet<KullaniciKurulus> KullaniciKuruluslari { get; }
    DbSet<Ulke> Ulkeler { get; }
    DbSet<Il> Iller { get; }
    DbSet<Ilce> Ilceler { get; }
    DbSet<Mahalle> Mahalleler { get; }
    DbSet<GlobalTanim> GlobalTanimlar { get; }
    DbSet<Personel> Personeller { get; }
    DbSet<PersonelYakin> PersonelYakinlari { get; }
    DbSet<PersonelBelge> PersonelBelgeleri { get; }
    DbSet<OrganizasyonBirimi> OrganizasyonBirimleri { get; }
    DbSet<PersonelAtama> PersonelAtamalari { get; }
    DbSet<Rol> Roller { get; }
    DbSet<RolIzin> RolIzinleri { get; }
    DbSet<PdfSablon> PdfSablonlari { get; }
    DbSet<PdfArsiv> PdfArsivleri { get; }
    DbSet<DenetimKaydi> DenetimKayitlari { get; }
    DbSet<YedekKaydi> YedekKayitlari { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
