using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité Enseignant.
/// </summary>
public class EnseignantConfiguration : IEntityTypeConfiguration<Enseignant>
{
    public void Configure(EntityTypeBuilder<Enseignant> b)
    {
        b.ToTable("enseignants");

        b.HasKey(e => e.IdEnseignant);

        b.Property(e => e.IdEnseignant)
            .HasColumnName("id_enseignant")
            .ValueGeneratedOnAdd();

        b.Property(e => e.IdUtilisateur)
            .HasColumnName("id_utilisateur");

        b.Property(e => e.Matricule)
            .HasColumnName("matricule")
            .HasMaxLength(30)
            .IsRequired();

        b.HasIndex(e => e.Matricule).IsUnique();

        b.Property(e => e.Grade)
            .HasColumnName("grade")
            .HasMaxLength(100);

        b.Property(e => e.Specialite)
            .HasColumnName("specialite")
            .HasMaxLength(150);

        b.Property(e => e.Statut)
            .HasColumnName("statut")
            .HasConversion<int>();

        b.Property(e => e.DateCreation)
            .HasColumnName("date_creation");

        // Relation 1-1 avec Utilisateur
        b.HasOne(e => e.Utilisateur)
            .WithMany()
            .HasForeignKey(e => e.IdUtilisateur)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(e => e.IdUtilisateur).IsUnique();
    }
}