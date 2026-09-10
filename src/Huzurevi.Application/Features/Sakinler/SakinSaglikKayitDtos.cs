using FluentValidation;

namespace Huzurevi.Application.Features.Sakinler;

public record SakinSaglikKayitDto(
    int Id,
    int SakinId,
    string Tur,
    DateTime Tarih,
    string Ad,
    string? DurumTipi,
    DateTime? BaslangicTarihi,
    DateTime? BitisTarihi,
    DateTime? AmeliyatTarihi,
    string? Hastane,
    string? Komplikasyon,
    string? MarkaModel,
    string? SeriNo,
    DateTime? TeminTarihi,
    DateTime? KontrolTarihi,
    string? Taraf,
    string? Derece,
    string? Bolge,
    string? Teshis,
    string? Doz,
    string? KullanimSikligi,
    string? UygulamaYolu,
    bool ReceteliMi,
    string? ReceteNo,
    string? ZamanlamaTipi,
    string? ZamanDilimleri,
    string? ReaksiyonTipi,
    string? Aciklama,
    bool AktifMi);

public record SakinSaglikKayitIstek(
    string Tur,
    DateTime Tarih,
    string Ad,
    string? DurumTipi,
    DateTime? BaslangicTarihi,
    DateTime? BitisTarihi,
    DateTime? AmeliyatTarihi,
    string? Hastane,
    string? Komplikasyon,
    string? MarkaModel,
    string? SeriNo,
    DateTime? TeminTarihi,
    DateTime? KontrolTarihi,
    string? Taraf,
    string? Derece,
    string? Bolge,
    string? Teshis,
    string? Doz,
    string? KullanimSikligi,
    string? UygulamaYolu,
    bool ReceteliMi,
    string? ReceteNo,
    string? ZamanlamaTipi,
    string? ZamanDilimleri,
    string? ReaksiyonTipi,
    string? Aciklama,
    bool AktifMi);

public class SakinSaglikKayitIstekDogrulayici : AbstractValidator<SakinSaglikKayitIstek>
{
    public SakinSaglikKayitIstekDogrulayici()
    {
        RuleFor(x => x.Tur).NotEmpty().Must(v => SakinSaglikKurallari.KayitTurleri.Contains(v));
        RuleFor(x => x.Tarih).NotEmpty();
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Aciklama).MaximumLength(500);
        RuleFor(x => x.Hastane).MaximumLength(150);
        RuleFor(x => x.Komplikasyon).MaximumLength(300);
    }
}
