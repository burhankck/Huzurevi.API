using Huzurevi.Application.Features.KurumSurec;

namespace Huzurevi.Application.Common.Interfaces;

public interface IKurumSurecPdfUretici
{
    byte[] EsyaTespitUret(string sakinAd, DateTime tarih, IReadOnlyList<EsyaTespitDto> kayitlar);
    byte[] TeslimTutanagiUret(MirasciTeslimDto kayit);
}
