using Microsoft.EntityFrameworkCore;
using Nemetscheck.OperativeService.Domain;
using Nemetscheck.OperativeService.Domain.DiceRolls;

namespace Nemetscheck.OperativeService.Infrastructure.Persistence;

internal sealed class OperativeDbContext(DbContextOptions<OperativeDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<DiceRoll> DiceRolls => Set<DiceRoll>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OperativeDbContext).Assembly);
    }

    async Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
}
