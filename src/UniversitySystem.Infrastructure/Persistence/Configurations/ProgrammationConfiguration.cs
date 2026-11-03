using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

public class ProgrammationConfiguration : IEntityTypeConfiguration<Programmation>
{
    public void Configure(EntityTypeBuilder<Programmation> b)
    {
        b.ToTable("programmations");
        b.HasKey(p => p.IdProgrammation);

        b.Property(p => p.IdProgrammation).HasColumnName("id_programmation").ValueGeneratedOnAdd();
        b.Property(p => p.IdUe).HasColumnName("id_ue");
        b.Property(p => p.IdEnseignant).HasColumnName("id_enseignant");
        b.Property(p => p.IdSalle).HasColumnName("id_salle");
        b.Property(p => p.IdFiliere).HasColumnName("id_filiere");
        b.Property(p => p.JourSemaine).HasColumnName("jour_semaine");
        b.Property(p => p.HeureDebut).HasColumnName("heure_debut");
        b.Property(p => p.HeureFin).HasColumnName("heure_fin");
        b.Property(p => p.AnneeAcademique).HasColumnName("annee_academique").HasMaxLength(9).IsRequired();
        b.Property(p => p.Semestre).HasColumnName("semestre");

        b.HasOne(p => p.Ue).WithMany().HasForeignKey(p => p.IdUe).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.Enseignant).WithMany().HasForeignKey(p => p.IdEnseignant).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.Salle).WithMany().HasForeignKey(p => p.IdSalle).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.Filiere).WithMany().HasForeignKey(p => p.IdFiliere).OnDelete(DeleteBehavior.Restrict);
    }
}