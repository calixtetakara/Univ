using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour la table de liaison RolePermission.
/// </summary>
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> b)
    {
        b.ToTable("role_permissions");

        // Clé composite
        b.HasKey(rp => new { rp.IdRole, rp.IdPermission });

        b.Property(rp => rp.IdRole).HasColumnName("id_role");
        b.Property(rp => rp.IdPermission).HasColumnName("id_permission");

        // Relation vers Role
        b.HasOne(rp => rp.Role)
            .WithMany()
            .HasForeignKey(rp => rp.IdRole)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation vers Permission
        b.HasOne(rp => rp.Permission)
            .WithMany()
            .HasForeignKey(rp => rp.IdPermission)
            .OnDelete(DeleteBehavior.Cascade);
    }
}