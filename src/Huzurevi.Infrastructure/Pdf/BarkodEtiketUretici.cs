using Huzurevi.Application.Common.Interfaces;
using QRCoder;

namespace Huzurevi.Infrastructure.Pdf;

public class BarkodEtiketUretici : IBarkodEtiketUretici
{
    public byte[] PngUret(string barkod, string kitapAd)
    {
        using var uretici = new QRCodeGenerator();
        using var veri = uretici.CreateQrCode($"{kitapAd} | {barkod}", QRCodeGenerator.ECCLevel.Q);
        using var png = new PngByteQRCode(veri);
        return png.GetGraphic(8);
    }
}
