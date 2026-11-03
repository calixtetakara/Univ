using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité Faculte.
/// </summary>
public class FaculteConfiguration : IEntityTypeConfiguration<Faculte>
{
    public void Configure(EntityTypeBuilder<Faculte> b)
    {
        b.ToTable("facultes");

        b.HasKey(f => f.IdFaculte);

        b.Property(f => f.IdFaculte)
            .HasColumnName("id_faculte")
            .ValueGeneratedOnAdd();

        b.Property(f => f.Code)
            .HasColumnName("code")
            .HasMaxLength(20)
            .IsRequired();

        b.HasIndex(f => f.Code).IsUnique();

        b.Property(f => f.Nom)
            .HasColumnName("nom")
            .HasMaxLength(150)
            .IsRequired();

        b.Property(f => f.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        b.Property(f => f.Actif)
            .HasColumnName("actif")
            .HasDefaultValue(true);
    }
}