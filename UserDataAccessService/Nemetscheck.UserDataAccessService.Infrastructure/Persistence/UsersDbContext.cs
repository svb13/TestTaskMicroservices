using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Nemetscheck.UserDataAccessService.Domain.Abstractions;
using Nemetscheck.UserDataAccessService.Domain.Exceptions;
using Nemetscheck.UserDataAccessService.Domain.Users;

namespace Nemetscheck.UserDataAccessService.Infrastructure.Persistence;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) 
    : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);

    async Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex) 
        when (ex.InnerException is SqliteException 
        { 
            SqliteErrorCode: 19, 
            SqliteExtendedErrorCode: 2067 or 1555 
        })
        {
            throw new ConflictException("A user with this email already exists", ex);
        }
    }
}
