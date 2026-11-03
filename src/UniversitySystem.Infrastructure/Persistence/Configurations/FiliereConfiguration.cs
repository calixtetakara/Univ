using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité Filiere.
/// </summary>
public class FiliereConfiguration : IEntityTypeConfiguration<Filiere>
{
    public void Configure(EntityTypeBuilder<Filiere> b)
    {
        b.ToTable("filieres");

        b.HasKey(f => f.IdFiliere);

        b.Property(f => f.IdFiliere)
            .HasColumnName("id_filiere")
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

        b.Property(f => f.IdFaculte)
            .HasColumnName("id_faculte");

        b.Property(f => f.Actif)
            .HasColumnName("actif")
            .HasDefaultValue(true);

        b.HasOne(f => f.Faculte)
            .WithMany(fa => fa.Filieres)
            .HasForeignKey(f => f.IdFaculte)
            .OnDelete(DeleteBehavior.Restrict);
    }
}