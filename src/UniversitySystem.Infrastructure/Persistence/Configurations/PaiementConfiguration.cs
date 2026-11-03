using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

public class PaiementConfiguration : IEntityTypeConfiguration<Paiement>
{
    public void Configure(EntityTypeBuilder<Paiement> b)
    {
        b.ToTable("paiements");
        b.HasKey(p => p.IdPaiement);

        b.Property(p => p.IdPaiement).HasColumnName("id_paiement").ValueGeneratedOnAdd();
        b.Property(p => p.IdInscription).HasColumnName("id_inscription");
        b.Property(p => p.Montant).HasColumnName("montant").HasPrecision(12, 2);
        b.Property(p => p.ReferenceTransaction).HasColumnName("reference_transaction").HasMaxLength(100);
        b.Property(p => p.ModePaiement).HasColumnName("mode_paiement").HasMaxLength(50).IsRequired();
        b.Property(p => p.Statut).HasColumnName("statut").HasConversion<int>();
        b.Property(p => p.DatePaiement).HasColumnName("date_paiement");
        b.Property(p => p.DateTraitement).HasColumnName("date_traitement");

        b.HasOne(p => p.Inscription)
            .WithMany()
            .HasForeignKey(p => p.IdInscription)
            .OnDelete(DeleteBehavior.Restrict);
    }
}