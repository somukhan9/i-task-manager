using Application.Contracts;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Services;

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    TaskManagerDbContext context,
    IConfiguration config) : IAuthService
{
    public async Task<User> RegisterUserAsync(string name, string username, string email, string password,
        string role, CancellationToken cancellationToken)
    {
        var existingUser =
            await context.Users.FirstOrDefaultAsync(u => u.Email == email || u.UserName == username, cancellationToken);

        if (existingUser is not null)
        {
            throw new Exception($"User already exists with username \"{username}\" or email \"{email}\".");
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var user = new User()
        {
            Name = name,
            UserName = username,
            Email = email,
            CreatedDate = DateTime.UtcNow
        };
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception(string.Join(";<br/>", result.Errors.Select(e => e.Description)));
        }

        result = await userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception(string.Join(";<br/>", result.Errors.Select(e => e.Description)));
        }

        await transaction.CommitAsync(cancellationToken);

        return user;
    }

    public async Task<string> LoginAsync(string username, string password, bool rememberMe,
        CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == username || u.UserName == username,
            cancellationToken);

        if (user is null)
        {
            throw new Exception("Invalid credentials.");
        }

        await signInManager.SignInAsync(user, isPersistent: rememberMe);

        return await GenerateJwtToken(user, cancellationToken);
    }

    public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user;
    }

    private async Task<string> GenerateJwtToken(User user, CancellationToken cancellationToken)
    {
        var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
        var role = roles.FirstOrDefault();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
        };

        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
        );

        return cancellationToken.IsCancellationRequested
            ? string.Empty
            : new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task SignOutAsync(CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            await signInManager.SignOutAsync();
        }
    }
}