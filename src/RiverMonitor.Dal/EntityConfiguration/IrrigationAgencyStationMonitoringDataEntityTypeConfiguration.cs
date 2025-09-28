using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Dal.EntityConfiguration;

public class IrrigationAgencyStationMonitoringDataEntityTypeConfiguration : 
    IEntityTypeConfiguration<IrrigationAgencyStationMonitoringData>
{
    public void Configure(EntityTypeBuilder<IrrigationAgencyStationMonitoringData> builder)
    {
        builder.HasKey(d => d.Id);

        // 經常會根據工作站和日期來查詢數據，建立複合索引
        builder.HasIndex(d => new { d.IrrigationAgencyStationId, d.SampleDate });

        builder.Property(d => d.SiteName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Version)
            .HasMaxLength(20);

        builder.Property(d => d.Note)
            .HasMaxLength(1000);

        builder.Property(d => d.WaterTemperatureC).HasColumnType("decimal(10, 2)");
        builder.Property(d => d.PhValue).HasColumnType("decimal(10, 2)");
        builder.Property(d => d.ElectricalConductivity).HasColumnType("decimal(10, 2)");

        builder.HasOne(data => data.Station)
            // 假設 IrrigationAgencyStation 中有一個名為 MonitoringData 的集合屬性
            // 如果沒有，也可以直接使用 .WithMany()
            .WithMany(station => station.MonitoringData)
            .HasForeignKey(data => data.IrrigationAgencyStationId)
            // 當工作站被刪除時，其下的監測數據也一併級聯刪除
            .OnDelete(DeleteBehavior.Cascade);
    }
}