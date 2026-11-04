using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

public class FiliereUeConfiguration : IEntityTypeConfiguration<FiliereUe>
{
    public void Configure(EntityTypeBuilder<FiliereUe> b)
    {
        b.ToTable("filiere_ues");
        b.HasKey(fu => fu.IdFiliereUe);

        b.Property(fu => fu.IdFiliereUe).HasColumnName("id_filiere_ue").ValueGeneratedOnAdd();
        b.Property(fu => fu.IdFiliere).HasColumnName("id_filiere");
        b.Property(fu => fu.IdUe).HasColumnName("id_ue");
        b.Property(fu => fu.Semestre).HasColumnName("semestre");
        b.Property(fu => fu.Obligatoire).HasColumnName("obligatoire").HasDefaultValue(true);

        b.HasIndex(fu => new { fu.IdFiliere, fu.IdUe, fu.Semestre }).IsUnique();

        // Relation vers Filiere
        b.HasOne(fu => fu.Filiere)
            .WithMany(f => f.FiliereUes) // Filiere a bien FiliereUes
            .HasForeignKey(fu => fu.IdFiliere)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation vers Ue (on utilise WithMany() sans navigation inverse si Ue n'a pas FiliereUes)
        b.HasOne(fu => fu.Ue)
            .WithMany()
            .HasForeignKey(fu => fu.IdUe)
            .OnDelete(DeleteBehavior.Restrict);
    }
}