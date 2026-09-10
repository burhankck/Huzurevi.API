using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.KurumSurec;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Huzurevi.Infrastructure.Pdf;

public class KurumSurecPdfUretici : IKurumSurecPdfUretici
{
    public byte[] EsyaTespitUret(string sakinAd, DateTime tarih, IReadOnlyList<EsyaTespitDto> kayitlar)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.Header().Text($"Eşya tespit tutanağı — {sakinAd}").FontSize(16).SemiBold();
                page.Content().PaddingTop(12).Column(col =>
                {
                    col.Item().Text($"Tarih: {tarih:dd.MM.yyyy}");
                    col.Item().PaddingTop(10).Table(tablo =>
                    {
                        tablo.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(2);
                            c.ConstantColumn(50);
                            c.RelativeColumn(3);
                            c.RelativeColumn(2);
                        });
                        tablo.Header(h =>
                        {
                            h.Cell().Text("Kategori").SemiBold();
                            h.Cell().Text("Adet").SemiBold();
                            h.Cell().Text("Açıklama").SemiBold();
                            h.Cell().Text("Onay").SemiBold();
                        });
                        if (kayitlar.Count == 0)
                        {
                            tablo.Cell().ColumnSpan(4).PaddingTop(8).Text("Kayıt yok.");
                        }
                        foreach (var k in kayitlar)
                        {
                            tablo.Cell().Text(k.Kategori);
                            tablo.Cell().Text(k.Adet.ToString());
                            tablo.Cell().Text(k.Aciklama ?? "—");
                            tablo.Cell().Text(k.OnayDurumu);
                        }
                    });
                });
            });
        }).GeneratePdf();
    }

    public byte[] TeslimTutanagiUret(MirasciTeslimDto kayit)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.Header().Text("Mirasçı teslim tutanağı").FontSize(16).SemiBold();
                page.Content().PaddingTop(16).Column(col =>
                {
                    col.Item().Text($"Sakin: {kayit.SakinAd}");
                    col.Item().Text($"Mirasçı: {kayit.MirasciAd ?? "—"}");
                    col.Item().Text($"Teslim alan: {kayit.TeslimAlan}");
                    col.Item().Text($"Telefon: {kayit.TeslimAlanTelefon ?? "—"}");
                    col.Item().Text($"Tarih: {kayit.TeslimTarihi:dd.MM.yyyy}");
                    col.Item().PaddingTop(8).Text($"Eşya özeti: {kayit.EsyaOzeti ?? "—"}");
                    col.Item().Text($"Not: {kayit.Notlar ?? "—"}");
                    col.Item().PaddingTop(24).Text("Teslim alan imza: ________________");
                    col.Item().Text("Kurum görevlisi imza: ________________");
                });
            });
        }).GeneratePdf();
    }
}
