using Microsoft.EntityFrameworkCore;
using Nemetscheck.UserDataAccessService.Domain.Users;

namespace Nemetscheck.UserDataAccessService.Infrastructure.Persistence;

internal sealed class UserRepository(UsersDbContext dbContext) : IUserRepository
{
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        dbContext.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken) =>
        await dbContext.Users.AddAsync(user, cancellationToken).ConfigureAwait(false);
}
