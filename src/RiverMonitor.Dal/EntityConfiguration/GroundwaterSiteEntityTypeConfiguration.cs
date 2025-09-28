using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Dal.EntityConfiguration;

public class GroundwaterSiteEntityTypeConfiguration : IEntityTypeConfiguration<GroundwaterSite>
{
    public void Configure(EntityTypeBuilder<GroundwaterSite> builder)
    {
        builder.HasKey(s => s.Id);

        // 將業務編號 StationId (來自 Siteid) 設為唯一索引
        builder.HasIndex(s => s.SiteId).IsUnique();

        builder.Property(s => s.SiteId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.SiteName).HasMaxLength(100);
        builder.Property(s => s.SiteEngName).HasMaxLength(200);
        builder.Property(s => s.UgwDistrictName).HasMaxLength(100);
        builder.Property(s => s.County).HasMaxLength(50);
        builder.Property(s => s.Township).HasMaxLength(50);
        builder.Property(s => s.Statusofuse).HasMaxLength(10);

        // 為 decimal 型別的座標設定資料庫中的精度和範圍
        builder.Property(s => s.Twd97Lon).HasColumnType("decimal(12, 8)");
        builder.Property(s => s.Twd97Lat).HasColumnType("decimal(12, 8)");
        builder.Property(s => s.Twd97Tm2X).HasColumnType("decimal(12, 4)");
        builder.Property(s => s.Twd97Tm2Y).HasColumnType("decimal(12, 4)");
    }
}