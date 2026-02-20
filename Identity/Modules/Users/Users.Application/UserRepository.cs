using OneOf;
using Users.Contracts.Errors;
using Users.Domain.Entities;

namespace Users.Application;

public interface UserRepository
{
    Task<User?> GetByUsername(string username, CancellationToken cancellationToken);

    Task<OneOf<User, DuplicateUsernameError>> CreateUser(
        User user,
        string password,
        CancellationToken cancellationToken
    );
}