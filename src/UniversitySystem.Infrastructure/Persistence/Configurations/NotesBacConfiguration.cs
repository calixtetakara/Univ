using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

public class NotesBacConfiguration : IEntityTypeConfiguration<NotesBac>
{
    public void Configure(EntityTypeBuilder<NotesBac> b)
    {
        b.ToTable("notes_bac");
        b.HasKey(n => n.IdNotesBac);

        b.Property(n => n.IdNotesBac).HasColumnName("id_notes_bac").ValueGeneratedOnAdd();
        b.Property(n => n.IdEtudiant).HasColumnName("id_etudiant");
        b.Property(n => n.Matiere).HasColumnName("matiere").HasMaxLength(100).IsRequired();
        b.Property(n => n.Note).HasColumnName("note").HasPrecision(4, 2);
        b.Property(n => n.Coefficient).HasColumnName("coefficient").HasPrecision(4, 2);

        b.HasOne(n => n.Etudiant)
            .WithMany()
            .HasForeignKey(n => n.IdEtudiant)
            .OnDelete(DeleteBehavior.Cascade);
    }
}