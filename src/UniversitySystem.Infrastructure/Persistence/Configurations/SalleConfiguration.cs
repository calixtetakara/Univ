using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

public class SalleConfiguration : IEntityTypeConfiguration<Salle>
{
    public void Configure(EntityTypeBuilder<Salle> b)
    {
        b.ToTable("salles");
        b.HasKey(s => s.IdSalle);

        b.Property(s => s.IdSalle).HasColumnName("id_salle").ValueGeneratedOnAdd();
        b.Property(s => s.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
        b.HasIndex(s => s.Code).IsUnique();
        b.Property(s => s.Nom).HasColumnName("nom").HasMaxLength(100).IsRequired();
        b.Property(s => s.Capacite).HasColumnName("capacite");
        b.Property(s => s.Type).HasColumnName("type").HasConversion<int>();
        b.Property(s => s.Batiment).HasColumnName("batiment").HasMaxLength(100);
        b.Property(s => s.Actif).HasColumnName("actif").HasDefaultValue(true);
    }
}