using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.DTOs;
using WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Tags("Users")]
    [Route("api/users")]
    public class UserController(ControllerParameters services) : SecureControllerBase(services)
    {
        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUser([FromRoute] string id)
        {
            var user = await Context.Users.FindAsync(id)
                ?? throw new NotFoundException($"User with ID '{id}' not found.");

            return Ok(new UserResponse(user));
        }


        // GET: api/users (with pagination using SecureControllerBase's Paginate method)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers([FromQuery] PaginationQuery? pagination)
        {
            // Use the Paginate method from SecureControllerBase
            var query = Context.Users.AsQueryable();
            var paginatedResult = await ToPagedListAsync(query, pagination, user => new UserResponse(user));

            return Ok(paginatedResult);
        }


        // GET: api/users/{userId}/debtors
        [HttpGet("{userId}/debtors")]
        public async Task<ActionResult<IEnumerable<DebtorResponse>>> GetDebtorsForUser(
            [FromRoute] string userId,
            [FromQuery] PaginationQuery? pagination)
        {
            // Query debtors for the specified user
            var query = Context.Users
                .Include(x => x.Debtors)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Debtors);

            if (!await query.AnyAsync())
                throw new NotFoundException("User not found or no debtors associated with this user.");

            // Paginate with projection
            return Ok(await ToPagedListAsync(query, pagination, debtor => new DebtorResponse(debtor)));
        }

        // POST: api/users
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest dto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException("Invalid user data.");
            }

            // Check for duplicate email
            if (await Context.Users.AnyAsync(x => x.Email == dto.Email))
            {
                throw new ConflictException("A user with the same email already exists.");
            }

            var user = new User(dto.FirstName, dto.LastName, dto.Email, dto.Password);
            Context.Users.Add(user);

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserResponse(user));
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponse>> UpdateUser(
            [FromRoute] string id, [FromBody] UpdateUserRequest dto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException("Invalid user data.");
            }

            var user = await Context.Users.FindAsync(id)
                ?? throw new NotFoundException($"User with ID '{id}' not found.");

            // Check for duplicate email (if changing the email)
            if (user.Email != dto.Email && await Context.Users.AnyAsync(x => x.Email == dto.Email))
            {
                throw new ConflictException("A user with the same email already exists.");
            }

            user.UpdateProfile(dto.FirstName, dto.LastName, dto.Email);
            await Context.SaveChangesAsync();

            return Ok(new UserResponse(user));
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserResponse>> DeleteUser([FromRoute] string id)
        {
            var user = await Context.Users.FindAsync(id)
                ?? throw new NotFoundException($"User with ID '{id}' not found.");

            Context.Users.Remove(user);
            await Context.SaveChangesAsync();

            return Ok(new UserResponse(user));
        }

        // PUT: api/users/{id}/change-password
        [HttpPut("{id}/change-password")]
        public async Task<ActionResult> ChangePassword(
            [FromRoute] string id, [FromBody] ChangePasswordRequest dto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException("Invalid password data.");
            }

            var user = await Context.Users.FindAsync(id)
                ?? throw new NotFoundException($"User with ID '{id}' not found.");

            if (!user.VerifyPassword(dto.OldPassword))
            {
                throw new ValidationException("The old password is incorrect.");
            }

            user.UpdatePassword(dto.OldPassword, dto.NewPassword);
            await Context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully." });
        }
    }
}
