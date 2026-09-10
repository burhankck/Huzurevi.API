using Huzurevi.Application.Features.Ayarlar;
using Huzurevi.Application.Features.Yedekleme;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Huzurevi.Infrastructure.Yedekleme;

public class YedeklemeZamanlayici : BackgroundService
{
    private readonly IServiceScopeFactory _kapsam;
    private readonly ILogger<YedeklemeZamanlayici> _gunluk;

    public YedeklemeZamanlayici(IServiceScopeFactory kapsam, ILogger<YedeklemeZamanlayici> gunluk)
    {
        _kapsam = kapsam;
        _gunluk = gunluk;
    }

    protected override async Task ExecuteAsync(CancellationToken durdur)
    {
        await Task.Delay(TimeSpan.FromMinutes(1), durdur);
        while (!durdur.IsCancellationRequested)
        {
            try
            {
                await Dene(durdur);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _gunluk.LogWarning(ex, "Zamanlanmış yedek denemesi başarısız.");
            }

            await Task.Delay(TimeSpan.FromMinutes(10), durdur);
        }
    }

    private async Task Dene(CancellationToken ct)
    {
        using var kapsam = _kapsam.CreateScope();
        var ayar = await kapsam.ServiceProvider.GetRequiredService<IAyarServisi>().GetirAsync(ct);
        if (!ayar.YedeklemeAktifMi) return;
        if (DateTime.Now.Hour != ayar.YedekSaat) return;

        var servis = kapsam.ServiceProvider.GetRequiredService<IYedeklemeServisi>();
        var liste = await servis.ListeleAsync(ct);
        var bugun = DateTime.Today;
        if (liste.Any(x => x.Tur == "Zamanlanmis" && x.Durum == "Tamam" && x.OlusturulmaTarihi.ToLocalTime().Date == bugun))
            return;

        await servis.OlusturAsync("Zamanlayici", "Zamanlanmis", ct);
        _gunluk.LogInformation("Zamanlanmış yedek alındı.");
    }
}
