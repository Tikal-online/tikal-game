using Games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Infrastructure.Configurations;

internal class TreasureTileConfiguration : IEntityTypeConfiguration<TreasureTile>
{
    public void Configure(EntityTypeBuilder<TreasureTile> builder)
    {
    }
}