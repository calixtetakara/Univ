using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Entities;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;

namespace UniversitySystem.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeder idempotent du Milestone 0 (M0-09).
/// Insère les 5 rôles système, les 17 permissions, la matrice RBAC
/// et le compte Super-Admin initial.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. Rôles système (5)
        await SeedRolesAsync(context);

        // 2. Permissions (17)
        await SeedPermissionsAsync(context);

        // 3. Matrice RolePermission
        await SeedRolePermissionsAsync(context);

        // 4. Super-Admin initial
        await SeedSuperAdminAsync(context);

        // 5. Paramètres calendrier
        await SeedParametresCalendrierAsync(context);
    }

    private static async Task SeedRolesAsync(AppDbContext context)
    {
        if (await context.Roles.AnyAsync()) return;

        var roles = new List<Role>
        {
            new() { Code = "superadmin", Nom = "Super Administrateur", Description = "Accès total au système. Non supprimable.", EstSysteme = true, Ordre = 1 },
            new() { Code = "scolarite", Nom = "Scolarité", Description = "Gestion des étudiants, inscriptions, notes, documents.", EstSysteme = true, Ordre = 2 },
            new() { Code = "enseignant", Nom = "Enseignant", Description = "Saisie des notes, consultation des programmations.", EstSysteme = true, Ordre = 3 },
            new() { Code = "etudiant", Nom = "Étudiant", Description = "Consultation notes, emploi du temps, documents.", EstSysteme = true, Ordre = 4 },
            new() { Code = "directeur", Nom = "Directeur de filière", Description = "Supervision d'une ou plusieurs filières.", EstSysteme = true, Ordre = 5 }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPermissionsAsync(AppDbContext context)
    {
        if (await context.Permissions.AnyAsync()) return;

        var permissions = new List<Permission>
        {
            new() { Code = "users.manage",       Nom = "Gérer les utilisateurs",          Description = "CRUD utilisateurs + attribution rôles" },
            new() { Code = "roles.manage",       Nom = "Gérer les rôles",                 Description = "CRUD rôles et matrice permissions" },
            new() { Code = "permissions.view",   Nom = "Voir les permissions",            Description = "Consultation seule des permissions" },
            new() { Code = "students.manage",    Nom = "Gérer les étudiants",             Description = "CRUD étudiants + dossier" },
            new() { Code = "inscriptions.manage",Nom = "Gérer les inscriptions",          Description = "Inscriptions, réinscriptions, validation" },
            new() { Code = "documents.manage",   Nom = "Gérer les documents étudiants",   Description = "Upload, validation, suppression documents" },
            new() { Code = "notes.saisie",       Nom = "Saisir les notes",                Description = "Saisie et modification des notes" },
            new() { Code = "notes.validation",   Nom = "Valider les notes",               Description = "Validation définitive des notes" },
            new() { Code = "notes.view",         Nom = "Consulter les notes",             Description = "Consultation seule des notes" },
            new() { Code = "programmation.manage", Nom = "Gérer la programmation",       Description = "Création et modification des séances" },
            new() { Code = "programmation.view",   Nom = "Voir la programmation",         Description = "Consultation emploi du temps" },
            new() { Code = "paiements.manage",   Nom = "Gérer les paiements",             Description = "Enregistrement et suivi des paiements" },
            new() { Code = "paiements.view",     Nom = "Voir les paiements",               Description = "Consultation seule des paiements" },
            new() { Code = "exports.generate",   Nom = "Générer des exports",             Description = "PDF, Excel, CSV" },
            new() { Code = "audit.view",         Nom = "Consulter l'audit",                Description = "Consultation des logs d'audit" },
            new() { Code = "settings.manage",    Nom = "Gérer les paramètres",            Description = "Calendrier académique, paramètres globaux" },
            new() { Code = "dashboard.view",     Nom = "Voir le tableau de bord",         Description = "Accès au dashboard selon le rôle" }
        };

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRolePermissionsAsync(AppDbContext context)
    {
        if (await context.RolePermissions.AnyAsync()) return;

        var roles = await context.Roles.ToDictionaryAsync(r => r.Code);
        var perms = await context.Permissions.ToDictionaryAsync(p => p.Code);

        var matrix = new List<(string RoleCode, string[] PermissionCodes)>
        {
            ("superadmin", perms.Keys.ToArray()),
            ("scolarite", new[]
            {
                "students.manage", "inscriptions.manage", "documents.manage",
                "notes.view", "notes.validation", "programmation.view",
                "paiements.manage", "paiements.view", "exports.generate", "dashboard.view"
            }),
            ("enseignant", new[]
            {
                "notes.saisie", "notes.view", "programmation.view", "dashboard.view"
            }),
            ("etudiant", new[]
            {
                "notes.view", "programmation.view", "paiements.view", "dashboard.view"
            }),
            ("directeur", new[]
            {
                "students.manage", "inscriptions.manage", "notes.view", "notes.validation",
                "programmation.view", "programmation.manage", "exports.generate", "dashboard.view"
            })
        };

        foreach (var (roleCode, permissionCodes) in matrix)
        {
            if (!roles.TryGetValue(roleCode, out var role)) continue;

            foreach (var permCode in permissionCodes)
            {
                if (!perms.TryGetValue(permCode, out var permission)) continue;

                context.RolePermissions.Add(new RolePermission
                {
                    IdRole = role.IdRole,
                    IdPermission = permission.IdPermission
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminAsync(AppDbContext context)
    {
        const string email = "admin@univ.local";

        if (await context.Utilisateurs.AnyAsync(u => u.Email == email))
            return;

        var superAdminRole = await context.Roles.FirstAsync(r => r.Code == "superadmin");

        var admin = new Utilisateur
        {
            Email = email,
            MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Admin@2026!"),
            Nom = "Administrateur",
            Prenom = "Super",
            Statut = StatutUtilisateur.Actif,
            DoitChangerMotDePasse = true, // L'admin devra changer le mot de passe à la 1ère connexion
            DateCreation = DateTime.UtcNow,
            IdRole = superAdminRole.IdRole
        };

        context.Utilisateurs.Add(admin);
        await context.SaveChangesAsync();
    }

    private static async Task SeedParametresCalendrierAsync(AppDbContext context)
    {
        var anneeCourante = DateTime.UtcNow.Year;
        var anneeAcademique = $"{anneeCourante}-{anneeCourante + 1}";

        // On vérifie si les paramètres pour cette année existent déjà (idempotence)
        if (await context.ParametresCalendrier.AnyAsync(p => p.AnneeAcademique == anneeAcademique)) 
            return;

        var parametres = new List<ParametreCalendrier>
        {
            new() { Cle = "ANNEE_ACADEMIQUE", Valeur = anneeAcademique, AnneeAcademique = anneeAcademique, Description = "Année académique courante" },
            new() { Cle = "DATE_DEBUT_INSCRIPTION_LICENCE", Valeur = $"{anneeCourante}-09-01", AnneeAcademique = anneeAcademique, Description = "Début inscriptions Licence" },
            new() { Cle = "DATE_FIN_INSCRIPTION_LICENCE",   Valeur = $"{anneeCourante}-10-15", AnneeAcademique = anneeAcademique, Description = "Fin inscriptions Licence" },
            new() { Cle = "DATE_DEBUT_INSCRIPTION_MASTER",  Valeur = $"{anneeCourante}-09-01", AnneeAcademique = anneeAcademique, Description = "Début inscriptions Master" },
            new() { Cle = "DATE_FIN_INSCRIPTION_MASTER",    Valeur = $"{anneeCourante}-10-31", AnneeAcademique = anneeAcademique, Description = "Fin inscriptions Master" },
        };

        await context.ParametresCalendrier.AddRangeAsync(parametres);
        await context.SaveChangesAsync();
    }
}