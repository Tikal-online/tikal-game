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
            .HasValue<TreasureTile>((int)TileType.Treasure)
            .HasValue<VolcanoTile>((int)TileType.Volcano)
            .HasValue<TempleTile>((int)TileType.Temple);

        builder.Ignore(x => x.Type);

        builder.ComplexProperty(x => x.Costs,
            costs =>
            {
                costs.Property(c => c.North)
                    .HasColumnName("NorthCost")
                    .IsRequired();

                costs.Property(c => c.NorthEast)
                    .HasColumnName("NorthEastCost")
                    .IsRequired();

                costs.Property(c => c.SouthEast)
                    .HasColumnName("SouthEastCost")
                    .IsRequired();

                costs.Property(c => c.South)
                    .HasColumnName("SouthCost")
                    .IsRequired();

                costs.Property(c => c.SouthWest)
                    .HasColumnName("SouthWestCost")
                    .IsRequired();

                costs.Property(c => c.NorthWest)
                    .HasColumnName("NorthWestCost")
                    .IsRequired();
            });

        builder.ComplexProperty(x => x.Coordinate,
            coordinate =>
            {
                coordinate.Property(c => c.Q)
                    .HasColumnName("QCoordinate")
                    .IsRequired();

                coordinate.Property(c => c.R)
                    .HasColumnName("RCoordinate")
                    .IsRequired();
            });

        builder.HasMany(x => x.TroopAssignments)
            .WithOne(x => x.Tile)
            .HasForeignKey(x => x.TileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}