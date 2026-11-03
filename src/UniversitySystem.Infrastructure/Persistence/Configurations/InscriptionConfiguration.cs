using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

public class InscriptionConfiguration : IEntityTypeConfiguration<Inscription>
{
    public void Configure(EntityTypeBuilder<Inscription> b)
    {
        b.ToTable("inscriptions");
        b.HasKey(i => i.IdInscription);

        b.Property(i => i.IdInscription).HasColumnName("id_inscription").ValueGeneratedOnAdd();
        b.Property(i => i.IdEtudiant).HasColumnName("id_etudiant");
        b.Property(i => i.IdFiliere).HasColumnName("id_filiere");
        b.Property(i => i.AnneeAcademique).HasColumnName("annee_academique").HasMaxLength(9).IsRequired();
        b.Property(i => i.Niveau).HasColumnName("niveau");
        b.Property(i => i.Statut).HasColumnName("statut").HasConversion<int>();
        b.Property(i => i.DateSoumission).HasColumnName("date_soumission");
        b.Property(i => i.DateTraitement).HasColumnName("date_traitement");
        b.Property(i => i.Commentaire).HasColumnName("commentaire").HasMaxLength(500);
        b.Property(i => i.DateCreation).HasColumnName("date_creation");

        b.HasOne(i => i.Etudiant)
            .WithMany()
            .HasForeignKey(i => i.IdEtudiant)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(i => i.Filiere)
            .WithMany()
            .HasForeignKey(i => i.IdFiliere)
            .OnDelete(DeleteBehavior.Restrict);

        // Un étudiant ne peut avoir qu'une inscription par année académique
        b.HasIndex(i => new { i.IdEtudiant, i.AnneeAcademique }).IsUnique();
    }
}