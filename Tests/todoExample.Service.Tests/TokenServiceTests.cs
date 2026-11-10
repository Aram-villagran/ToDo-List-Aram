using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using todoExample.domain.Models.Identity;
using todoExample.Service.Security;

namespace todoExample.Service.Tests;

public class TokenServiceTests
{
    internal static JwtSettings CreateSettings(int accessTokenMinutes = 180, string? secretKey = null) => new()
    {
        SecretKey = secretKey ?? Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        Issuer = "todoExample.api",
        Audience = "todoExample.clients",
        AccessTokenMinutes = accessTokenMinutes,
        RefreshTokenDays = 7
    };

    internal static ApplicationUser CreateUser() => new()
    {
        Id = Guid.NewGuid(),
        UserName = "doe@sacai.com.mx",
        Email = "doe@sacai.com.mx",
        FirstName = "John",
        LastName = "Doe",
        Enabled = true
    };

    private static TokenService CreateService(JwtSettings settings) => new(Options.Create(settings));

    [Fact]
    public void CreateAccessToken_ExpiresIn180Minutes()
    {
        var service = CreateService(CreateSettings());

        var (token, expiresAt) = service.CreateAccessToken(CreateUser(), []);

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(DateTimeKind.Utc, expiresAt.Kind);
        Assert.InRange(expiresAt, DateTime.UtcNow.AddMinutes(180).AddSeconds(-5), DateTime.UtcNow.AddMinutes(180).AddSeconds(5));
    }

    [Fact]
    public async Task CreateAccessToken_IncludesUserClaimsAndRoles()
    {
        var service = CreateService(CreateSettings());
        var user = CreateUser();
        var (token, _) = service.CreateAccessToken(user, ["admin", "user"]);

        var principal = await service.GetPrincipalFromExpiredTokenAsync(token);

        Assert.NotNull(principal);
        Assert.Equal(user.Id.ToString(), principal.FindFirst("sub")?.Value);
        Assert.Equal(user.Email, principal.FindFirst("email")?.Value);
        Assert.True(principal.IsInRole("admin"));
        Assert.True(principal.IsInRole("user"));
    }

    [Fact]
    public async Task GetPrincipal_AcceptsExpiredToken()
    {
        var service = CreateService(CreateSettings(accessTokenMinutes: -1));
        var (token, _) = service.CreateAccessToken(CreateUser(), []);

        var principal = await service.GetPrincipalFromExpiredTokenAsync(token);

        Assert.NotNull(principal);
    }

    [Fact]
    public async Task GetPrincipal_RejectsTokenSignedWithOtherKey()
    {
        var (foreignToken, _) = CreateService(CreateSettings()).CreateAccessToken(CreateUser(), []);
        var service = CreateService(CreateSettings());

        var principal = await service.GetPrincipalFromExpiredTokenAsync(foreignToken);

        Assert.Null(principal);
    }

    [Fact]
    public async Task GetPrincipal_RejectsGarbage()
    {
        var service = CreateService(CreateSettings());

        Assert.Null(await service.GetPrincipalFromExpiredTokenAsync("abc"));
    }

    [Fact]
    public void CreateRefreshToken_IsUniqueAndExpiresIn7Days()
    {
        var service = CreateService(CreateSettings());

        var first = service.CreateRefreshToken();
        var second = service.CreateRefreshToken();

        Assert.NotEqual(first.Token, second.Token);
        Assert.True(first.Token.Length >= 80);
        Assert.InRange(first.ExpiresAt, DateTime.UtcNow.AddDays(7).AddSeconds(-5), DateTime.UtcNow.AddDays(7).AddSeconds(5));
    }

    [Fact]
    public void HashRefreshToken_IsDeterministicAndNotPlainText()
    {
        var service = CreateService(CreateSettings());
        var (token, _) = service.CreateRefreshToken();

        var hash = service.HashRefreshToken(token);

        Assert.Equal(hash, service.HashRefreshToken(token));
        Assert.NotEqual(token, hash);
        Assert.Equal(32, Convert.FromBase64String(hash).Length);
    }

    [Fact]
    public void GetSigningKey_ThrowsWhenKeyShorterThan32Bytes()
    {
        var settings = CreateSettings(secretKey: Convert.ToBase64String(RandomNumberGenerator.GetBytes(16)));

        Assert.Throws<InvalidOperationException>(() => settings.GetSigningKey());
    }
}
