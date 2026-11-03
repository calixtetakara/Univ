using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Role.
/// Mappe la table "roles" selon le MLD §6.2.1 du SFD.
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("roles");

        b.HasKey(r => r.IdRole);

        b.Property(r => r.IdRole)
            .HasColumnName("id_role")
            .ValueGeneratedOnAdd();

        b.Property(r => r.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        b.HasIndex(r => r.Code).IsUnique();

        b.Property(r => r.Nom)
            .HasColumnName("nom")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(255);

        b.Property(r => r.EstSysteme)
            .HasColumnName("est_systeme")
            .HasDefaultValue(false);

        b.Property(r => r.Ordre)
            .HasColumnName("ordre")
            .HasDefaultValue(0);
    }
}