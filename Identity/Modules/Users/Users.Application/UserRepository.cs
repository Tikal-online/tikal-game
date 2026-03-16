using OneOf;
using Users.Contracts.Errors;
using Users.Domain.Entities;

namespace Users.Application;

public interface UserRepository
{
    Task<User?> GetByUsername(string username);

    Task<OneOf<User, DuplicateUsernameError>> CreateUser(User user, string password);

    Task AssignRole(User user, string role);

    Task<IList<string>> GetRoles(User user);

    Task<bool> ValidatePassword(User user, string password);
}