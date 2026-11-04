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

        b.Property(f => f.CodeFiliere)
            .HasColumnName("code_filiere")
            .HasMaxLength(20)
            .IsRequired();

        b.HasIndex(f => f.CodeFiliere).IsUnique();

        b.Property(f => f.NomFiliere)
            .HasColumnName("nom_filiere")
            .HasMaxLength(150)
            .IsRequired();

        b.Property(f => f.IdFaculte)
            .HasColumnName("id_faculte");

        // Relation vers Faculte
        b.HasOne(f => f.Faculte)
            .WithMany(fac => fac.Filieres)
            .HasForeignKey(f => f.IdFaculte)
            .OnDelete(DeleteBehavior.Restrict);
    }
}