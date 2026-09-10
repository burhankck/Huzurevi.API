using FluentValidation;

namespace Huzurevi.Application.Features.Yetkiler;

public record RolDto(int Id, string Kod, string Ad, bool SistemRoluMu, bool AktifMi, List<string> Izinler);
public record RolListDto(int Id, string Kod, string Ad, bool SistemRoluMu, bool AktifMi);
public record RolIstek(string Kod, string Ad, bool AktifMi, List<string> Izinler);
public record IzinTanimDto(string Kod, string Grup, string Islem);

public class RolIstekDogrulayici : AbstractValidator<RolIstek>
{
    public RolIstekDogrulayici()
    {
        RuleFor(x => x.Ad).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Kod).MaximumLength(40);
    }
}
