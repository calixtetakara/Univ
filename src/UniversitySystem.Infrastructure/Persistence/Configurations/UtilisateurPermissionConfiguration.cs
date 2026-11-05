using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

public class UtilisateurPermissionConfiguration : IEntityTypeConfiguration<UtilisateurPermission>
{
    public void Configure(EntityTypeBuilder<UtilisateurPermission> b)
    {
        b.ToTable("utilisateur_permissions");
        
        // Clé primaire composite
        b.HasKey(up => new { up.IdUtilisateur, up.IdPermission });

        b.Property(up => up.Accorde).HasColumnName("accorde");
        b.Property(up => up.ModifiePar).HasColumnName("modifie_par");

        b.HasOne(up => up.Utilisateur)
            .WithMany()
            .HasForeignKey(up => up.IdUtilisateur)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(up => up.Permission)
            .WithMany()
            .HasForeignKey(up => up.IdPermission)
            .OnDelete(DeleteBehavior.Cascade);
    }
}