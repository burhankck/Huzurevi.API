using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Huzurevi.Infrastructure.Persistence;

public sealed class UygulamaDbContextFabrika : IDesignTimeDbContextFactory<UygulamaDbContext>
{
    public UygulamaDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = TryReadDevelopmentConnectionString();
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=localhost;Port=5432;Database=HuzureviDb;Username=burhan;Password=";
        }

        var optionsBuilder = new DbContextOptionsBuilder<UygulamaDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            npgsql.CommandTimeout(30);
        });

        return new UygulamaDbContext(optionsBuilder.Options);
    }

    private static string? TryReadDevelopmentConnectionString()
    {
        var cwd = Directory.GetCurrentDirectory();
        var candidates = new[]
        {
            Path.Combine(cwd, "appsettings.Development.json"),
            Path.Combine(cwd, "appsettings.json"),
            Path.GetFullPath(Path.Combine(cwd, "..", "..", "appsettings.Development.json")),
            Path.GetFullPath(Path.Combine(cwd, "..", "..", "appsettings.json"))
        };

        foreach (var path in candidates)
        {
            if (!File.Exists(path))
            {
                continue;
            }

            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            if (!doc.RootElement.TryGetProperty("ConnectionStrings", out var connections))
            {
                continue;
            }

            foreach (var key in new[] { "DefaultConnection", "VarsayilanBaglanti" })
            {
                if (connections.TryGetProperty(key, out var value))
                {
                    var connectionString = value.GetString();
                    if (!string.IsNullOrWhiteSpace(connectionString))
                    {
                        return connectionString;
                    }
                }
            }
        }

        return null;
    }
}
