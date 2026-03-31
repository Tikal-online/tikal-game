using Accounts.Application.DataAccess;
using Accounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure.Database;

public sealed class AccountsDbContext : DbContext, UnitOfWork
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