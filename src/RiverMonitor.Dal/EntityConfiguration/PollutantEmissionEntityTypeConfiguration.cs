using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Dal.EntityConfiguration;

public class PollutantEmissionEntityTypeConfiguration : IEntityTypeConfiguration<PollutantEmission>
{
    public void Configure(EntityTypeBuilder<PollutantEmission> builder)
    {
        builder.HasKey(e => e.Id);
        
        // 設定索引可以加速查詢
        builder.HasIndex(e => e.EmissionItemName);
        builder.HasIndex(e => e.EmissionStartDate);

        builder.Property(e => e.EmissionItemName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.EmissionWaterVolume)
            .HasColumnType("decimal(18, 4)");

        builder.Property(e => e.EmissionValue)
            .HasColumnType("decimal(18, 10)");
            
        builder.Property(e => e.TotalItemValue)
            .HasColumnType("decimal(26, 10)");
    }
}