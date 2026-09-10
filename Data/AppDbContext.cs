using Huzurevi.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Huzurevi.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Oda> Odalar => Set<Oda>();
    public DbSet<Yatak> Yataklar => Set<Yatak>();
    public DbSet<Sakin> Sakinler => Set<Sakin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Sakin - Yatak birebir ilişki açık tanımı (Dependent: Yatak)
        modelBuilder.Entity<Yatak>()
            .HasOne(y => y.Sakin)
            .WithOne()
            .HasForeignKey<Yatak>(y => y.SakinId)
            .OnDelete(DeleteBehavior.SetNull);

        // TC Kimlik No benzersizlik kuralı
        modelBuilder.Entity<Sakin>()
            .HasIndex(s => s.TcKimlikNo)
            .IsUnique();

        // Soft Delete (Silindi Mi filtresi)
        modelBuilder.Entity<Oda>().HasQueryFilter(o => !o.SilindiMi);
        modelBuilder.Entity<Yatak>().HasQueryFilter(y => !y.SilindiMi);
        modelBuilder.Entity<Sakin>().HasQueryFilter(s => !s.SilindiMi);
    }
}