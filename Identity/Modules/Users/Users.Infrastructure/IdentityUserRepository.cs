using OneOf;
using Users.Application;
using Users.Contracts.Errors;
using Users.Domain.Entities;

namespace Users.Infrastructure;

public class IdentityUserRepository : UserRepository
{
    private readonly List<User> users = [];

    public Task<User?> GetByUsername(string username, CancellationToken cancellationToken)
    {
        return Task.FromResult(users.FirstOrDefault(u => u.Username == username));
    }

    public Task<OneOf<User, DuplicateUsernameError>> CreateUser(
        User user,
        string password,
        CancellationToken cancellationToken
    )
    {
        if (users.Any(u => u.Username == user.Username))
        {
            return Task.FromResult<OneOf<User, DuplicateUsernameError>>(new DuplicateUsernameError(user.Username));
        }

        users.Add(user);

        return Task.FromResult<OneOf<User, DuplicateUsernameError>>(user);
    }
}