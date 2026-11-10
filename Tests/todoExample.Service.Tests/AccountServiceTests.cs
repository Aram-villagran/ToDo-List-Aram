using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using todoExample.domain.DTOs;
using todoExample.domain.Models.Identity;
using todoExample.Service.Security;
using todoExample.Service.Service;

namespace todoExample.Service.Tests;

public class AccountServiceTests
{
    private const string Password = "Secreta#123";

    private readonly Mock<UserManager<ApplicationUser>> _userManager = new(
        Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);

    private readonly TokenService _tokenService = new(Options.Create(TokenServiceTests.CreateSettings()));
    private readonly ApplicationUser _user = TokenServiceTests.CreateUser();
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _service = new AccountService(_userManager.Object, _tokenService);

        _userManager.Setup(m => m.FindByEmailAsync(_user.Email!)).ReturnsAsync(_user);
        _userManager.Setup(m => m.FindByIdAsync(_user.Id.ToString())).ReturnsAsync(_user);
        _userManager.Setup(m => m.CheckPasswordAsync(_user, Password)).ReturnsAsync(true);
        _userManager.Setup(m => m.GetRolesAsync(_user)).ReturnsAsync(["admin"]);
        _userManager.Setup(m => m.SetAuthenticationTokenAsync(_user, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(IdentityResult.Success);
    }

    private void SetStoredRefreshToken(string refreshToken, DateTime expiresAt) =>
        _userManager.Setup(m => m.GetAuthenticationTokenAsync(_user, AccountService.LoginProvider, AccountService.RefreshTokenName))
            .ReturnsAsync($"{_tokenService.HashRefreshToken(refreshToken)}|{expiresAt:O}");

    private RefreshTokenRequestDto CreateRefreshRequest(string refreshToken) => new()
    {
        AccessToken = _tokenService.CreateAccessToken(_user, ["admin"]).Token,
        RefreshToken = refreshToken
    };

    [Fact]
    public async Task Login_ReturnsTokens_WhenCredentialsValid()
    {
        var result = await _service.LoginAsync(new LoginRequestDto { Email = _user.Email!, Password = Password });

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.InRange(result.AccessTokenExpiresAt, DateTime.UtcNow.AddMinutes(179), DateTime.UtcNow.AddMinutes(181));
        var expectedPrefix = _tokenService.HashRefreshToken(result.RefreshToken) + "|";
        _userManager.Verify(m => m.SetAuthenticationTokenAsync(_user, AccountService.LoginProvider,
            AccountService.RefreshTokenName, It.Is<string>(v => v.StartsWith(expectedPrefix))), Times.Once);
    }

    [Fact]
    public async Task Login_ReturnsNull_WhenUserNotFound()
    {
        var result = await _service.LoginAsync(new LoginRequestDto { Email = "nadie@sacai.com.mx", Password = Password });

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_ReturnsNull_WhenUserDisabled()
    {
        _user.Enabled = false;

        var result = await _service.LoginAsync(new LoginRequestDto { Email = _user.Email!, Password = Password });

        Assert.Null(result);
        _userManager.Verify(m => m.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Login_ReturnsNull_AndCountsFailure_WhenPasswordWrong()
    {
        var result = await _service.LoginAsync(new LoginRequestDto { Email = _user.Email!, Password = "incorrecta" });

        Assert.Null(result);
        _userManager.Verify(m => m.AccessFailedAsync(_user), Times.Once);
    }

    [Fact]
    public async Task Refresh_ReturnsNewTokens_WhenValid()
    {
        var (oldRefresh, _) = _tokenService.CreateRefreshToken();
        SetStoredRefreshToken(oldRefresh, DateTime.UtcNow.AddDays(1));

        var result = await _service.RefreshTokenAsync(CreateRefreshRequest(oldRefresh));

        Assert.NotNull(result);
        Assert.NotEqual(oldRefresh, result.RefreshToken);
        var expectedPrefix = _tokenService.HashRefreshToken(result.RefreshToken) + "|";
        _userManager.Verify(m => m.SetAuthenticationTokenAsync(_user, AccountService.LoginProvider,
            AccountService.RefreshTokenName, It.Is<string>(v => v.StartsWith(expectedPrefix))), Times.Once);
    }

    [Fact]
    public async Task Refresh_ReturnsNull_WhenNewRefreshTokenCannotBeStored()
    {
        var (oldRefresh, _) = _tokenService.CreateRefreshToken();
        SetStoredRefreshToken(oldRefresh, DateTime.UtcNow.AddDays(1));
        // Otro refresh simultáneo ya cambió el ConcurrencyStamp del usuario
        _userManager.Setup(m => m.SetAuthenticationTokenAsync(_user, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityErrorDescriber().ConcurrencyFailure()));

        var result = await _service.RefreshTokenAsync(CreateRefreshRequest(oldRefresh));

        Assert.Null(result);
    }

    [Fact]
    public async Task Refresh_ReturnsNull_WhenRefreshTokenDoesNotMatch()
    {
        SetStoredRefreshToken(_tokenService.CreateRefreshToken().Token, DateTime.UtcNow.AddDays(1));

        var result = await _service.RefreshTokenAsync(CreateRefreshRequest(_tokenService.CreateRefreshToken().Token));

        Assert.Null(result);
    }

    [Fact]
    public async Task Refresh_ReturnsNull_WhenRefreshTokenExpired()
    {
        var (refresh, _) = _tokenService.CreateRefreshToken();
        SetStoredRefreshToken(refresh, DateTime.UtcNow.AddMinutes(-1));

        var result = await _service.RefreshTokenAsync(CreateRefreshRequest(refresh));

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUser_AssignsUserRole()
    {
        _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), Password)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "user")).ReturnsAsync(IdentityResult.Success);

        var (result, user) = await _service.CreateUserAsync(new UserCreateDto
        {
            FirstName = "Ana", LastName = "Pérez", Email = "ana@sacai.com.mx", Password = Password
        });

        Assert.True(result.Succeeded);
        Assert.NotNull(user);
        Assert.Equal("ana@sacai.com.mx", user.Email);
        Assert.Equal(["user"], user.Roles);
        _userManager.Verify(m => m.AddToRoleAsync(It.Is<ApplicationUser>(u => u.UserName == "ana@sacai.com.mx"), "user"), Times.Once);
    }
}
