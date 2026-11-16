using Games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Infrastructure.Configurations;

internal sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Colour)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Points)
            .IsRequired();

        builder.Property(x => x.TurnPosition)
            .IsRequired();

        // 2 Players in the same game should never have the same turn position
        builder.HasIndex(x => new { x.GameId, x.TurnPosition })
            .IsUnique();

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.HasMany(x => x.TroopAssignments)
            .WithOne(x => x.Player)
            .HasForeignKey(x => x.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}