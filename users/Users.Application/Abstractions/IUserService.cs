namespace Users.Application.Abstractions;

// public record URepresentation(string Email, string FirstName, string LastName);
public record URepresentation
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public interface IUserService
{
    Task<string> CreateUserAsync(
        URepresentation representation,
        CancellationToken cancellationToken);

    Task ResetPasswordAsync(
        string userId,
        string password,
        CancellationToken cancellationToken);
        
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken);
}