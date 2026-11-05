using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Persistence;

/// <summary>
/// Contexte EF Core principal de l'application.
/// Charge automatiquement toutes les configurations Fluent API.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>Rôles du système RBAC.</summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>Permissions atomiques.</summary>
    public DbSet<Permission> Permissions => Set<Permission>();

    /// <summary>Liaison Role ↔ Permission.</summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    /// <summary>Comptes utilisateurs.</summary>
    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();

    public DbSet<Faculte> Facultes => Set<Faculte>();
    public DbSet<Filiere> Filieres => Set<Filiere>();
    public DbSet<Ue> Ues => Set<Ue>();
    public DbSet<FiliereUe> FiliereUes => Set<FiliereUe>();
    public DbSet<Etudiant> Etudiants => Set<Etudiant>();
    public DbSet<Enseignant> Enseignants => Set<Enseignant>();

    public DbSet<Salle> Salles => Set<Salle>();
    public DbSet<Inscription> Inscriptions => Set<Inscription>();
    public DbSet<NotesBac> NotesBacs => Set<NotesBac>();
    public DbSet<DocumentEtudiant> DocumentsEtudiant => Set<DocumentEtudiant>();
    public DbSet<Programmation> Programmations => Set<Programmation>();
    public DbSet<Paiement> Paiements => Set<Paiement>();

    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<EmailQueue> EmailQueues => Set<EmailQueue>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ParametreCalendrier> ParametresCalendrier => Set<ParametreCalendrier>();
    public DbSet<ExportJob> ExportJobs => Set<ExportJob>();
    public DbSet<UtilisateurPermission> UtilisateurPermissions { get; set; }
    public DbSet<Compteur> Compteurs => Set<Compteur>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Applique toutes les classes IEntityTypeConfiguration<> de l'assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}