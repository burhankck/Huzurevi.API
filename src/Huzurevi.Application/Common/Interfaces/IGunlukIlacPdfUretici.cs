using Huzurevi.Application.Features.KurumSaglik;

namespace Huzurevi.Application.Common.Interfaces;

public interface IGunlukIlacPdfUretici
{
    byte[] Uret(DateTime tarih, IReadOnlyList<IlacUygulamaDto> kayitlar);
}
