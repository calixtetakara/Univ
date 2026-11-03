using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour la liaison FiliereUe.
/// </summary>
public class FiliereUeConfiguration : IEntityTypeConfiguration<FiliereUe>
{
    public void Configure(EntityTypeBuilder<FiliereUe> b)
    {
        b.ToTable("filiere_ues");

        b.HasKey(fu => fu.IdFiliereUe);

        b.Property(fu => fu.IdFiliereUe)
            .HasColumnName("id_filiere_ue")
            .ValueGeneratedOnAdd();

        b.Property(fu => fu.IdFiliere).HasColumnName("id_filiere");
        b.Property(fu => fu.IdUe).HasColumnName("id_ue");
        b.Property(fu => fu.Semestre).HasColumnName("semestre");
        b.Property(fu => fu.Obligatoire)
            .HasColumnName("obligatoire")
            .HasDefaultValue(true);

        // Unicité : une UE ne peut apparaître qu'une seule fois par filière + semestre
        b.HasIndex(fu => new { fu.IdFiliere, fu.IdUe, fu.Semestre }).IsUnique();

        b.HasOne(fu => fu.Filiere)
            .WithMany(f => f.FiliereUes)
            .HasForeignKey(fu => fu.IdFiliere)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(fu => fu.Ue)
            .WithMany(u => u.FiliereUes)
            .HasForeignKey(fu => fu.IdUe)
            .OnDelete(DeleteBehavior.Restrict);
    }
}