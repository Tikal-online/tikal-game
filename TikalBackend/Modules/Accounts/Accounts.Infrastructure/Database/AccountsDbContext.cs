using Accounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure.Database;

public sealed class AccountsDbContext : DbContext
{
    public const string Schema = "Accounts";

    public DbSet<Account> Accounts { get; set; }

    public AccountsDbContext(DbContextOptions<AccountsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountsDbContext).Assembly);
    }
}