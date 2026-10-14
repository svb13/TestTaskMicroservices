using Nemetscheck.UserDataAccessService.Domain.Users;

namespace Nemetscheck.UserDataAccessService.Domain.Abstractions;

public interface ITokenGenerator
{
    AccessToken Generate(User user);
}
