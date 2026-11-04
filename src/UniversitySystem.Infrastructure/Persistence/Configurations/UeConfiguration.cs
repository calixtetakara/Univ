using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

public class UeConfiguration : IEntityTypeConfiguration<Ue>
{
    public void Configure(EntityTypeBuilder<Ue> b)
    {
        b.ToTable("ues");
        b.HasKey(u => u.IdUe);

        b.Property(u => u.IdUe).HasColumnName("id_ue").ValueGeneratedOnAdd();
        b.Property(u => u.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
        b.HasIndex(u => u.Code).IsUnique();
        b.Property(u => u.Intitule).HasColumnName("intitule").HasMaxLength(200).IsRequired();
        b.Property(u => u.Credits).HasColumnName("credits");
        b.Property(u => u.VolumeHoraire).HasColumnName("volume_horaire");
        b.Property(u => u.Description).HasColumnName("description").HasMaxLength(500);
        b.Property(u => u.Actif).HasColumnName("actif").HasDefaultValue(true);
    }
}