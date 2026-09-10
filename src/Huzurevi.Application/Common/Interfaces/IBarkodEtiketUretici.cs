using Huzurevi.Application.Features.Kutuphane;

namespace Huzurevi.Application.Common.Interfaces;

public interface IBarkodEtiketUretici
{
    byte[] PngUret(string barkod, string kitapAd);
}
