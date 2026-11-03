using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité Permission.
/// </summary>
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> b)
    {
        b.ToTable("permissions");

        b.HasKey(p => p.IdPermission);

        b.Property(p => p.IdPermission)
            .HasColumnName("id_permission")
            .ValueGeneratedOnAdd();

        b.Property(p => p.Code)
            .HasColumnName("code")
            .HasMaxLength(100)
            .IsRequired();

        // Code unique pour éviter les doublons de permissions
        b.HasIndex(p => p.Code).IsUnique();

        b.Property(p => p.Nom)
            .HasColumnName("nom")
            .HasMaxLength(150)
            .IsRequired();

        b.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(255);

        b.Property(p => p.Module)
            .HasColumnName("module")
            .HasMaxLength(50);
    }
}