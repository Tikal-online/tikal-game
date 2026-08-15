using Games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Infrastructure.Configurations;

internal class VolcanoTileConfiguration : IEntityTypeConfiguration<VolcanoTile>
{
    public void Configure(EntityTypeBuilder<VolcanoTile> builder)
    {
    }
}