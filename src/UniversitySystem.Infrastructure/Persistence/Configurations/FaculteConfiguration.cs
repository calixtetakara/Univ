using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence.Configurations;

public class FaculteConfiguration : IEntityTypeConfiguration<Faculte>
{
    public void Configure(EntityTypeBuilder<Faculte> b)
    {
        b.ToTable("facultes");
        b.HasKey(f => f.IdFaculte);
        
        b.Property(f => f.IdFaculte).HasColumnName("id_faculte").ValueGeneratedOnAdd();
        b.Property(f => f.CodeFaculte).HasColumnName("code_faculte").HasMaxLength(20).IsRequired();
        b.HasIndex(f => f.CodeFaculte).IsUnique();
        
        b.Property(f => f.NomFaculte).HasColumnName("nom_faculte").HasMaxLength(150).IsRequired();
        b.Property(f => f.TypeFaculte).HasColumnName("type_faculte").HasConversion<int>();
        b.Property(f => f.Description).HasColumnName("description").HasMaxLength(500);
        b.Property(f => f.Actif).HasColumnName("actif").HasDefaultValue(true);
    }
}