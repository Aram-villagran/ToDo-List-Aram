using System.Security.Claims;

namespace todoExample.api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue("sub");

        if (!Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("El token no contiene un usuario válido.");
        }

        return userId;
    }
}