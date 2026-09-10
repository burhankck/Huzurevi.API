using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Huzurevi.Application.Common.Interfaces;

namespace Huzurevi.Infrastructure.Pdf;

public class TabloCiktiUretici : ITabloCiktiUretici
{
    public TabloCikti Excel(string baslik, IReadOnlyList<string> kolonlar, IReadOnlyList<IReadOnlyList<string>> satirlar)
    {
        using var kitap = new XLWorkbook();
        var sayfa = kitap.AddWorksheet(SayfaAdi(baslik));
        sayfa.Cell(1, 1).Value = baslik;
        sayfa.Range(1, 1, 1, Math.Max(kolonlar.Count, 1)).Merge().Style.Font.SetBold().Font.SetFontSize(14);
        for (var i = 0; i < kolonlar.Count; i++)
            sayfa.Cell(3, i + 1).Value = kolonlar[i];
        sayfa.Row(3).Style.Font.SetBold();
        for (var r = 0; r < satirlar.Count; r++)
        {
            for (var c = 0; c < kolonlar.Count; c++)
            {
                var deger = c < satirlar[r].Count ? satirlar[r][c] : "";
                sayfa.Cell(r + 4, c + 1).Value = deger;
            }
        }
        sayfa.Columns().AdjustToContents();
        using var bellek = new MemoryStream();
        kitap.SaveAs(bellek);
        return new TabloCikti(bellek.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Dosya(baslik, "xlsx"));
    }

    public TabloCikti Pdf(string baslik, IReadOnlyList<string> kolonlar, IReadOnlyList<IReadOnlyList<string>> satirlar)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var bytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(28);
                page.Header().Text(baslik).FontSize(16).SemiBold();
                page.Content().PaddingTop(10).Table(tablo =>
                {
                    tablo.ColumnsDefinition(c =>
                    {
                        foreach (var _ in kolonlar)
                            c.RelativeColumn();
                    });
                    tablo.Header(h =>
                    {
                        foreach (var kolon in kolonlar)
                            h.Cell().BorderBottom(1).Padding(4).Text(kolon).SemiBold();
                    });
                    if (satirlar.Count == 0)
                        tablo.Cell().ColumnSpan((uint)Math.Max(kolonlar.Count, 1)).PaddingTop(8).Text("Kayıt yok.");
                    foreach (var satir in satirlar)
                    {
                        for (var i = 0; i < kolonlar.Count; i++)
                            tablo.Cell().Padding(3).Text(i < satir.Count ? satir[i] : "");
                    }
                });
                page.Footer().AlignRight().Text(x =>
                {
                    x.Span("Huzurevi — ");
                    x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                });
            });
        }).GeneratePdf();
        return new TabloCikti(bytes, "application/pdf", Dosya(baslik, "pdf"));
    }

    public byte[] MetinPdf(string baslik, string govde)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.Header().Text(baslik).FontSize(18).SemiBold();
                page.Content().PaddingTop(16).Text(govde).FontSize(11).LineHeight(1.4f);
            });
        }).GeneratePdf();
    }

    private static string Dosya(string baslik, string uzanti)
    {
        var ad = new string(baslik.Where(c => char.IsLetterOrDigit(c) || c is ' ' or '-' or '_').ToArray()).Trim().Replace(' ', '_');
        if (string.IsNullOrWhiteSpace(ad)) ad = "cikti";
        return $"{ad}_{DateTime.Now:yyyyMMdd}.{uzanti}";
    }

    private static string SayfaAdi(string baslik)
    {
        var ad = new string(baslik.Take(28).ToArray());
        return string.IsNullOrWhiteSpace(ad) ? "Cikti" : ad;
    }
}
