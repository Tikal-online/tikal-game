using Microsoft.AspNetCore.Identity;
using OneOf;
using Users.Application;
using Users.Contracts.Errors;
using Users.Domain.Entities;

namespace Users.Infrastructure;

internal sealed class IdentityUserRepository : UserRepository
{
    private readonly UserManager<User> userManager;

    public IdentityUserRepository(UserManager<User> userManager)
    {
        this.userManager = userManager;
    }

    public Task<User?> GetByUsername(string username)
    {
        return userManager.FindByNameAsync(username);
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

    public Task<IList<string>> GetRoles(User user)
    {
        return userManager.GetRolesAsync(user);
    }

    public Task<bool> ValidatePassword(User user, string password)
    {
        return userManager.CheckPasswordAsync(user, password);
    }
}