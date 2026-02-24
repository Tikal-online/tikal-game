using Microsoft.AspNetCore.Identity;
using OneOf;
using Users.Application;
using Users.Contracts.Errors;
using Users.Domain.Entities;
using Users.Infrastructure.Entities;

namespace Users.Infrastructure;

public class IdentityUserRepository : UserRepository
{
    private readonly UserManager<UserEntity> userManager;

    public IdentityUserRepository(UserManager<UserEntity> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<User?> GetByUsername(string username, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(username);

        return user is null ? null : new User(user.Id, user.UserName ?? "");
    }

    public async Task<OneOf<User, DuplicateUsernameError>> CreateUser(
        User user,
        string password,
        CancellationToken cancellationToken
    )
    {
        var createdUser = new UserEntity
        {
            Id = user.Id, UserName = user.Username
        };

        var creationResult = await userManager.CreateAsync(createdUser, password);

        if (!creationResult.Succeeded)
        {
            return new DuplicateUsernameError(user.Username);
        }

        return new User(createdUser.Id, createdUser.UserName);
    }
}