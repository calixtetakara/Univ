using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Fluent API pour l'entité EmailQueue.
/// </summary>
public class EmailQueueConfiguration : IEntityTypeConfiguration<EmailQueue>
{
    public void Configure(EntityTypeBuilder<EmailQueue> b)
    {
        b.ToTable("email_queue");

        b.HasKey(e => e.IdEmail);

        b.Property(e => e.IdEmail)
            .HasColumnName("id_email")
            .ValueGeneratedOnAdd();

        b.Property(e => e.Destinataire)
            .HasColumnName("destinataire")
            .HasMaxLength(255)
            .IsRequired();

        b.Property(e => e.Sujet)
            .HasColumnName("sujet")
            .HasMaxLength(300)
            .IsRequired();

        b.Property(e => e.CorpsHtml)
            .HasColumnName("corps_html")
            .HasColumnType("longtext")
            .IsRequired();

        b.Property(e => e.Envoye)
            .HasColumnName("envoye")
            .HasDefaultValue(false);

        b.Property(e => e.DateEnvoi)
            .HasColumnName("date_envoi");

        b.Property(e => e.Tentatives)
            .HasColumnName("tentatives")
            .HasDefaultValue(0);

        b.Property(e => e.DerniereErreur)
            .HasColumnName("derniere_erreur")
            .HasMaxLength(1000);

        b.Property(e => e.DateCreation)
            .HasColumnName("date_creation");
    }
}