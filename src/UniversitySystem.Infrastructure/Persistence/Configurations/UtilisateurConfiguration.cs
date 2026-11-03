using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité Utilisateur.
/// </summary>
public class UtilisateurConfiguration : IEntityTypeConfiguration<Utilisateur>
{
    public void Configure(EntityTypeBuilder<Utilisateur> b)
    {
        b.ToTable("utilisateurs");

        b.HasKey(u => u.IdUtilisateur);

        b.Property(u => u.IdUtilisateur)
            .HasColumnName("id_utilisateur")
            .ValueGeneratedOnAdd();

        b.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        // Email unique (sert de login)
        b.HasIndex(u => u.Email).IsUnique();

        b.Property(u => u.MotDePasseHash)
            .HasColumnName("mot_de_passe_hash")
            .HasMaxLength(255)
            .IsRequired();

        b.Property(u => u.Nom)
            .HasColumnName("nom")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(u => u.Prenom)
            .HasColumnName("prenom")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(u => u.Telephone)
            .HasColumnName("telephone")
            .HasMaxLength(30);

        b.Property(u => u.IdRole)
            .HasColumnName("id_role");

        // Stockage de l'enum en int
        b.Property(u => u.Statut)
            .HasColumnName("statut")
            .HasConversion<int>();

        b.Property(u => u.DoitChangerMotDePasse)
            .HasColumnName("doit_changer_mot_de_passe")
            .HasDefaultValue(false);

        b.Property(u => u.DerniereConnexion)
            .HasColumnName("derniere_connexion");

        b.Property(u => u.DateCreation)
            .HasColumnName("date_creation");

        b.Property(u => u.DateModification)
            .HasColumnName("date_modification");

        // Relation vers Role (Restrict pour éviter la suppression accidentelle d'un rôle utilisé)
        b.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.IdRole)
            .OnDelete(DeleteBehavior.Restrict);
    }
}