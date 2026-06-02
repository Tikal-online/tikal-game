using Lobbies.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobbies.Infrastructure.Configurations;

internal sealed class LobbyConfiguration : IEntityTypeConfiguration<Lobby>
{
    public void Configure(EntityTypeBuilder<Lobby> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.MaxPlayers)
            .IsRequired();

        builder.HasMany(x => x.Players)
            .WithOne(x => x.Lobby)
            .HasForeignKey(x => x.LobbyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}