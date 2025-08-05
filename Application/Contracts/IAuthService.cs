using Domain.Entities;

namespace Application.Contracts;

public interface IAuthService
{
    Task<User> RegisterUserAsync(string name, string username, string email, string password, string role,
        CancellationToken cancellationToken);

    Task<string> LoginAsync(string username, string password, bool rememberMe, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    System.Threading.Tasks.Task SignOutAsync(CancellationToken cancellationToken);
}