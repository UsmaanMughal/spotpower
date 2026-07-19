using Microsoft.EntityFrameworkCore;
using SpotPower.Core;

namespace SpotPower.Infrastructure.Persistence;

public class SpotPowerDbContext(DbContextOptions<SpotPowerDbContext> options) : DbContext(options)
{
    public DbSet<AuctionPrice> AuctionPrices => Set<AuctionPrice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuctionPrice>(entity =>
        {
            entity.HasIndex(p => new { p.DeliveryDate, p.Period }).IsUnique();
            entity.Property(p => p.PriceEurPerMwhSpain).HasPrecision(10, 2);
            entity.Property(p => p.PriceEurPerMwhPortugal).HasPrecision(10, 2);
        });
    }
}
