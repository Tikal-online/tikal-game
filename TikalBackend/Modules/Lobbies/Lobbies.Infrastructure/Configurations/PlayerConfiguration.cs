using Lobbies.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobbies.Infrastructure.Configurations;

internal sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SelectedColour)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.IsReady)
            .IsRequired();

        builder.Property(x => x.IsOwner)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
            .IsUnique();
    }
}