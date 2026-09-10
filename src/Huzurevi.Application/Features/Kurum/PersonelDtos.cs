using FluentValidation;

namespace Huzurevi.Application.Features.Kurum;

public record PersonelListDto(int Id, string SicilNo, string Ad, string Soyad, string? Unvan, string? Gorev, string Durum, string KurulusAd, bool FotoVarMi);
public record PersonelDto(
    int Id, int KurulusId, string SicilNo, string? TcKimlikNo, string Ad, string Soyad, string? Unvan, string? Meslek, string? Gorev, string Durum,
    DateTime? DogumTarihi, string? Cinsiyet, string? KanGrubu, string? Eposta, string? Telefon, string? Adres,
    int? UlkeId, int? IlId, int? IlceId, int? MahalleId, DateTime? IseBaslamaTarihi, DateTime? AyrilisTarihi, string? Notlar, bool FotoVarMi);
public record PersonelIstek(
    int KurulusId, string SicilNo, string? TcKimlikNo, string Ad, string Soyad, string? Unvan, string? Meslek, string? Gorev, string Durum,
    DateTime? DogumTarihi, string? Cinsiyet, string? KanGrubu, string? Eposta, string? Telefon, string? Adres,
    int? UlkeId, int? IlId, int? IlceId, int? MahalleId, DateTime? IseBaslamaTarihi, DateTime? AyrilisTarihi, string? Notlar);

public record PersonelYakinDto(int Id, string Ad, string Soyad, string Yakinlik, string? Telefon, string? Adres);
public record PersonelYakinIstek(string Ad, string Soyad, string Yakinlik, string? Telefon, string? Adres);
public record PersonelBelgeDto(int Id, string BelgeTuru, DateTime? BelgeTarihi, string? Aciklama, string OrijinalAd);

public class PersonelIstekDogrulayici : AbstractValidator<PersonelIstek>
{
    public PersonelIstekDogrulayici()
    {
        RuleFor(x => x.KurulusId).GreaterThan(0);
        RuleFor(x => x.SicilNo).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Durum).Must(v => KurumSabitleri.PersonelDurumlari.Contains(v));
        RuleFor(x => x.TcKimlikNo).Length(11).Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.TcKimlikNo));
    }
}

public class PersonelYakinIstekDogrulayici : AbstractValidator<PersonelYakinIstek>
{
    public PersonelYakinIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Soyad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Yakinlik).NotEmpty().MaximumLength(40);
    }
}

public record OrganizasyonBirimDto(int Id, int KurulusId, int? UstBirimId, string Ad, string? Kod, bool AktifMi);
public record OrganizasyonBirimIstek(int KurulusId, int? UstBirimId, string Ad, string? Kod, bool AktifMi);
public record PersonelAtamaDto(int Id, int PersonelId, string PersonelAd, int BirimId, string BirimAd, string? Gorev, DateTime BaslangicTarihi, DateTime? BitisTarihi, bool AktifMi);
public record PersonelAtamaIstek(int PersonelId, int BirimId, string? Gorev, DateTime BaslangicTarihi, DateTime? BitisTarihi, bool AktifMi);

public class OrganizasyonBirimIstekDogrulayici : AbstractValidator<OrganizasyonBirimIstek>
{
    public OrganizasyonBirimIstekDogrulayici()
    {
        RuleFor(x => x.KurulusId).GreaterThan(0);
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(120);
    }
}

public class PersonelAtamaIstekDogrulayici : AbstractValidator<PersonelAtamaIstek>
{
    public PersonelAtamaIstekDogrulayici()
    {
        RuleFor(x => x.PersonelId).GreaterThan(0);
        RuleFor(x => x.BirimId).GreaterThan(0);
        RuleFor(x => x.BaslangicTarihi).NotEmpty();
    }
}
