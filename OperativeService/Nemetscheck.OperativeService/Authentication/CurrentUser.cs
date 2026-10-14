using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Nemetscheck.OperativeService.Domain;

namespace Nemetscheck.OperativeService.Authentication;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public long Id
    {
        get
        {
            var subject = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return long.TryParse(subject, NumberStyles.None, CultureInfo.InvariantCulture, out var id) && id > 0
                ? id
                : throw new UnauthorizedException("Token does not contain a valid user identifier");
        }
    }
}
