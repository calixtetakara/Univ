using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité ExportJob.
/// </summary>
public class ExportJobConfiguration : IEntityTypeConfiguration<ExportJob>
{
    public void Configure(EntityTypeBuilder<ExportJob> b)
    {
        b.ToTable("export_jobs");

        b.HasKey(e => e.IdExportJob);

        b.Property(e => e.IdExportJob)
            .HasColumnName("id_export_job")
            .ValueGeneratedOnAdd();

        b.Property(e => e.IdUtilisateur)
            .HasColumnName("id_utilisateur");

        b.Property(e => e.Type)
            .HasColumnName("type")
            .HasConversion<int>();

        b.Property(e => e.NomFichier)
            .HasColumnName("nom_fichier")
            .HasMaxLength(255);

        b.Property(e => e.CheminFichier)
            .HasColumnName("chemin_fichier")
            .HasMaxLength(500);

        b.Property(e => e.Statut)
            .HasColumnName("statut")
            .HasMaxLength(30)
            .HasDefaultValue("Pending");

        b.Property(e => e.MessageErreur)
            .HasColumnName("message_erreur")
            .HasMaxLength(1000);

        b.Property(e => e.DateCreation)
            .HasColumnName("date_creation");

        b.Property(e => e.DateFin)
            .HasColumnName("date_fin");

        b.HasOne(e => e.Utilisateur)
            .WithMany()
            .HasForeignKey(e => e.IdUtilisateur)
            .OnDelete(DeleteBehavior.Restrict);
    }
}