using Games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Infrastructure.Configurations;

internal class TempleTileConfiguration : IEntityTypeConfiguration<TempleTile>
{
    public void Configure(EntityTypeBuilder<TempleTile> builder)
    {
        builder.Property(x => x.TempleLevel)
            .IsRequired();
    }
}