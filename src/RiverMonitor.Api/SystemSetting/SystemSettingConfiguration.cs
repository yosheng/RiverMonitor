using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RiverMonitor.Api.SystemSetting;

public class SystemSettingConfiguration : IEntityTypeConfiguration<Model.Entities.SystemSetting>
{
    public void Configure(EntityTypeBuilder<Model.Entities.SystemSetting> builder)
    {
        builder.HasComment("系统配置")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasComment("主键");
        
        builder.Property(x => x.Key)
            .HasComment("配置键")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasComment("配置值")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasComment("配置描述");
    }
}