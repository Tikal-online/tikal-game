using Microsoft.AspNetCore.Identity;

namespace Users.Domain.Entities;

public sealed class User : IdentityUser<int>
{
}