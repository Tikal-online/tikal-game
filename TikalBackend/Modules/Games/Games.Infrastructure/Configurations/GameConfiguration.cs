using Games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Infrastructure.Configurations;

internal sealed class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LobbyId)
            .IsRequired();

        builder.HasOne(x => x.TileMap)
            .WithOne(x => x.Game)
            .HasForeignKey<TileMap>(x => x.GameId)
            .IsRequired();

        builder.HasMany(x => x.Players)
            .WithOne(x => x.Game)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.LobbyId)
            .IsUnique();
    }
}