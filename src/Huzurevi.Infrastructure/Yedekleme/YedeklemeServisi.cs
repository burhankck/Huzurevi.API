using System.Diagnostics;
using System.IO.Compression;
using Huzurevi.Application.Common.Exceptions;
using Huzurevi.Application.Common.Interfaces;
using Huzurevi.Application.Features.Ayarlar;
using Huzurevi.Application.Features.Denetim;
using Huzurevi.Application.Features.Yedekleme;
using Huzurevi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Huzurevi.Infrastructure.Yedekleme;

public class YedeklemeServisi : IYedeklemeServisi
{
    private readonly IUygulamaDbContext _db;
    private readonly IAyarServisi _ayar;
    private readonly IDenetimServisi _denetim;
    private readonly ILogger<YedeklemeServisi> _gunluk;
    private readonly string _yedekKok;
    private readonly string _yuklemeKok;
    private readonly string _baglanti;

    public YedeklemeServisi(
        IUygulamaDbContext db,
        IAyarServisi ayar,
        IDenetimServisi denetim,
        IHostEnvironment ortam,
        IConfiguration yapilandirma,
        ILogger<YedeklemeServisi> gunluk)
    {
        _db = db;
        _ayar = ayar;
        _denetim = denetim;
        _gunluk = gunluk;
        _yedekKok = Path.Combine(ortam.ContentRootPath, "Yedekler");
        _yuklemeKok = Path.Combine(ortam.ContentRootPath, "Uploads");
        _baglanti = yapilandirma.GetConnectionString("VarsayilanBaglanti")
            ?? yapilandirma.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Veritabanı bağlantısı yok.");
        Directory.CreateDirectory(_yedekKok);
    }

    public async Task<List<YedekDto>> ListeleAsync(CancellationToken ct = default)
    {
        var kayitlar = await _db.YedekKayitlari.AsNoTracking().OrderByDescending(x => x.Id).Take(100).ToListAsync(ct);
        return kayitlar.Select(Map).ToList();
    }

    public async Task<YedekDto> OlusturAsync(string olusturan, string tur, CancellationToken ct = default)
    {
        var damga = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var ad = $"huzurevi_{damga}";
        var dumpYol = Path.Combine(_yedekKok, $"{ad}.dump");
        string? zipYol = null;
        var kayit = new YedekKaydi
        {
            Ad = ad,
            Tur = tur,
            Durum = "Tamam",
            Olusturan = olusturan,
            OlusturulmaTarihi = DateTime.UtcNow,
            VeritabaniYolu = Path.GetFileName(dumpYol)
        };

        try
        {
            await PgArac("pg_dump", ["-d", Cs().Database ?? "HuzureviDb", "-Fc", "-f", dumpYol], ct);
            if (Directory.Exists(_yuklemeKok) && Directory.EnumerateFileSystemEntries(_yuklemeKok).Any())
            {
                zipYol = Path.Combine(_yedekKok, $"{ad}.dosyalar.zip");
                if (File.Exists(zipYol)) File.Delete(zipYol);
                ZipFile.CreateFromDirectory(_yuklemeKok, zipYol, CompressionLevel.SmallestSize, false);
                kayit.DosyaArsivYolu = Path.GetFileName(zipYol);
            }

            kayit.Boyut = new FileInfo(dumpYol).Length + (zipYol is null ? 0 : new FileInfo(zipYol).Length);
            _db.YedekKayitlari.Add(kayit);
            await _db.SaveChangesAsync(ct);
            await _denetim.YazAsync(new DenetimYazIstek("Islem", "Yedek alındı", $"{ad} ({tur})", olusturan, null, null, 200, "/api/Yedek", null, null), ct);
            await EskileriSil(ct);
            return Map(kayit);
        }
        catch (Exception ex)
        {
            kayit.Durum = "Hata";
            kayit.HataMesaji = Kisalt(ex.Message, 400);
            _db.YedekKayitlari.Add(kayit);
            await _db.SaveChangesAsync(ct);
            _gunluk.LogError(ex, "Yedek alınamadı.");
            throw new GecersizIstekHatasi($"Yedek alınamadı: {ex.Message}");
        }
    }

    public async Task GeriYukleAsync(int id, string olusturan, CancellationToken ct = default)
    {
        var kayit = await Kayit(id, ct);
        if (kayit.Durum != "Tamam")
            throw new GecersizIstekHatasi("Hatalı yedekten geri yükleme yapılamaz.");
        var dump = TamYol(kayit.VeritabaniYolu);
        if (!File.Exists(dump))
            throw new KayitBulunamadiHatasi("Yedek dosyası diskte yok.");

        await BaglantilariKes(ct);
        await PgArac("pg_restore", ["--clean", "--if-exists", "--no-owner", "--no-acl", "-d", Cs().Database!, dump], ct);

        if (!string.IsNullOrWhiteSpace(kayit.DosyaArsivYolu))
        {
            var zip = TamYol(kayit.DosyaArsivYolu);
            if (File.Exists(zip))
            {
                var gecici = Path.Combine(_yedekKok, $"geri_{Guid.NewGuid():N}");
                ZipFile.ExtractToDirectory(zip, gecici, true);
                if (Directory.Exists(_yuklemeKok))
                    Directory.Delete(_yuklemeKok, true);
                Directory.Move(gecici, _yuklemeKok);
            }
        }

        await _denetim.YazAsync(new DenetimYazIstek("Islem", "Yedek geri yüklendi", kayit.Ad, olusturan, null, null, 200, "/api/Yedek", null, null), ct);
    }

