using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Dal.EntityConfiguration;

public class WastewaterPermitEntityTypeConfiguration : IEntityTypeConfiguration<WastewaterPermit>
{
    public void Configure(EntityTypeBuilder<WastewaterPermit> builder)
    {
        builder.HasKey(p => p.Id); // 設定主鍵

        // 設定管制編號為唯一索引，因為它是業務上的唯一標識符
        builder.HasIndex(p => p.EmsNo).IsUnique();

        // 設定欄位屬性
        builder.Property(p => p.EmsNo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.FacilityName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Address)
            .HasMaxLength(500);
            
        builder.Property(p => p.UniformNo)
            .HasMaxLength(10);
            
        builder.Property(p => p.PermitNo)
            .HasMaxLength(100);

        // 為 decimal 型別設定精度，這對資料庫效能和儲存很重要
        builder.Property(p => p.PermittedWaterVolume)
            .HasColumnType("decimal(18, 4)");

        builder.Property(p => p.OutletTm2x)
            .HasColumnType("decimal(18, 6)");

        builder.Property(p => p.OutletTm2y)
            .HasColumnType("decimal(18, 6)");
            
        builder.Property(p => p.OutletLongitude)
            .HasColumnType("decimal(18, 10)");

        builder.Property(p => p.OutletLatitude)
            .HasColumnType("decimal(18, 10)");

        // 設定一對多關聯
        builder.HasMany(p => p.Emissions)
            .WithOne(e => e.Permit)
            .HasForeignKey(e => e.WastewaterPermitId)
            .OnDelete(DeleteBehavior.Cascade); // 當許可證被刪除時，其下的排放紀錄也一併刪除
    }
}