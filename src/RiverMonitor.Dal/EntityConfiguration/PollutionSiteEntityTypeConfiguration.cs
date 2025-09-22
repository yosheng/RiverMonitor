using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Dal.EntityConfiguration;

public class PollutionSiteEntityTypeConfiguration : IEntityTypeConfiguration<PollutionSite>
{
    public void Configure(EntityTypeBuilder<PollutionSite> builder)
    {
        builder.ToTable("PollutionSites");

        builder.HasKey(s => s.Id);

        // 將業務邏輯上的 SiteId 設為唯一索引，確保資料不重複
        builder.HasIndex(s => s.SiteId).IsUnique();
        
        builder.Property(s => s.SiteId)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(s => s.SiteName).HasMaxLength(255);
        builder.Property(s => s.County).HasMaxLength(50);
        builder.Property(s => s.Township).HasMaxLength(50);
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.LandLots).HasMaxLength(1000);
        
        // 為了儲存較長的污染物描述，使用 HasMaxLength(4000) 或依需求調整
        builder.Property(s => s.Pollutants).HasMaxLength(4000); 

        builder.Property(s => s.SiteArea).HasColumnType("decimal(18, 2)");
        builder.Property(s => s.Dtmx).HasColumnType("decimal(18, 6)");
        builder.Property(s => s.Dtmy).HasColumnType("decimal(18, 6)");
        builder.Property(s => s.Longitude).HasColumnType("decimal(18, 15)");
        builder.Property(s => s.Latitude).HasColumnType("decimal(18, 15)");

        // 設定一對多關聯
        builder.HasMany(s => s.Announcements)
            .WithOne(a => a.Site)
            .HasForeignKey(a => a.PollutionSiteId)
            .OnDelete(DeleteBehavior.Cascade); // 當場址被刪除時，其下的公告也一併刪除
    }
}