
using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.DTOs;

public class CreateUserRequest
{
    [Required]
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class ChangePasswordRequest
{
    public required string NewPassword { get; init; }
    public required string OldPassword { get; init; }
}

public class UpdateUserRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
}

public class UserResponse(User user)
{
    public string UserId { get; } = user.Id;
    public string FirstName { get; } = user.FirstName;
    public string LastName { get; } = user.LastName;
    public string Email { get; } = user.Email;
    public DateTime CreatedAt { get; } = user.CreatedAt;
}