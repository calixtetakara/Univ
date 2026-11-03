using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

public class DocumentEtudiantConfiguration : IEntityTypeConfiguration<DocumentEtudiant>
{
    public void Configure(EntityTypeBuilder<DocumentEtudiant> b)
    {
        b.ToTable("documents_etudiant");
        b.HasKey(d => d.IdDocument);

        b.Property(d => d.IdDocument).HasColumnName("id_document").ValueGeneratedOnAdd();
        b.Property(d => d.IdEtudiant).HasColumnName("id_etudiant");
        b.Property(d => d.Type).HasColumnName("type").HasConversion<int>();
        b.Property(d => d.NomFichier).HasColumnName("nom_fichier").HasMaxLength(255).IsRequired();
        b.Property(d => d.CheminFichier).HasColumnName("chemin_fichier").HasMaxLength(500).IsRequired();
        b.Property(d => d.TailleOctets).HasColumnName("taille_octets");
        b.Property(d => d.ContentType).HasColumnName("content_type").HasMaxLength(100).IsRequired();
        b.Property(d => d.DateUpload).HasColumnName("date_upload");

        b.HasOne(d => d.Etudiant)
            .WithMany()
            .HasForeignKey(d => d.IdEtudiant)
            .OnDelete(DeleteBehavior.Cascade);
    }
}