using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Dal.EntityConfiguration;

public class SiteAnnouncementEntityTypeConfiguration : IEntityTypeConfiguration<SiteAnnouncement>
{
    public void Configure(EntityTypeBuilder<SiteAnnouncement> builder)
    {
        builder.ToTable("SiteAnnouncements");
        
        builder.HasKey(a => a.Id);

        // 公告文號通常也是唯一的
        builder.HasIndex(a => a.AnnouncementNo).IsUnique();
        
        builder.Property(a => a.AnnouncementNo)
            .IsRequired()
            .HasMaxLength(100);

        // 對於長文本，建議不設定最大長度 (EF Core 會對應到 nvarchar(max) 或 text)
        builder.Property(a => a.Title).HasMaxLength(500);
    }
}