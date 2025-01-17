using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Configurations;
using WebApi.Middleware;

namespace WebApi.Controllers;

[Tags("Authentication")]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(ControllerParameters services) : PublicControllerBase(services)
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest dto, [FromServices] Settings settings)
    {
        if (!ModelState.IsValid)
            throw new ArgumentException("Please provide both username and password in the correct format.");

        var user = await Context.Users
            .SingleOrDefaultAsync(x => x.Email == dto.Username)
            ?? throw new NotFoundException("The username you entered does not exist.");

        if (!user.VerifyPassword(dto.Password))
            throw new UnauthorizedAccessException("The password you entered is incorrect.");

        // Generate JWT token
        var jwtSecret = settings.JWT.Secret;
        var jwtExpiry = settings.JWT.ExpiresIn;

        var token = CreateJwtToken(user, jwtSecret, jwtExpiry);

        return Ok(new LoginResponse(token, jwtExpiry));
    }
}
