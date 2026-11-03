using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité AuditLog.
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.ToTable("audit_logs");

        b.HasKey(a => a.IdAudit);

        b.Property(a => a.IdAudit)
            .HasColumnName("id_audit")
            .ValueGeneratedOnAdd();

        b.Property(a => a.IdUtilisateur)
            .HasColumnName("id_utilisateur");

        b.Property(a => a.Action)
            .HasColumnName("action")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(a => a.Entite)
            .HasColumnName("entite")
            .HasMaxLength(100);

        b.Property(a => a.IdEntite)
            .HasColumnName("id_entite")
            .HasMaxLength(50);

        b.Property(a => a.Details)
            .HasColumnName("details")
            .HasColumnType("text");

        b.Property(a => a.AdresseIp)
            .HasColumnName("adresse_ip")
            .HasMaxLength(45);

        b.Property(a => a.DateAction)
            .HasColumnName("date_action");

        // Index pour les recherches fréquentes
        b.HasIndex(a => a.DateAction);
        b.HasIndex(a => a.Action);
        b.HasIndex(a => a.IdUtilisateur);
    }
}