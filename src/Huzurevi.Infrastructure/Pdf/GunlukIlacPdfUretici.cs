using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.KurumSaglik;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Huzurevi.Infrastructure.Pdf;

public class GunlukIlacPdfUretici : IGunlukIlacPdfUretici
{
    public byte[] Uret(DateTime tarih, IReadOnlyList<IlacUygulamaDto> kayitlar)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var gun = tarih.ToString("dd.MM.yyyy");
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.Header().Text($"Günlük ilaç uygulamaları — {gun}").FontSize(16).SemiBold();
                page.Content().PaddingTop(16).Table(tablo =>
                {
                    tablo.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                        c.RelativeColumn(1);
                        c.RelativeColumn(1);
                        c.RelativeColumn(1);
                    });
                    tablo.Header(h =>
                    {
                        h.Cell().Text("Sakin").SemiBold();
                        h.Cell().Text("İlaç").SemiBold();
                        h.Cell().Text("Saat").SemiBold();
                        h.Cell().Text("Durum").SemiBold();
                        h.Cell().Text("Personel").SemiBold();
                    });
                    if (kayitlar.Count == 0)
                    {
                        tablo.Cell().ColumnSpan(5).PaddingTop(8).Text("Bu tarihte uygulama kaydı yok.");
                    }
                    foreach (var k in kayitlar)
                    {
                        tablo.Cell().Text(k.SakinAd);
                        tablo.Cell().Text(k.IlacAdi);
                        tablo.Cell().Text(k.Tarih.ToString("HH:mm"));
                        tablo.Cell().Text(k.Durum);
                        tablo.Cell().Text(k.Personel ?? "—");
                    }
                });
            });
        }).GeneratePdf();
    }
}
