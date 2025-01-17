
using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs;

public class LoginRequest
{
    [EmailAddress]
    public required string Username { get; init; }
    public required string Password { get; init; }
}

public class LoginResponse(string accessToken, int expiresIn)
{
    public string AccessToken { get; } = accessToken;
    public string TokenType { get; } = "Bearer";
    public int ExpiresIn { get; } = expiresIn;
}