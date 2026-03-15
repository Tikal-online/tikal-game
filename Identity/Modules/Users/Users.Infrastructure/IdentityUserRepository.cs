using Microsoft.AspNetCore.Identity;
using OneOf;
using Users.Application;
using Users.Contracts.Errors;
using Users.Domain.Entities;

namespace Users.Infrastructure;

public class IdentityUserRepository : UserRepository
{
    private readonly UserManager<User> userManager;

    public IdentityUserRepository(UserManager<User> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await userManager.FindByNameAsync(username);
    }

    public async Task<OneOf<User, DuplicateUsernameError>> CreateUser(User user, string password)
    {
        var creationResult = await userManager.CreateAsync(user, password);

        if (!creationResult.Succeeded)
        {
            return new DuplicateUsernameError(user.UserName ?? "");
        }

        return user;
    }

    public async Task AssignRole(User user, string role)
    {
        await userManager.AddToRoleAsync(user, role);
    }
}