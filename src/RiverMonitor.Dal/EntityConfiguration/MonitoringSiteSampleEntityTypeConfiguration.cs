using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Dal.EntityConfiguration;

public class MonitoringSiteSampleEntityTypeConfiguration : IEntityTypeConfiguration<MonitoringSiteSample>
{
    public void Configure(EntityTypeBuilder<MonitoringSiteSample> builder)
    {
        builder.HasKey(s => s.Id);

        // 為經常查詢的欄位建立索引，可以提升效能
        builder.HasIndex(s => s.SampleDate);
        builder.HasIndex(s => s.ItemName);

        builder.Property(s => s.ItemName).HasMaxLength(100);
        builder.Property(s => s.ItemEngName).HasMaxLength(200);
        builder.Property(s => s.ItemEngAbbreviation).HasMaxLength(50);
        builder.Property(s => s.ItemUnit).HasMaxLength(50);
        
        builder.Property(s => s.ItemValue).HasColumnType("decimal(18, 5)");

        // 設定一對多關聯
        builder.HasOne(sample => sample.Site)
            .WithMany(site => site.Samples)
            .HasForeignKey(sample => sample.MonitoringSiteId)
            .OnDelete(DeleteBehavior.Cascade); // 當測站被刪除時，其下的樣本數據也一併刪除
    }
}