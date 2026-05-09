using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Gateway.Models;
using NorthwindTraders.Gateway.Services;

namespace NorthwindTraders.Gateway.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService) => _tokenService = tokenService;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (valid, role) = UserStore.Validate(request.Username, request.Password);
        if (!valid)
            return Unauthorized(new { error = "Invalid username or password." });

        var (token, expiresAt) = _tokenService.CreateToken(request.Username, role);
        return Ok(new LoginResponse(token, expiresAt));
    }
}
