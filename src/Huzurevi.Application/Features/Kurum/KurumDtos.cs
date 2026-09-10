using FluentValidation;

namespace Huzurevi.Application.Features.Kurum;

public static class KurumSabitleri
{
    public static readonly string[] Roller = ["Yonetici", "Personel"];
    public static readonly string[] TanimKategorileri = ["Saglik", "Bakim", "Oda", "Personel", "Finans", "Diger"];
    public static readonly string[] PersonelDurumlari = ["Aktif", "İzinli", "Ayrıldı"];
}

public record KurulusDto(int Id, string Ad, string KisaAd, string PlakaKodu, string? Adres, string? Telefon, string? Dahili, bool AktifMi);
public record KurulusIstek(string Ad, string KisaAd, string PlakaKodu, string? Adres, string? Telefon, string? Dahili, bool AktifMi);
public record KurulusSecimDto(int Id, string Ad, string Rol);
public record AdresVarsayilanDto(int? UlkeId, string? UlkeAd, int? IlId, string? IlAd, string PlakaKodu);

public class KurulusIstekDogrulayici : AbstractValidator<KurulusIstek>
{
    public KurulusIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(150);
        RuleFor(x => x.KisaAd).NotEmpty().MaximumLength(40);
        RuleFor(x => x.PlakaKodu).NotEmpty().Length(2).Matches(@"^\d{2}$");
    }
}

public record UlkeDto(int Id, string Ad, string Kod);
public record UlkeIstek(string Ad, string Kod);
public record IlDto(int Id, int UlkeId, string UlkeAd, string Ad, string PlakaKodu);
public record IlIstek(int UlkeId, string Ad, string PlakaKodu);
public record IlceDto(int Id, int IlId, string IlAd, string Ad);
public record IlceIstek(int IlId, string Ad);
public record MahalleDto(int Id, int IlceId, string IlceAd, string Ad);
public record MahalleIstek(int IlceId, string Ad);
public record GlobalTanimDto(int Id, string Kategori, string Ad, string? Kod, bool AktifMi);
public record GlobalTanimIstek(string Kategori, string Ad, string? Kod, bool AktifMi);

public class UlkeIstekDogrulayici : AbstractValidator<UlkeIstek>
{
    public UlkeIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Kod).NotEmpty().MaximumLength(8);
    }
}
public class IlIstekDogrulayici : AbstractValidator<IlIstek>
{
    public IlIstekDogrulayici()
    {
        RuleFor(x => x.UlkeId).GreaterThan(0);
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PlakaKodu).NotEmpty().Length(2);
    }
}
public class IlceIstekDogrulayici : AbstractValidator<IlceIstek>
{
    public IlceIstekDogrulayici()
    {
        RuleFor(x => x.IlId).GreaterThan(0);
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(80);
    }
}
public class MahalleIstekDogrulayici : AbstractValidator<MahalleIstek>
{
    public MahalleIstekDogrulayici()
    {
        RuleFor(x => x.IlceId).GreaterThan(0);
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(80);
    }
}
public class GlobalTanimIstekDogrulayici : AbstractValidator<GlobalTanimIstek>
{
    public GlobalTanimIstekDogrulayici()
    {
        RuleFor(x => x.Kategori).Must(v => KurumSabitleri.TanimKategorileri.Contains(v));
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(120);
    }
}
