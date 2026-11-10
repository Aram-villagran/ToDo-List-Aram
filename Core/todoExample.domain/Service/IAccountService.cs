using Microsoft.AspNetCore.Identity;
using todoExample.domain.DTOs;

namespace todoExample.domain.Service;

public interface IAccountService
{
    public Task<List<UserDto>> GetUsersAsync(string? email);

    public Task<(IdentityResult Result, UserDto? User)> CreateUserAsync(UserCreateDto dto);

    /// <returns>null cuando las credenciales no son válidas.</returns>
    public Task<TokenResponseDto?> LoginAsync(LoginRequestDto dto);

    /// <returns>null cuando el access token, el usuario o el refresh token no son válidos.</returns>
    public Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto dto);
}