    public async Task SilAsync(int id, CancellationToken ct = default)
    {
        var kayit = await Kayit(id, ct);
        SilDosya(kayit.VeritabaniYolu);
        SilDosya(kayit.DosyaArsivYolu);
        _db.YedekKayitlari.Remove(kayit);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<KayitliDosya> IndirAsync(int id, string parca, CancellationToken ct = default)
    {
        var kayit = await Kayit(id, ct);
        var goreceli = parca == "dosyalar" ? kayit.DosyaArsivYolu : kayit.VeritabaniYolu;
        if (string.IsNullOrWhiteSpace(goreceli))
            throw new KayitBulunamadiHatasi("Bu yedekte istenen parça yok.");
        var tam = TamYol(goreceli);
        if (!File.Exists(tam))
            throw new KayitBulunamadiHatasi("Dosya bulunamadı.");
        var tip = parca == "dosyalar" ? "application/zip" : "application/octet-stream";
        return new KayitliDosya(File.OpenRead(tam), tip, goreceli);
    }

    private async Task EskileriSil(CancellationToken ct)
    {
        var ayar = await _ayar.GetirAsync(ct);
        var adet = Math.Max(ayar.YedekSaklamaAdet, 3);
        var fazla = await _db.YedekKayitlari.Where(x => x.Durum == "Tamam").OrderByDescending(x => x.Id).Skip(adet).ToListAsync(ct);
        foreach (var eski in fazla)
        {
            SilDosya(eski.VeritabaniYolu);
            SilDosya(eski.DosyaArsivYolu);
            _db.YedekKayitlari.Remove(eski);
        }
        if (fazla.Count > 0)
            await _db.SaveChangesAsync(ct);
    }

    private async Task PgArac(string arac, IReadOnlyList<string> argumanlar, CancellationToken ct)
    {
        var cs = Cs();
        var exe = AracBul(arac);
        var psi = new ProcessStartInfo
        {
            FileName = exe,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        psi.ArgumentList.Add("-h");
        psi.ArgumentList.Add(cs.Host ?? "localhost");
        psi.ArgumentList.Add("-p");
        psi.ArgumentList.Add((cs.Port == 0 ? 5432 : cs.Port).ToString());
        psi.ArgumentList.Add("-U");
        psi.ArgumentList.Add(cs.Username ?? "postgres");
        foreach (var a in argumanlar)
            psi.ArgumentList.Add(a);
        psi.Environment["PGPASSWORD"] = cs.Password ?? "";

        using var surec = new Process { StartInfo = psi };
        surec.Start();
        var hata = await surec.StandardError.ReadToEndAsync(ct);
        await surec.WaitForExitAsync(ct);
        if (surec.ExitCode != 0)
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(hata) ? $"{arac} kod {surec.ExitCode}" : hata.Trim());
    }

    private async Task BaglantilariKes(CancellationToken ct)
    {
        var cs = Cs();
        var builder = new NpgsqlConnectionStringBuilder(_baglanti) { Timeout = 15 };
        await using var conn = new NpgsqlConnection(builder.ConnectionString);
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT pg_terminate_backend(pid)
            FROM pg_stat_activity
            WHERE datname = @db AND pid <> pg_backend_pid();
            """;
        cmd.Parameters.AddWithValue("db", cs.Database ?? "");
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private NpgsqlConnectionStringBuilder Cs() => new(_baglanti);

    private async Task<YedekKaydi> Kayit(int id, CancellationToken ct) =>
        await _db.YedekKayitlari.FirstOrDefaultAsync(x => x.Id == id, ct)
        ?? throw new KayitBulunamadiHatasi("Yedek kaydı bulunamadı.");

    private string TamYol(string goreceli)
    {
        var tam = Path.GetFullPath(Path.Combine(_yedekKok, goreceli));
        if (!tam.StartsWith(Path.GetFullPath(_yedekKok), StringComparison.OrdinalIgnoreCase))
            throw new GecersizIstekHatasi("Geçersiz yedek yolu.");
        return tam;
    }

    private void SilDosya(string? goreceli)
    {
        if (string.IsNullOrWhiteSpace(goreceli)) return;
        var tam = Path.Combine(_yedekKok, goreceli);
        if (File.Exists(tam)) File.Delete(tam);
    }

    private static string AracBul(string ad)
    {
        var path = Environment.GetEnvironmentVariable("PATH") ?? "";
        foreach (var dizin in path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            var aday = Path.Combine(dizin, ad);
            if (File.Exists(aday)) return aday;
        }
        foreach (var dizin in new[]
                 {
                     "/Applications/Postgres.app/Contents/Versions/18/bin",
                     "/Applications/Postgres.app/Contents/Versions/17/bin",
                     "/Applications/Postgres.app/Contents/Versions/16/bin",
                     "/opt/homebrew/bin",
                     "/opt/homebrew/opt/postgresql@16/bin",
                     "/opt/homebrew/opt/postgresql@17/bin",
                     "/usr/local/bin",
                     "/usr/bin"
                 })
        {
            var aday = Path.Combine(dizin, ad);
            if (File.Exists(aday)) return aday;
        }
        throw new InvalidOperationException($"{ad} bulunamadı. PostgreSQL istemci araçları kurulu olmalıdır.");
    }

    private static YedekDto Map(YedekKaydi x) =>
        new(x.Id, x.OlusturulmaTarihi, x.Ad, x.Tur, x.Durum, x.Boyut, x.Olusturan, x.HataMesaji, !string.IsNullOrWhiteSpace(x.DosyaArsivYolu));

    private static string Kisalt(string metin, int max) => metin.Length <= max ? metin : metin[..max];
}
