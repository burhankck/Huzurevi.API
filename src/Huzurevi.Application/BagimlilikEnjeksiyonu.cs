using FluentValidation;
using Huzurevi.Application.Features.Anasayfa;
using Huzurevi.Application.Features.Ayarlar;
using Huzurevi.Application.Features.IlacTakipleri;
using Huzurevi.Application.Features.Kimlik;
using Huzurevi.Application.Features.Kullanicilar;
using Huzurevi.Application.Features.Odalar;
using Huzurevi.Application.Features.KurumSaglik;
using Huzurevi.Application.Features.KurumSurec;
using Huzurevi.Application.Features.Kutuphane;
using Huzurevi.Application.Features.Kurum;
using Huzurevi.Application.Features.Sakinler;
using Huzurevi.Application.Features.Yemekhane;
using Huzurevi.Application.Features.Yataklar;
using Huzurevi.Application.Features.Cikti;
using Huzurevi.Application.Features.Denetim;
using Huzurevi.Application.Features.Yetkiler;
using Huzurevi.Application.Features.Ziyaretler;
using Microsoft.Extensions.DependencyInjection;

namespace Huzurevi.Application;

public static class BagimlilikEnjeksiyonu
{
    public static IServiceCollection UygulamaKatmaniniEkle(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(BagimlilikEnjeksiyonu));

        services.AddScoped<IOdaServisi, OdaServisi>();
        services.AddScoped<IYatakServisi, YatakServisi>();
        services.AddScoped<ISakinServisi, SakinServisi>();
        services.AddScoped<IIlacTakipServisi, IlacTakipServisi>();
        services.AddScoped<IAnasayfaServisi, AnasayfaServisi>();
        services.AddScoped<IKimlikServisi, KimlikServisi>();
        services.AddScoped<IAyarServisi, AyarServisi>();
        services.AddScoped<IKullaniciServisi, KullaniciServisi>();
        services.AddScoped<IZiyaretServisi, ZiyaretServisi>();
        services.AddScoped<IVasiServisi, VasiServisi>();
        services.AddScoped<IMirasciServisi, MirasciServisi>();
        services.AddScoped<ISakinBelgeServisi, SakinBelgeServisi>();
        services.AddScoped<ISakinMalServisi, SakinMalServisi>();
        services.AddScoped<ISakinGelirServisi, SakinGelirServisi>();
        services.AddScoped<ISakinSosyalGuvenceServisi, SakinSosyalGuvenceServisi>();
        services.AddScoped<ISakinGunlukIzinServisi, SakinGunlukIzinServisi>();
        services.AddScoped<ISakinEmanetServisi, SakinEmanetServisi>();
        services.AddScoped<ISakinOlcumServisi, SakinOlcumServisi>();
        services.AddScoped<ISakinSaglikDegerlendirmeServisi, SakinSaglikDegerlendirmeServisi>();
        services.AddScoped<ISakinSaglikKayitServisi, SakinSaglikKayitServisi>();
        services.AddScoped<IKurumSaglikKayitServisi, KurumSaglikKayitServisi>();
        services.AddScoped<INarkotikServisi, NarkotikServisi>();
        services.AddScoped<IIlacEmriServisi, IlacEmriServisi>();
        services.AddScoped<IOnayYetkiServisi, OnayYetkiServisi>();
        services.AddScoped<IIzinSureciServisi, IzinSureciServisi>();
        services.AddScoped<IEsyaTespitServisi, EsyaTespitServisi>();
        services.AddScoped<IMirasciTeslimServisi, MirasciTeslimServisi>();
        services.AddScoped<ISosyalIncelemeServisi, SosyalIncelemeServisi>();
        services.AddScoped<IPsikolojikDegerlendirmeServisi, PsikolojikDegerlendirmeServisi>();
        services.AddScoped<IYemekhaneServisi, YemekhaneServisi>();
        services.AddScoped<IKutuphaneServisi, KutuphaneServisi>();
        services.AddScoped<IKurulusServisi, KurulusServisi>();
        services.AddScoped<IAdresTanimServisi, AdresTanimServisi>();
        services.AddScoped<IPersonelServisi, PersonelServisi>();
        services.AddScoped<IOrganizasyonServisi, OrganizasyonServisi>();
        services.AddScoped<IYetkiServisi, YetkiServisi>();
        services.AddScoped<IRolServisi, RolServisi>();
        services.AddScoped<ICiktiServisi, CiktiServisi>();
        services.AddScoped<IPdfSablonServisi, PdfSablonServisi>();
        services.AddScoped<IDenetimServisi, DenetimServisi>();

        return services;
    }
}
