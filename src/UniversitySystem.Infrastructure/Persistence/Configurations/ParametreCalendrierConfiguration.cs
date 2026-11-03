using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité ParametreCalendrier.
/// </summary>
public class ParametreCalendrierConfiguration : IEntityTypeConfiguration<ParametreCalendrier>
{
    public void Configure(EntityTypeBuilder<ParametreCalendrier> b)
    {
        b.ToTable("parametres_calendrier");

        b.HasKey(p => p.IdParametre);

        b.Property(p => p.IdParametre)
            .HasColumnName("id_parametre")
            .ValueGeneratedOnAdd();

        b.Property(p => p.Cle)
            .HasColumnName("cle")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(p => p.Valeur)
            .HasColumnName("valeur")
            .HasMaxLength(255)
            .IsRequired();

        b.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        b.Property(p => p.AnneeAcademique)
            .HasColumnName("annee_academique")
            .HasMaxLength(9)
            .IsRequired();

        // Une clé est unique par année académique
        b.HasIndex(p => new { p.Cle, p.AnneeAcademique }).IsUnique();
    }
}