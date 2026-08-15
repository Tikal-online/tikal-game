using Games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Infrastructure.Configurations;

internal class EmptyTileConfiguration : IEntityTypeConfiguration<EmptyTile>
{
    public void Configure(EntityTypeBuilder<EmptyTile> builder)
    {
    }
}