using Games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Infrastructure.Configurations;

internal sealed class TileMapConfiguration : IEntityTypeConfiguration<TileMap>
{
    public void Configure(EntityTypeBuilder<TileMap> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.Tiles)
            .WithOne(x => x.TileMap)
            .HasForeignKey(x => x.TileMapId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}