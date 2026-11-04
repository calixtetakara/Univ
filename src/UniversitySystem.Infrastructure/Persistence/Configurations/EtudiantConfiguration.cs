using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité Etudiant.
/// </summary>
public class EtudiantConfiguration : IEntityTypeConfiguration<Etudiant>
{
    public void Configure(EntityTypeBuilder<Etudiant> b)
    {
        b.ToTable("etudiants");

        b.HasKey(e => e.IdEtudiant);

        b.Property(e => e.IdEtudiant)
            .HasColumnName("id_etudiant")
            .ValueGeneratedOnAdd();

        b.Property(e => e.IdUtilisateur)
            .HasColumnName("id_utilisateur");

        b.Property(e => e.Matricule)
            .HasColumnName("matricule")
            .HasMaxLength(50)
            .IsRequired(false);

        b.HasIndex(e => e.Matricule).IsUnique();

        b.Property(e => e.DateNaissance)
            .HasColumnName("date_naissance");

        b.Property(e => e.LieuNaissance)
            .HasColumnName("lieu_naissance")
            .HasMaxLength(150);

        b.Property(e => e.Genre)
            .HasColumnName("genre")
            .HasConversion<int>();

        b.Property(e => e.Adresse)
            .HasColumnName("adresse")
            .HasMaxLength(255);

        b.Property(e => e.Nationalite)
            .HasColumnName("nationalite")
            .HasMaxLength(100);

        b.Property(e => e.IdFiliere)
            .HasColumnName("id_filiere");

        b.Property(e => e.DateCreation)
            .HasColumnName("date_creation");

        // Relation 1-1 avec Utilisateur
        b.HasOne(e => e.Utilisateur)
            .WithMany()
            .HasForeignKey(e => e.IdUtilisateur)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(e => e.IdUtilisateur).IsUnique(); // Un utilisateur = un seul profil étudiant

        b.HasOne(e => e.Filiere)
            .WithMany()
            .HasForeignKey(e => e.IdFiliere)
            .OnDelete(DeleteBehavior.SetNull);
    }
}