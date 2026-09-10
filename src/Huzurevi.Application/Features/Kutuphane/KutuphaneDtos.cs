using FluentValidation;

namespace Huzurevi.Application.Features.Kutuphane;

public static class KutuphaneSabitleri
{
    public static readonly string[] KopyaDurumlari = ["Rafta", "Odünçte", "Kayıp", "Bakımda"];
}

public record DolapDto(int Id, string Ad, string? Konum, bool AktifMi);
public record DolapIstek(string Ad, string? Konum, bool AktifMi);

public class DolapIstekDogrulayici : AbstractValidator<DolapIstek>
{
    public DolapIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(80);
    }
}

public record RafDto(int Id, int DolapId, string DolapAd, string Ad);
public record RafIstek(int DolapId, string Ad);

public class RafIstekDogrulayici : AbstractValidator<RafIstek>
{
    public RafIstekDogrulayici()
    {
        RuleFor(x => x.DolapId).GreaterThan(0);
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(80);
    }
}

public record KitapDto(int Id, string? Isbn, string Ad, string? Yazar, string? Yayinevi, string? Aciklama, bool KapakVarMi);
public record KitapIstek(string? Isbn, string Ad, string? Yazar, string? Yayinevi, string? Aciklama);

public class KitapIstekDogrulayici : AbstractValidator<KitapIstek>
{
    public KitapIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Isbn).MaximumLength(20);
    }
}

public record KitapKopyaDto(int Id, int KitapId, string KitapAd, string Barkod, string Durum, int? RafId, string? RafAd, string? Konum);
public record KitapKopyaIstek(int KitapId, string Barkod, string Durum, int? RafId, string? Konum);

public class KitapKopyaIstekDogrulayici : AbstractValidator<KitapKopyaIstek>
{
    public KitapKopyaIstekDogrulayici()
    {
        RuleFor(x => x.KitapId).GreaterThan(0);
        RuleFor(x => x.Barkod).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Durum).Must(v => KutuphaneSabitleri.KopyaDurumlari.Contains(v));
    }
}

public record KitapOduncDto(
    int Id, int KopyaId, string Barkod, string KitapAd, int SakinId, string SakinAd,
    DateTime OduncTarihi, DateTime PlanlananTeslim, DateTime? IadeTarihi, string? DurumNotu, bool GeciktiMi);

public record KitapOduncIstek(int KopyaId, int SakinId, DateTime OduncTarihi, DateTime PlanlananTeslim, string? DurumNotu);
public record KitapIadeIstek(DateTime IadeTarihi, string? DurumNotu, string KopyaDurumu);

public class KitapOduncIstekDogrulayici : AbstractValidator<KitapOduncIstek>
{
    public KitapOduncIstekDogrulayici()
    {
        RuleFor(x => x.KopyaId).GreaterThan(0);
        RuleFor(x => x.SakinId).GreaterThan(0);
        RuleFor(x => x.PlanlananTeslim).GreaterThanOrEqualTo(x => x.OduncTarihi);
    }
}

public class KitapIadeIstekDogrulayici : AbstractValidator<KitapIadeIstek>
{
    public KitapIadeIstekDogrulayici()
    {
        RuleFor(x => x.IadeTarihi).NotEmpty();
        RuleFor(x => x.KopyaDurumu).Must(v => KutuphaneSabitleri.KopyaDurumlari.Contains(v));
    }
}
