using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Dal.EntityConfiguration;

public class IrrigationAgencyStationEntityTypeConfiguration : IEntityTypeConfiguration<IrrigationAgencyStation>
{
    public void Configure(EntityTypeBuilder<IrrigationAgencyStation> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.HasIndex(s => new { s.Name, s.IrrigationAgencyId })
            .IsUnique();
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(s => s.Phone)
            .HasMaxLength(20);
        
        builder.Property(s => s.Address)
            .HasMaxLength(500);
        
        builder.Property(s => s.Twd97Lon).HasColumnType("decimal(12, 8)");
        builder.Property(s => s.Twd97Lat).HasColumnType("decimal(12, 8)");
        
        // 設定與 IrrigationAgency 的一對多關聯
        builder.HasOne(station => station.Agency)
            .WithMany(agency => agency.Stations)
            .HasForeignKey(station => station.IrrigationAgencyId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}