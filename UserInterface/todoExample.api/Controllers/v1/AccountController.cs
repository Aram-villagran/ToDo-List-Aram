using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using todoExample.domain.DTOs;
using todoExample.domain.Service;

namespace todoExample.api.Controllers.v1;

[ApiController]
[Route("/api/v1/[controller]")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers([FromQuery] string? email)
    {
        return Ok(await accountService.GetUsersAsync(email));
    }

    [HttpPost("register")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserDto>> Register(UserCreateDto dto)
    {
        var (result, user) = await accountService.CreateUserAsync(dto);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return CreatedAtAction(nameof(GetUsers), new { email = user!.Email }, user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginRequestDto dto)
    {
        var tokens = await accountService.LoginAsync(dto);

        return tokens is null ? Unauthorized(new { message = "Credenciales inválidas." }) : Ok(tokens);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto dto)
    {
        var tokens = await accountService.RefreshTokenAsync(dto);

        return tokens is null ? Unauthorized(new { message = "Token inválido." }) : Ok(tokens);
    }
}
