using Games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Infrastructure.Configurations;

internal class TroopAssignmentConfiguration : IEntityTypeConfiguration<TroopAssignment>
{
    public void Configure(EntityTypeBuilder<TroopAssignment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TroopType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Count)
            .IsRequired();
    }
}