using Users.Application.Models;

namespace Users.Application.Abstractions;

public interface IUserService
{
    Task<string> CreateUserAsync(
        UserRequest request,
        CancellationToken cancellationToken);

    Task ResetPasswordAsync(
        string userId,
        string password,
        CancellationToken cancellationToken);
        
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken);
}