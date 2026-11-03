using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité Compteur.
/// </summary>
public class CompteurConfiguration : IEntityTypeConfiguration<Compteur>
{
    public void Configure(EntityTypeBuilder<Compteur> b)
    {
        b.ToTable("compteurs");

        b.HasKey(c => c.IdCompteur);

        b.Property(c => c.IdCompteur)
            .HasColumnName("id_compteur")
            .ValueGeneratedOnAdd();

        b.Property(c => c.Cle)
            .HasColumnName("cle")
            .HasMaxLength(100)
            .IsRequired();

        // La clé du compteur doit être unique
        b.HasIndex(c => c.Cle).IsUnique();

        b.Property(c => c.Valeur)
            .HasColumnName("valeur")
            .HasDefaultValue(0);
    }
}