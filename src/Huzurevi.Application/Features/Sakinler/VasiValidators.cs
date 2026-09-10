using FluentValidation;

namespace Huzurevi.Application.Features.Sakinler;

internal static class VasiKurallari
{
    public static readonly string[] Turler = ["Yasal Vasi", "Kayyım", "Vesayet", "Diğer"];
    public static readonly string[] Durumlar = ["Aktif", "Sona Erdi"];
}

public class VasiOlusturIstekDogrulayici : AbstractValidator<VasiOlusturIstek>
{
    public VasiOlusturIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TcKimlikNo)
            .Length(11)
            .Matches(@"^\d{11}$")
            .When(x => !string.IsNullOrWhiteSpace(x.TcKimlikNo))
            .WithMessage("T.C. Kimlik No 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Eposta).EmailAddress().MaximumLength(150).When(x => !string.IsNullOrWhiteSpace(x.Eposta));
        RuleFor(x => x.Adres).MaximumLength(300);
        RuleFor(x => x.Yakinlik).MaximumLength(30);
        RuleFor(x => x.MahkemeAdi).MaximumLength(150);
        RuleFor(x => x.KararNo).MaximumLength(50);
        RuleFor(x => x.Kapsam).MaximumLength(200);
        RuleFor(x => x.Sebep).MaximumLength(200);
        RuleFor(x => x.Aciklama).MaximumLength(500);
        RuleFor(x => x.VasiTuru)
            .Must(v => v is null || VasiKurallari.Turler.Contains(v))
            .WithMessage("Vasi türü geçersiz.");
        RuleFor(x => x.Durum)
            .Must(v => v is null || VasiKurallari.Durumlar.Contains(v))
            .WithMessage("Vasilik durumu Aktif veya Sona Erdi olmalıdır.");
        RuleFor(x => x)
            .Must(x => x.BitisTarihi is null || x.BaslangicTarihi is null || x.BitisTarihi >= x.BaslangicTarihi)
            .WithMessage("Vasilik bitiş tarihi başlangıçtan önce olamaz.");
    }
}

public class VasiGuncelleIstekDogrulayici : AbstractValidator<VasiGuncelleIstek>
{
    public VasiGuncelleIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TcKimlikNo)
            .Length(11)
            .Matches(@"^\d{11}$")
            .When(x => !string.IsNullOrWhiteSpace(x.TcKimlikNo))
            .WithMessage("T.C. Kimlik No 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Telefon).MaximumLength(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telefon)).WithMessage("Telefon en fazla 11 haneli rakam olmalıdır.");
        RuleFor(x => x.Eposta).EmailAddress().MaximumLength(150).When(x => !string.IsNullOrWhiteSpace(x.Eposta));
        RuleFor(x => x.Adres).MaximumLength(300);
        RuleFor(x => x.Yakinlik).MaximumLength(30);
        RuleFor(x => x.MahkemeAdi).MaximumLength(150);
        RuleFor(x => x.KararNo).MaximumLength(50);
        RuleFor(x => x.Kapsam).MaximumLength(200);
        RuleFor(x => x.Sebep).MaximumLength(200);
        RuleFor(x => x.Aciklama).MaximumLength(500);
        RuleFor(x => x.VasiTuru)
            .Must(v => v is null || VasiKurallari.Turler.Contains(v))
            .WithMessage("Vasi türü geçersiz.");
        RuleFor(x => x.Durum)
            .Must(v => v is null || VasiKurallari.Durumlar.Contains(v))
            .WithMessage("Vasilik durumu Aktif veya Sona Erdi olmalıdır.");
        RuleFor(x => x)
            .Must(x => x.BitisTarihi is null || x.BaslangicTarihi is null || x.BitisTarihi >= x.BaslangicTarihi)
            .WithMessage("Vasilik bitiş tarihi başlangıçtan önce olamaz.");
    }
}

public class BelgeYukleIstekDogrulayici : AbstractValidator<BelgeYukleIstek>
{
    public BelgeYukleIstekDogrulayici()
    {
        RuleFor(x => x.BelgeTuru).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Aciklama).MaximumLength(500);
    }
}

public class BelgeGuncelleIstekDogrulayici : AbstractValidator<BelgeGuncelleIstek>
{
    public BelgeGuncelleIstekDogrulayici()
    {
        RuleFor(x => x.BelgeTuru).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Aciklama).MaximumLength(500);
    }
}
