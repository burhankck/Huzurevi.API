using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.Infrastructure.Persistence;

public class UygulamaDbContext : DbContext, IUygulamaDbContext
{
    static UygulamaDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options)
    {
    }

    public DbSet<Oda> Odalar => Set<Oda>();
    public DbSet<Yatak> Yataklar => Set<Yatak>();
    public DbSet<Sakin> Sakinler => Set<Sakin>();
    public DbSet<IlacTakip> IlacTakipleri => Set<IlacTakip>();
    public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
    public DbSet<UygulamaAyari> UygulamaAyarlari => Set<UygulamaAyari>();
    public DbSet<Yakin> Yakinlar => Set<Yakin>();
    public DbSet<Ziyaret> Ziyaretler => Set<Ziyaret>();
    public DbSet<Vasi> Vasiler => Set<Vasi>();
    public DbSet<Mirasci> Mirascilar => Set<Mirasci>();
    public DbSet<SakinBelge> SakinBelgeleri => Set<SakinBelge>();
    public DbSet<SakinMal> SakinMallari => Set<SakinMal>();
    public DbSet<SakinGelir> SakinGelirleri => Set<SakinGelir>();
    public DbSet<SakinSosyalGuvence> SakinSosyalGuvenceleri => Set<SakinSosyalGuvence>();
    public DbSet<SakinGunlukIzin> SakinGunlukIzinleri => Set<SakinGunlukIzin>();
    public DbSet<SakinEmanet> SakinEmanetleri => Set<SakinEmanet>();
    public DbSet<SakinYerlesim> SakinYerlesimleri => Set<SakinYerlesim>();
    public DbSet<SakinOlcum> SakinOlcumleri => Set<SakinOlcum>();
    public DbSet<SakinSaglikDegerlendirme> SakinSaglikDegerlendirmeleri => Set<SakinSaglikDegerlendirme>();
    public DbSet<SakinSaglikKayit> SakinSaglikKayitlari => Set<SakinSaglikKayit>();
    public DbSet<KurumSaglikKayit> KurumSaglikKayitlari => Set<KurumSaglikKayit>();
    public DbSet<NarkotikIlac> NarkotikIlaclari => Set<NarkotikIlac>();
    public DbSet<NarkotikHareket> NarkotikHareketleri => Set<NarkotikHareket>();
    public DbSet<IlacEmri> IlacEmirleri => Set<IlacEmri>();
    public DbSet<IlacUygulama> IlacUygulamalari => Set<IlacUygulama>();
    public DbSet<OnayYetkisi> OnayYetkileri => Set<OnayYetkisi>();
    public DbSet<IzinSureci> IzinSurecleri => Set<IzinSureci>();
    public DbSet<EsyaTespit> EsyaTespitleri => Set<EsyaTespit>();
    public DbSet<MirasciTeslim> MirasciTeslimleri => Set<MirasciTeslim>();
    public DbSet<SosyalInceleme> SosyalIncelemeler => Set<SosyalInceleme>();
    public DbSet<PsikolojikDegerlendirme> PsikolojikDegerlendirmeler => Set<PsikolojikDegerlendirme>();
    public DbSet<Yemek> Yemekler => Set<Yemek>();
    public DbSet<BeslenmeProfili> BeslenmeProfilleri => Set<BeslenmeProfili>();
    public DbSet<GunlukMenu> GunlukMenuler => Set<GunlukMenu>();
    public DbSet<OzelMenuPlani> OzelMenuPlanlari => Set<OzelMenuPlani>();
    public DbSet<YemekTuketim> YemekTuketimleri => Set<YemekTuketim>();
    public DbSet<SiviAlimi> SiviAlimlari => Set<SiviAlimi>();
    public DbSet<KutuphaneDolap> KutuphaneDolaplari => Set<KutuphaneDolap>();
    public DbSet<KutuphaneRaf> KutuphaneRaflari => Set<KutuphaneRaf>();
    public DbSet<Kitap> Kitaplar => Set<Kitap>();
    public DbSet<KitapKopya> KitapKopyalari => Set<KitapKopya>();
    public DbSet<KitapOdunc> KitapOduncleri => Set<KitapOdunc>();
    public DbSet<Kurulus> Kuruluslar => Set<Kurulus>();
    public DbSet<KullaniciKurulus> KullaniciKuruluslari => Set<KullaniciKurulus>();
    public DbSet<Ulke> Ulkeler => Set<Ulke>();
    public DbSet<Il> Iller => Set<Il>();
    public DbSet<Ilce> Ilceler => Set<Ilce>();
    public DbSet<Mahalle> Mahalleler => Set<Mahalle>();
    public DbSet<GlobalTanim> GlobalTanimlar => Set<GlobalTanim>();
    public DbSet<Personel> Personeller => Set<Personel>();
    public DbSet<PersonelYakin> PersonelYakinlari => Set<PersonelYakin>();
    public DbSet<PersonelBelge> PersonelBelgeleri => Set<PersonelBelge>();
    public DbSet<OrganizasyonBirimi> OrganizasyonBirimleri => Set<OrganizasyonBirimi>();
    public DbSet<PersonelAtama> PersonelAtamalari => Set<PersonelAtama>();
    public DbSet<Rol> Roller => Set<Rol>();
    public DbSet<RolIzin> RolIzinleri => Set<RolIzin>();
    public DbSet<PdfSablon> PdfSablonlari => Set<PdfSablon>();
    public DbSet<PdfArsiv> PdfArsivleri => Set<PdfArsiv>();
    public DbSet<DenetimKaydi> DenetimKayitlari => Set<DenetimKaydi>();
    public DbSet<YedekKaydi> YedekKayitlari => Set<YedekKaydi>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Oda>(entity =>
        {
            entity.ToTable("Odalar");
            entity.HasQueryFilter(o => !o.SilindiMi);
            entity.HasIndex(o => o.OdaNumarasi)
                .IsUnique()
                .HasFilter("\"SilindiMi\" = FALSE");
            entity.Property(o => o.OdaNumarasi).IsRequired().HasMaxLength(20);
            entity.Property(o => o.Blok).HasMaxLength(50);
            entity.Property(o => o.Durum).IsRequired().HasMaxLength(20);
            entity.Property(o => o.OdaTipi).HasMaxLength(30);
        });

        modelBuilder.Entity<Yatak>(entity =>
        {
            entity.ToTable("Yataklar");
            entity.HasQueryFilter(y => !y.SilindiMi);
            entity.HasOne(y => y.Oda)
                .WithMany(o => o.Yataklar)
                .HasForeignKey(y => y.OdaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(y => y.Sakin)
                .WithOne()
                .HasForeignKey<Yatak>(y => y.SakinId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(y => y.SakinId).IsUnique();
        });

        modelBuilder.Entity<Sakin>(entity =>
        {
            entity.ToTable("Sakinler");
            entity.HasQueryFilter(s => !s.SilindiMi);
            entity.HasIndex(s => s.TcKimlikNo).IsUnique();
            entity.Property(s => s.Cinsiyet).HasMaxLength(20);
            entity.Property(s => s.DogumYeri).HasMaxLength(100);
            entity.Property(s => s.Uyruk).HasMaxLength(50);
            entity.Property(s => s.MedeniDurum).HasMaxLength(30);
            entity.Property(s => s.KanGrubu).HasMaxLength(20);
            entity.Property(s => s.Adres).HasMaxLength(300);
            entity.Property(s => s.AcilTelefon).HasMaxLength(20);
            entity.Property(s => s.Notlar).HasMaxLength(500);
            entity.Property(s => s.FotoYolu).HasMaxLength(260);
            entity.Property(s => s.KayitNo).HasMaxLength(20);
            entity.HasIndex(s => s.KayitNo)
                .IsUnique()
                .HasFilter("\"SilindiMi\" = FALSE AND \"KayitNo\" IS NOT NULL");
            entity.Property(s => s.BabaAdi).HasMaxLength(100);
            entity.Property(s => s.AnaAdi).HasMaxLength(100);
            entity.Property(s => s.OgrenimDurumu).HasMaxLength(40);
            entity.Property(s => s.Meslek).HasMaxLength(100);
            entity.Property(s => s.EngelDurumu).HasMaxLength(40);
            entity.Property(s => s.NeredenGeldigi).HasMaxLength(150);
            entity.Property(s => s.OncekiYasamYeri).HasMaxLength(150);
            entity.Property(s => s.NufusKutukIli).HasMaxLength(50);
            entity.Property(s => s.UcretDurumu).HasMaxLength(40);
            entity.Property(s => s.AylikGelir).HasPrecision(18, 2);
            entity.Property(s => s.BasvuruDurumu).HasMaxLength(40);
            entity.Property(s => s.KayitTuru).HasMaxLength(40);
            entity.Property(s => s.AyrilisDurumu).HasMaxLength(40);
            entity.Property(s => s.KabulNedeni).HasMaxLength(80);
            entity.Property(s => s.KabulSekli).HasMaxLength(40);
            entity.Property(s => s.SonOturduguAdres).HasMaxLength(300);
            entity.Property(s => s.KimGetirdi).HasMaxLength(150);
            // Mevcut şemada Sakin.YatakId ayrı kolon, ilişki gölge kolon YatakId2 üzerinden duruyor.
            entity.HasOne(s => s.Yatak)
                .WithMany()
                .HasForeignKey("YatakId2");
        });

        modelBuilder.Entity<IlacTakip>(entity =>
        {
            entity.ToTable("IlacTakipleri");
            entity.Property(i => i.IlacAdi).IsRequired().HasMaxLength(150);
            entity.Property(i => i.Dozaj).IsRequired().HasMaxLength(100);
            entity.Property(i => i.Zaman).IsRequired().HasMaxLength(100);
            entity.Property(i => i.Notlar).HasMaxLength(300);
            entity.HasOne(i => i.Sakin)
                .WithMany(s => s.IlacTakipleri)
                .HasForeignKey(i => i.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Kullanici>(entity =>
        {
            entity.ToTable("Kullanicilar");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.HasIndex(k => k.KullaniciAdi).IsUnique();
            entity.HasIndex(k => k.Eposta).IsUnique();
            entity.Property(k => k.KullaniciAdi).IsRequired().HasMaxLength(50);
            entity.Property(k => k.Ad).IsRequired().HasMaxLength(100);
            entity.Property(k => k.Soyad).IsRequired().HasMaxLength(100);
            entity.Property(k => k.Eposta).IsRequired().HasMaxLength(150);
            entity.Property(k => k.Telefon).HasMaxLength(20);
            entity.Property(k => k.SifreHash).IsRequired().HasMaxLength(200);
            entity.Property(k => k.Rol).IsRequired().HasMaxLength(50);
            entity.Property(k => k.SifreSifirlamaTokenHash).HasMaxLength(200);
            entity.Property(k => k.TcKimlikNo).HasMaxLength(11);
            entity.HasOne(k => k.Personel).WithMany().HasForeignKey(k => k.PersonelId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<UygulamaAyari>(entity =>
        {
            entity.ToTable("UygulamaAyarlari");
            entity.HasData(new UygulamaAyari
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
            });
        });

        modelBuilder.Entity<Yakin>(entity =>
        {
            entity.ToTable("Yakinlar");
            entity.HasQueryFilter(y => !y.SilindiMi);
            entity.Property(y => y.Ad).IsRequired().HasMaxLength(100);
            entity.Property(y => y.Soyad).IsRequired().HasMaxLength(100);
            entity.Property(y => y.Yakinlik).IsRequired().HasMaxLength(30);
            entity.Property(y => y.Telefon).HasMaxLength(20);
            entity.Property(y => y.Eposta).HasMaxLength(150);
            entity.Property(y => y.Adres).HasMaxLength(300);
            entity.Property(y => y.Meslek).HasMaxLength(100);
            entity.Property(y => y.Aciklama).HasMaxLength(500);
            entity.HasOne(y => y.Sakin)
                .WithMany(s => s.Yakinlar)
                .HasForeignKey(y => y.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Ziyaret>(entity =>
        {
            entity.ToTable("Ziyaretler");
            entity.HasQueryFilter(z => !z.SilindiMi);
            entity.Property(z => z.ZiyaretciAd).IsRequired().HasMaxLength(100);
            entity.Property(z => z.ZiyaretciSoyad).IsRequired().HasMaxLength(100);
            entity.Property(z => z.TcKimlikNo).HasMaxLength(11);
            entity.Property(z => z.Telefon).HasMaxLength(20);
            entity.Property(z => z.Yakinlik).HasMaxLength(50);
            entity.Property(z => z.Notlar).HasMaxLength(300);
            entity.HasOne(z => z.Sakin)
                .WithMany(s => s.Ziyaretler)
                .HasForeignKey(z => z.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(z => z.CikisTarihi);
        });

        modelBuilder.Entity<Vasi>(entity =>
        {
            entity.ToTable("Vasiler");
            entity.HasQueryFilter(v => !v.SilindiMi);
            entity.Property(v => v.Ad).IsRequired().HasMaxLength(100);
            entity.Property(v => v.Soyad).IsRequired().HasMaxLength(100);
            entity.Property(v => v.TcKimlikNo).HasMaxLength(11);
            entity.Property(v => v.Telefon).HasMaxLength(20);
            entity.Property(v => v.Eposta).HasMaxLength(150);
            entity.Property(v => v.Adres).HasMaxLength(300);
            entity.Property(v => v.Yakinlik).HasMaxLength(30);
            entity.Property(v => v.MahkemeAdi).HasMaxLength(150);
            entity.Property(v => v.KararNo).HasMaxLength(50);
            entity.Property(v => v.Kapsam).HasMaxLength(200);
            entity.Property(v => v.VasiTuru).IsRequired().HasMaxLength(30);
            entity.Property(v => v.Sebep).HasMaxLength(200);
            entity.Property(v => v.Durum).IsRequired().HasMaxLength(20);
            entity.Property(v => v.Aciklama).HasMaxLength(500);
            entity.HasOne(v => v.Sakin)
                .WithMany(s => s.Vasiler)
                .HasForeignKey(v => v.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Mirasci>(entity =>
        {
            entity.ToTable("Mirascilar");
            entity.HasQueryFilter(m => !m.SilindiMi);
            entity.Property(m => m.Ad).IsRequired().HasMaxLength(100);
            entity.Property(m => m.Soyad).IsRequired().HasMaxLength(100);
            entity.Property(m => m.TcKimlikNo).HasMaxLength(11);
            entity.Property(m => m.Telefon).HasMaxLength(20);
            entity.Property(m => m.Yakinlik).IsRequired().HasMaxLength(30);
            entity.Property(m => m.Adres).HasMaxLength(300);
            entity.HasOne(m => m.Sakin)
                .WithMany(s => s.Mirascilar)
                .HasForeignKey(m => m.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SakinBelge>(entity =>
        {
            entity.ToTable("SakinBelgeleri");
            entity.HasQueryFilter(b => !b.SilindiMi);
            entity.Property(b => b.Grup).IsRequired().HasMaxLength(20);
            entity.Property(b => b.BelgeTuru).IsRequired().HasMaxLength(80);
            entity.Property(b => b.Aciklama).HasMaxLength(500);
            entity.Property(b => b.OrijinalAd).IsRequired().HasMaxLength(200);
            entity.Property(b => b.SaklamaYolu).IsRequired().HasMaxLength(260);
            entity.Property(b => b.IcerikTipi).IsRequired().HasMaxLength(100);
            entity.HasOne(b => b.Sakin)
                .WithMany(s => s.Belgeler)
                .HasForeignKey(b => b.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SakinMal>(entity =>
        {
            entity.ToTable("SakinMallari");
            entity.HasQueryFilter(m => !m.SilindiMi);
            entity.Property(m => m.MalTuru).IsRequired().HasMaxLength(40);
            entity.Property(m => m.Deger).HasPrecision(18, 2);
            entity.Property(m => m.Adres).HasMaxLength(300);
            entity.Property(m => m.Aciklama).HasMaxLength(500);
            entity.HasOne(m => m.Sakin)
                .WithMany(s => s.Mallar)
                .HasForeignKey(m => m.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SakinGelir>(entity =>
        {
            entity.ToTable("SakinGelirleri");
            entity.HasQueryFilter(g => !g.SilindiMi);
            entity.Property(g => g.GelirTuru).IsRequired().HasMaxLength(40);
            entity.Property(g => g.Periyot).IsRequired().HasMaxLength(20);
            entity.Property(g => g.Deger).HasPrecision(18, 2);
            entity.Property(g => g.Aciklama).HasMaxLength(500);
            entity.HasOne(g => g.Sakin)
                .WithMany(s => s.Gelirler)
                .HasForeignKey(g => g.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SakinSosyalGuvence>(entity =>
        {
            entity.ToTable("SakinSosyalGuvenceleri");
            entity.HasQueryFilter(g => !g.SilindiMi);
            entity.Property(g => g.GuvenceTuru).IsRequired().HasMaxLength(40);
            entity.Property(g => g.Aciklama).HasMaxLength(500);
            entity.HasOne(g => g.Sakin)
                .WithMany(s => s.SosyalGuvenceler)
                .HasForeignKey(g => g.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SakinGunlukIzin>(entity =>
        {
            entity.ToTable("SakinGunlukIzinleri");
            entity.HasQueryFilter(i => !i.SilindiMi);
            entity.Property(i => i.Yer).IsRequired().HasMaxLength(200);
            entity.Property(i => i.CikisSaati).HasMaxLength(5);
            entity.Property(i => i.DonusSaati).HasMaxLength(5);
            entity.HasOne(i => i.Sakin)
                .WithMany(s => s.GunlukIzinler)
                .HasForeignKey(i => i.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SakinEmanet>(entity =>
        {
            entity.ToTable("SakinEmanetleri");
            entity.HasQueryFilter(e => !e.SilindiMi);
            entity.Property(e => e.IslemTuru).IsRequired().HasMaxLength(30);
            entity.Property(e => e.EmanetTuru).IsRequired().HasMaxLength(40);
            entity.Property(e => e.Durum).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Deger).HasPrecision(18, 2);
            entity.Property(e => e.Saat).HasMaxLength(5);
            entity.Property(e => e.TeslimEden).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TeslimAlan).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Aciklama).HasMaxLength(500);
            entity.HasOne(e => e.Sakin)
                .WithMany(s => s.Emanetler)
                .HasForeignKey(e => e.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SakinYerlesim>(entity =>
        {
            entity.ToTable("SakinYerlesimleri");
            entity.HasQueryFilter(y => !y.SilindiMi);
            entity.Property(y => y.OdaNumarasi).IsRequired().HasMaxLength(20);
            entity.Property(y => y.YatakNumarasi).IsRequired().HasMaxLength(50);
            entity.Property(y => y.Blok).HasMaxLength(50);
            entity.HasOne(y => y.Sakin)
                .WithMany(s => s.Yerlesimler)
                .HasForeignKey(y => y.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(y => y.Oda)
                .WithMany()
                .HasForeignKey(y => y.OdaId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(y => y.Yatak)
                .WithMany()
                .HasForeignKey(y => y.YatakId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(y => new { y.SakinId, y.CikisTarihi });
        });

        modelBuilder.Entity<SakinOlcum>(entity =>
        {
            entity.ToTable("SakinOlcumleri");
            entity.HasQueryFilter(o => !o.SilindiMi);
            entity.Property(o => o.BoyCm).HasPrecision(6, 1);
            entity.Property(o => o.KiloKg).HasPrecision(6, 1);
            entity.Property(o => o.KanGrubu).HasMaxLength(20);
            entity.Property(o => o.Aciklama).HasMaxLength(500);
            entity.HasOne(o => o.Sakin)
                .WithMany(s => s.Olcumler)
                .HasForeignKey(o => o.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SakinSaglikDegerlendirme>(entity =>
        {
            entity.ToTable("SakinSaglikDegerlendirmeleri");
            entity.HasQueryFilter(d => !d.SilindiMi);
            entity.Property(d => d.Tur).IsRequired().HasMaxLength(20);
            entity.Property(d => d.Durum).IsRequired().HasMaxLength(40);
            entity.Property(d => d.Seviye).HasMaxLength(20);
            entity.Property(d => d.Taraf).HasMaxLength(20);
            entity.Property(d => d.Aciklama).HasMaxLength(500);
            entity.HasOne(d => d.Sakin)
                .WithMany(s => s.SaglikDegerlendirmeleri)
                .HasForeignKey(d => d.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(d => new { d.SakinId, d.Tur });
        });

        modelBuilder.Entity<SakinSaglikKayit>(entity =>
        {
            entity.ToTable("SakinSaglikKayitlari");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Tur).IsRequired().HasMaxLength(20);
            entity.Property(k => k.Ad).IsRequired().HasMaxLength(150);
            entity.Property(k => k.DurumTipi).HasMaxLength(40);
            entity.Property(k => k.Hastane).HasMaxLength(150);
            entity.Property(k => k.Komplikasyon).HasMaxLength(300);
            entity.Property(k => k.MarkaModel).HasMaxLength(100);
            entity.Property(k => k.SeriNo).HasMaxLength(80);
            entity.Property(k => k.Taraf).HasMaxLength(20);
            entity.Property(k => k.Derece).HasMaxLength(40);
            entity.Property(k => k.Bolge).HasMaxLength(80);
            entity.Property(k => k.Teshis).HasMaxLength(150);
            entity.Property(k => k.Doz).HasMaxLength(80);
            entity.Property(k => k.KullanimSikligi).HasMaxLength(80);
            entity.Property(k => k.UygulamaYolu).HasMaxLength(80);
            entity.Property(k => k.ReceteNo).HasMaxLength(80);
            entity.Property(k => k.ZamanlamaTipi).HasMaxLength(40);
            entity.Property(k => k.ZamanDilimleri).HasMaxLength(200);
            entity.Property(k => k.ReaksiyonTipi).HasMaxLength(80);
            entity.Property(k => k.Aciklama).HasMaxLength(500);
            entity.HasOne(k => k.Sakin)
                .WithMany(s => s.SaglikKayitlari)
                .HasForeignKey(k => k.SakinId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(k => new { k.SakinId, k.Tur });
        });

        modelBuilder.Entity<KurumSaglikKayit>(entity =>
        {
            entity.ToTable("KurumSaglikKayitlari");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Tur).IsRequired().HasMaxLength(30);
            entity.Property(k => k.Personel).HasMaxLength(120);
            entity.Property(k => k.Imzalayan).HasMaxLength(120);
            entity.Property(k => k.KanSekeri).HasPrecision(6, 1);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(k => new { k.Tur, k.Tarih });
        });

        modelBuilder.Entity<NarkotikIlac>(entity =>
        {
            entity.ToTable("NarkotikIlaclari");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Ad).IsRequired().HasMaxLength(120);
            entity.Property(k => k.Birim).HasMaxLength(20);
            entity.Property(k => k.Stok).HasPrecision(12, 2);
        });

        modelBuilder.Entity<NarkotikHareket>(entity =>
        {
            entity.ToTable("NarkotikHareketleri");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.HareketTuru).IsRequired().HasMaxLength(20);
            entity.Property(k => k.Miktar).HasPrecision(12, 2);
            entity.HasOne(k => k.Ilac).WithMany(i => i.Hareketler).HasForeignKey(k => k.NarkotikIlacId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<IlacEmri>(entity =>
        {
            entity.ToTable("IlacEmirleri");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.IlacAdi).IsRequired().HasMaxLength(150);
            entity.Property(k => k.KayitTuru).IsRequired().HasMaxLength(20);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IlacUygulama>(entity =>
        {
            entity.ToTable("IlacUygulamalari");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Durum).IsRequired().HasMaxLength(20);
            entity.HasOne(k => k.Emir).WithMany(e => e.Uygulamalar).HasForeignKey(k => k.IlacEmriId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(k => k.Tarih);
        });

        modelBuilder.Entity<OnayYetkisi>(entity =>
        {
            entity.ToTable("OnayYetkileri");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Alan).IsRequired().HasMaxLength(40);
            entity.HasOne(k => k.Kullanici).WithMany().HasForeignKey(k => k.KullaniciId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(k => new { k.KullaniciId, k.Alan });
        });

        modelBuilder.Entity<IzinSureci>(entity =>
        {
            entity.ToTable("IzinSurecleri");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.IzinTuru).IsRequired().HasMaxLength(40);
            entity.Property(k => k.OnayDurumu).IsRequired().HasMaxLength(20);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EsyaTespit>(entity =>
        {
            entity.ToTable("EsyaTespitleri");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Kategori).IsRequired().HasMaxLength(80);
            entity.Property(k => k.OnayDurumu).IsRequired().HasMaxLength(20);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MirasciTeslim>(entity =>
        {
            entity.ToTable("MirasciTeslimleri");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.TeslimAlan).IsRequired().HasMaxLength(120);
            entity.Property(k => k.OnayDurumu).IsRequired().HasMaxLength(20);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(k => k.Mirasci).WithMany().HasForeignKey(k => k.MirasciId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SosyalInceleme>(entity =>
        {
            entity.ToTable("SosyalIncelemeler");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.AdSoyad).IsRequired().HasMaxLength(150);
            entity.Property(k => k.Durum).IsRequired().HasMaxLength(30);
            entity.Property(k => k.OnayDurumu).IsRequired().HasMaxLength(20);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PsikolojikDegerlendirme>(entity =>
        {
            entity.ToTable("PsikolojikDegerlendirmeler");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Tur).IsRequired().HasMaxLength(20);
            entity.Property(k => k.OnayDurumu).IsRequired().HasMaxLength(20);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Yemek>(entity =>
        {
            entity.ToTable("Yemekler");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Ad).IsRequired().HasMaxLength(150);
            entity.Property(k => k.Kategori).IsRequired().HasMaxLength(40);
            entity.Property(k => k.Kalori).HasPrecision(8, 1);
            entity.Property(k => k.Protein).HasPrecision(8, 1);
            entity.Property(k => k.Karbonhidrat).HasPrecision(8, 1);
            entity.Property(k => k.Yag).HasPrecision(8, 1);
        });

        modelBuilder.Entity<BeslenmeProfili>(entity =>
        {
            entity.ToTable("BeslenmeProfilleri");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(k => k.SakinId);
        });

        modelBuilder.Entity<GunlukMenu>(entity =>
        {
            entity.ToTable("GunlukMenuler");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.OgunTipi).IsRequired().HasMaxLength(20);
            entity.HasOne(k => k.Yemek).WithMany().HasForeignKey(k => k.YemekId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(k => k.Tarih);
        });

        modelBuilder.Entity<OzelMenuPlani>(entity =>
        {
            entity.ToTable("OzelMenuPlanlari");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.OgunTipi).IsRequired().HasMaxLength(20);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(k => k.Yemek).WithMany().HasForeignKey(k => k.YemekId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<YemekTuketim>(entity =>
        {
            entity.ToTable("YemekTuketimleri");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.OgunTipi).IsRequired().HasMaxLength(20);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(k => k.Yemek).WithMany().HasForeignKey(k => k.YemekId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(k => k.Tarih);
        });

        modelBuilder.Entity<SiviAlimi>(entity =>
        {
            entity.ToTable("SiviAlimlari");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.SiviTuru).IsRequired().HasMaxLength(30);
            entity.Property(k => k.MiktarMl).HasPrecision(8, 1);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<KutuphaneDolap>(entity =>
        {
            entity.ToTable("KutuphaneDolaplari");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Ad).IsRequired().HasMaxLength(80);
        });

        modelBuilder.Entity<KutuphaneRaf>(entity =>
        {
            entity.ToTable("KutuphaneRaflari");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Ad).IsRequired().HasMaxLength(80);
            entity.HasOne(k => k.Dolap).WithMany(d => d.Raflar).HasForeignKey(k => k.DolapId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Kitap>(entity =>
        {
            entity.ToTable("Kitaplar");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Ad).IsRequired().HasMaxLength(200);
            entity.Property(k => k.Isbn).HasMaxLength(20);
        });

        modelBuilder.Entity<KitapKopya>(entity =>
        {
            entity.ToTable("KitapKopyalari");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.Property(k => k.Barkod).IsRequired().HasMaxLength(40);
            entity.Property(k => k.Durum).IsRequired().HasMaxLength(20);
            entity.HasOne(k => k.Kitap).WithMany(b => b.Kopyalar).HasForeignKey(k => k.KitapId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(k => k.Raf).WithMany().HasForeignKey(k => k.RafId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<KitapOdunc>(entity =>
        {
            entity.ToTable("KitapOduncleri");
            entity.HasQueryFilter(k => !k.SilindiMi);
            entity.HasOne(k => k.Kopya).WithMany().HasForeignKey(k => k.KopyaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(k => k.Sakin).WithMany().HasForeignKey(k => k.SakinId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Kurulus>(entity =>
        {
            entity.ToTable("Kuruluslar");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(150);
            entity.Property(x => x.KisaAd).IsRequired().HasMaxLength(40);
            entity.Property(x => x.PlakaKodu).IsRequired().HasMaxLength(2);
            entity.Property(x => x.Adres).HasMaxLength(400);
            entity.Property(x => x.Telefon).HasMaxLength(20);
            entity.Property(x => x.Dahili).HasMaxLength(20);
        });

        modelBuilder.Entity<KullaniciKurulus>(entity =>
        {
            entity.ToTable("KullaniciKuruluslari");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.HasIndex(x => new { x.KullaniciId, x.KurulusId }).IsUnique().HasFilter("\"SilindiMi\" = FALSE");
            entity.Property(x => x.Rol).IsRequired().HasMaxLength(50);
            entity.HasOne(x => x.Kullanici).WithMany().HasForeignKey(x => x.KullaniciId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Kurulus).WithMany().HasForeignKey(x => x.KurulusId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ulke>(entity =>
        {
            entity.ToTable("Ulkeler");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(80);
            entity.Property(x => x.Kod).IsRequired().HasMaxLength(8);
        });

        modelBuilder.Entity<Il>(entity =>
        {
            entity.ToTable("Iller");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(50);
            entity.Property(x => x.PlakaKodu).IsRequired().HasMaxLength(2);
            entity.HasOne(x => x.Ulke).WithMany().HasForeignKey(x => x.UlkeId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ilce>(entity =>
        {
            entity.ToTable("Ilceler");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(80);
            entity.HasOne(x => x.Il).WithMany().HasForeignKey(x => x.IlId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Mahalle>(entity =>
        {
            entity.ToTable("Mahalleler");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(80);
            entity.HasOne(x => x.Ilce).WithMany().HasForeignKey(x => x.IlceId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GlobalTanim>(entity =>
        {
            entity.ToTable("GlobalTanimlar");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.Property(x => x.Kategori).IsRequired().HasMaxLength(40);
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Kod).HasMaxLength(40);
        });

        modelBuilder.Entity<Personel>(entity =>
        {
            entity.ToTable("Personeller");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.HasIndex(x => new { x.KurulusId, x.SicilNo }).IsUnique().HasFilter("\"SilindiMi\" = FALSE");
            entity.Property(x => x.SicilNo).IsRequired().HasMaxLength(30);
            entity.Property(x => x.TcKimlikNo).HasMaxLength(11);
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Soyad).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Unvan).HasMaxLength(80);
            entity.Property(x => x.Meslek).HasMaxLength(80);
            entity.Property(x => x.Gorev).HasMaxLength(80);
            entity.Property(x => x.Durum).IsRequired().HasMaxLength(20);
            entity.HasOne(x => x.Kurulus).WithMany().HasForeignKey(x => x.KurulusId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Ulke).WithMany().HasForeignKey(x => x.UlkeId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(x => x.Il).WithMany().HasForeignKey(x => x.IlId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(x => x.Ilce).WithMany().HasForeignKey(x => x.IlceId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(x => x.Mahalle).WithMany().HasForeignKey(x => x.MahalleId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PersonelYakin>(entity =>
        {
            entity.ToTable("PersonelYakinlari");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Soyad).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Yakinlik).IsRequired().HasMaxLength(40);
            entity.HasOne(x => x.Personel).WithMany().HasForeignKey(x => x.PersonelId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PersonelBelge>(entity =>
        {
            entity.ToTable("PersonelBelgeleri");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.Property(x => x.BelgeTuru).IsRequired().HasMaxLength(80);
            entity.HasOne(x => x.Personel).WithMany().HasForeignKey(x => x.PersonelId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrganizasyonBirimi>(entity =>
        {
            entity.ToTable("OrganizasyonBirimleri");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Kod).HasMaxLength(30);
            entity.HasOne(x => x.Kurulus).WithMany().HasForeignKey(x => x.KurulusId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.UstBirim).WithMany().HasForeignKey(x => x.UstBirimId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PersonelAtama>(entity =>
        {
            entity.ToTable("PersonelAtamalari");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.Property(x => x.Gorev).HasMaxLength(80);
            entity.HasOne(x => x.Personel).WithMany().HasForeignKey(x => x.PersonelId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Birim).WithMany().HasForeignKey(x => x.BirimId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Roller");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.HasIndex(x => x.Kod).IsUnique().HasFilter("\"SilindiMi\" = FALSE");
            entity.Property(x => x.Kod).IsRequired().HasMaxLength(40);
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(80);
        });

        modelBuilder.Entity<RolIzin>(entity =>
        {
            entity.ToTable("RolIzinleri");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.HasIndex(x => new { x.RolId, x.IzinKodu }).IsUnique().HasFilter("\"SilindiMi\" = FALSE");
            entity.Property(x => x.IzinKodu).IsRequired().HasMaxLength(80);
            entity.HasOne(x => x.Rol).WithMany().HasForeignKey(x => x.RolId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PdfSablon>(entity =>
        {
            entity.ToTable("PdfSablonlari");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.HasIndex(x => x.Kod).IsUnique().HasFilter("\"SilindiMi\" = FALSE");
            entity.Property(x => x.Kod).IsRequired().HasMaxLength(40);
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Baslik).IsRequired().HasMaxLength(160);
        });

        modelBuilder.Entity<PdfArsiv>(entity =>
        {
            entity.ToTable("PdfArsivleri");
            entity.HasQueryFilter(x => !x.SilindiMi);
            entity.Property(x => x.DosyaAdi).IsRequired().HasMaxLength(160);
            entity.Property(x => x.Olusturan).IsRequired().HasMaxLength(120);
            entity.HasOne(x => x.Sablon).WithMany().HasForeignKey(x => x.SablonId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Sakin).WithMany().HasForeignKey(x => x.SakinId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<DenetimKaydi>(entity =>
        {
            entity.ToTable("DenetimKayitlari");
            entity.HasIndex(x => x.OlusturulmaTarihi);
            entity.HasIndex(x => x.Tur);
            entity.Property(x => x.Tur).IsRequired().HasMaxLength(20);
            entity.Property(x => x.Islem).IsRequired().HasMaxLength(80);
            entity.Property(x => x.Aciklama).IsRequired().HasMaxLength(400);
            entity.Property(x => x.KullaniciAdi).HasMaxLength(80);
            entity.Property(x => x.IpAdresi).HasMaxLength(64);
            entity.Property(x => x.Yol).HasMaxLength(240);
            entity.Property(x => x.HataTipi).HasMaxLength(120);
        });

        modelBuilder.Entity<YedekKaydi>(entity =>
        {
            entity.ToTable("YedekKayitlari");
            entity.Property(x => x.Ad).IsRequired().HasMaxLength(80);
            entity.Property(x => x.Tur).IsRequired().HasMaxLength(20);
            entity.Property(x => x.Durum).IsRequired().HasMaxLength(20);
            entity.Property(x => x.Olusturan).IsRequired().HasMaxLength(80);
            entity.Property(x => x.VeritabaniYolu).IsRequired().HasMaxLength(160);
            entity.Property(x => x.DosyaArsivYolu).HasMaxLength(160);
        });
    }
}
