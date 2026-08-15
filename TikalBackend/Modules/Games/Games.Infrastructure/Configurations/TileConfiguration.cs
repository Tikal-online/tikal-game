using Games.Domain.Entities;
using Games.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Infrastructure.Configurations;

internal sealed class TileConfiguration : IEntityTypeConfiguration<Tile>
{
    public void Configure(EntityTypeBuilder<Tile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasDiscriminator<int>("TileType")
            .HasValue<EmptyTile>((int)TileType.Empty)
            .HasValue<TempleTile>((int)TileType.Temple);

        builder.Ignore(x => x.Type);

        builder.Ignore(x => x.Coordinate);

        builder.Ignore(x => x.Costs);

        builder.HasMany(x => x.TroopAssignments)
            .WithOne(x => x.Tile)
            .HasForeignKey(x => x.TileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}