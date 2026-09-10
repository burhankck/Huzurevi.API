namespace Huzurevi.Application.Common.Interfaces;

public record TabloCikti(byte[] Icerik, string IcerikTipi, string DosyaAdi);

public interface ITabloCiktiUretici
{
    TabloCikti Excel(string baslik, IReadOnlyList<string> kolonlar, IReadOnlyList<IReadOnlyList<string>> satirlar);
    TabloCikti Pdf(string baslik, IReadOnlyList<string> kolonlar, IReadOnlyList<IReadOnlyList<string>> satirlar);
    byte[] MetinPdf(string baslik, string govde);
}
