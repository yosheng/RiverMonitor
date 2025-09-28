using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Dal.EntityConfiguration;

public class IrrigationAgencyEntityTypeConfiguration : IEntityTypeConfiguration<IrrigationAgency>
{
    public void Configure(EntityTypeBuilder<IrrigationAgency> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.HasIndex(s => s.OpenUnitId)
            .IsUnique();

        builder.Property(s => s.OpenUnitId)
            .HasMaxLength(8);
        
        builder.Property(s => s.Name)
            .HasMaxLength(50);
        
        builder.Property(s => s.Phone)
            .HasMaxLength(20);
        
        builder.Property(s => s.Address)
            .HasMaxLength(500);
    }
}