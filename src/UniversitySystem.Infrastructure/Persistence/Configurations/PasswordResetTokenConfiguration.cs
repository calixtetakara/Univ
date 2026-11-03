using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité PasswordResetToken.
/// </summary>
public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> b)
    {
        b.ToTable("password_reset_tokens");

        b.HasKey(t => t.IdToken);

        b.Property(t => t.IdToken)
            .HasColumnName("id_token")
            .ValueGeneratedOnAdd();

        b.Property(t => t.IdUtilisateur)
            .HasColumnName("id_utilisateur");

        b.Property(t => t.Token)
            .HasColumnName("token")
            .HasMaxLength(100)
            .IsRequired();

        // Un token doit être unique
        b.HasIndex(t => t.Token).IsUnique();

        b.Property(t => t.DateExpiration)
            .HasColumnName("date_expiration");

        b.Property(t => t.Utilise)
            .HasColumnName("utilise")
            .HasDefaultValue(false);

        b.Property(t => t.DateCreation)
            .HasColumnName("date_creation");

        b.HasOne(t => t.Utilisateur)
            .WithMany()
            .HasForeignKey(t => t.IdUtilisateur)
            .OnDelete(DeleteBehavior.Cascade);
    }
}