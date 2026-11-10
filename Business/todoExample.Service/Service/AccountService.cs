using System.Globalization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using todoExample.domain.DTOs;
using todoExample.domain.Models.Identity;
using todoExample.domain.Service;

namespace todoExample.Service.Service;

public class AccountService(UserManager<ApplicationUser> userManager, ITokenService tokenService) : IAccountService
{
    public const string LoginProvider = "todoExample";
    public const string RefreshTokenName = "RefreshToken";
    public const string DefaultRole = "user";

    public async Task<List<UserDto>> GetUsersAsync(string? email)
    {
        var query = userManager.Users;

        if (!string.IsNullOrWhiteSpace(email))
        {
            var normalizedEmail = userManager.NormalizeEmail(email);
            query = query.Where(u => u.NormalizedEmail != null && u.NormalizedEmail.Contains(normalizedEmail));
        }

        var users = await query.OrderBy(u => u.Email).ToListAsync();

        var result = new List<UserDto>(users.Count);
        foreach (var user in users)
        {
            result.Add(ToDto(user, await userManager.GetRolesAsync(user)));
        }

        return result;
    }

    public async Task<(IdentityResult Result, UserDto? User)> CreateUserAsync(UserCreateDto dto)
    {
        var now = DateTime.UtcNow;
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Enabled = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return (result, null);

        result = await userManager.AddToRoleAsync(user, DefaultRole);
        if (!result.Succeeded) return (result, null);

        return (result, ToDto(user, [DefaultRole]));
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null || !await CanSignInAsync(user)) return null;

        if (!await userManager.CheckPasswordAsync(user, dto.Password))
        {
            await userManager.AccessFailedAsync(user);
            return null;
        }

        await userManager.ResetAccessFailedCountAsync(user);

        return await IssueTokensAsync(user);
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        var principal = await tokenService.GetPrincipalFromExpiredTokenAsync(dto.AccessToken);
        var userId = principal?.FindFirst("sub")?.Value;
        if (userId is null) return null;

        var user = await userManager.FindByIdAsync(userId);
        if (user is null || !await CanSignInAsync(user)) return null;

        var stored = await userManager.GetAuthenticationTokenAsync(user, LoginProvider, RefreshTokenName);
        if (!IsStoredRefreshTokenValid(stored, dto.RefreshToken)) return null;

        return await IssueTokensAsync(user);
    }

    #region Helpers

    private async Task<bool> CanSignInAsync(ApplicationUser user) =>
        user.Enabled && !await userManager.IsLockedOutAsync(user);

    /// <returns>null si no se pudo guardar el refresh token (p. ej. otro refresh simultáneo ganó la rotación).</returns>
    private async Task<TokenResponseDto?> IssueTokensAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var (accessToken, accessExpiresAt) = tokenService.CreateAccessToken(user, roles);
        var (refreshToken, refreshExpiresAt) = tokenService.CreateRefreshToken();

        // Sólo se guarda el hash; el token en claro únicamente lo recibe el cliente
        var stored = $"{tokenService.HashRefreshToken(refreshToken)}|{refreshExpiresAt.ToString("O", CultureInfo.InvariantCulture)}";
        var storeResult = await userManager.SetAuthenticationTokenAsync(user, LoginProvider, RefreshTokenName, stored);
        if (!storeResult.Succeeded) return null;

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt
        };
    }

    private bool IsStoredRefreshTokenValid(string? stored, string refreshToken)
    {
        var parts = stored?.Split('|');
        if (parts is not { Length: 2 }) return false;

        if (!DateTime.TryParse(parts[1], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var expiresAt)
            || expiresAt <= DateTime.UtcNow)
        {
            return false;
        }

        try
        {
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(parts[0]),
                Convert.FromBase64String(tokenService.HashRefreshToken(refreshToken)));
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static UserDto ToDto(ApplicationUser user, IList<string> roles) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email ?? string.Empty,
        Roles = roles
    };

    #endregion
}
