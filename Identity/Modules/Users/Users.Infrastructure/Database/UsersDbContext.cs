using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Users.Infrastructure.Entities;

namespace Users.Infrastructure.Database;

public class UsersDbContext : IdentityDbContext<UserEntity, IdentityRole<int>, int>
{
    public const string Schema = "users";

    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(Schema);
    }
}