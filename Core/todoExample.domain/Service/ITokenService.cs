using System.Security.Claims;
using todoExample.domain.Models.Identity;

namespace todoExample.domain.Service;

public interface ITokenService
{
    public (string Token, DateTime ExpiresAt) CreateAccessToken(ApplicationUser user, IEnumerable<string> roles);

    /// <summary>
    /// Valida firma, issuer y audience ignorando la expiración. Devuelve null si el token no es válido.
    /// </summary>
    public Task<ClaimsPrincipal?> GetPrincipalFromExpiredTokenAsync(string token);

    public (string Token, DateTime ExpiresAt) CreateRefreshToken();

    public string HashRefreshToken(string token);
}
