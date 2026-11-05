using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Entities;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;

namespace UniversitySystem.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeder idempotent du Milestone 0 à 5.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await SeedRolesAsync(context);
        await SeedPermissionsAsync(context);
        await SeedRolePermissionsAsync(context);
        await SeedSuperAdminAsync(context);
        await SeedParametresCalendrierAsync(context);
        await SeedFacultesEtFilieresAsync(context);
    }

    private static async Task SeedRolesAsync(AppDbContext context)
    {
        if (await context.Roles.AnyAsync()) return;

        var roles = new List<Role>
        {
            new() { Code = "superadmin", Nom = "Super Administrateur", Description = "Accès total au système. Non supprimable.", EstSysteme = true, Ordre = 1 },
            new() { Code = "scolarite", Nom = "Scolarité", Description = "Gestion des étudiants, inscriptions, notes, documents.", EstSysteme = true, Ordre = 2 },
            new() { Code = "pedagogie", Nom = "Pédagogie", Description = "Gestion des enseignants et du planning.", EstSysteme = true, Ordre = 3 },
            new() { Code = "enseignant", Nom = "Enseignant", Description = "Saisie des notes, consultation des programmations.", EstSysteme = true, Ordre = 4 },
            new() { Code = "etudiant", Nom = "Étudiant", Description = "Consultation notes, emploi du temps, documents.", EstSysteme = true, Ordre = 5 },
            new() { Code = "directeur", Nom = "Directeur de filière", Description = "Supervision d'une ou plusieurs filières.", EstSysteme = true, Ordre = 6 }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPermissionsAsync(AppDbContext context)
    {
        if (await context.Permissions.AnyAsync()) return;

        var permissions = new List<Permission>
        {
            // Anciennes permissions
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
            new() { Code = "dashboard.view",     Nom = "Voir le tableau de bord",         Description = "Accès au dashboard selon le rôle" },
            
            // Nouvelles permissions (M3)
            new() { Code = "DOSSIER_LIRE",       Nom = "Voir les dossiers",               Description = "Consulter la table de décision" },
            new() { Code = "DOSSIER_TRAITER",    Nom = "Traiter les dossiers",            Description = "Approuver ou rejeter un dossier" },
            new() { Code = "CONFIG_ACADEMIQUE",  Nom = "Configurer l'académique",         Description = "Gérer les facultés, filières et UE" },
            new() { Code = "ETUDIANTS_LISTER",   Nom = "Lister les étudiants",            Description = "Voir la liste globale des étudiants" },

            // Nouvelles permissions (M4)
            new() { Code = "PAIEMENT_EFFECTUER", Nom = "Effectuer un paiement", Description = "Permet de payer les frais de scolarité" },
            new() { Code = "RECU_TELECHARGER", Nom = "Télécharger un reçu", Description = "Permet de télécharger les reçus de paiement" },

            // Nouvelles permissions (M5)
            new() { Code = "ENSEIGNANT_CREER", Nom = "Créer des enseignants", Description = "Gestion des comptes enseignants" },
            new() { Code = "PROGRAMMATION_GERER", Nom = "Gérer le planning", Description = "Création et modification des cours" },
            new() { Code = "PLANNING_CONSULTER_PERSO", Nom = "Consulter son planning", Description = "Voir ses cours (enseignant)" },
            new() { Code = "ETUDIANTS_CLASSE_LIRE", Nom = "Lire les étudiants de sa classe", Description = "Voir les étudiants de ses filières" },
                        // Nouvelles permissions (M6)
            new() { Code = "PERMISSIONS_GERER", Nom = "Gérer les permissions", Description = "Accorder ou retirer des permissions" },
            new() { Code = "FINANCES_LIRE", Nom = "Voir les finances", Description = "Consulter les paiements" },
            new() { Code = "KPI_LIRE", Nom = "Voir les KPI", Description = "Consulter les indicateurs de performance" }
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
            // SuperAdmin a TOUTES les permissions
            ("superadmin", perms.Keys.ToArray()),
            
            // Scolarité
            ("scolarite", new[]
            {
                "students.manage", "inscriptions.manage", "documents.manage",
                "notes.view", "notes.validation", "programmation.view",
                "paiements.manage", "paiements.view", "exports.generate", "dashboard.view",
                "DOSSIER_LIRE", "DOSSIER_TRAITER", "CONFIG_ACADEMIQUE", "ETUDIANTS_LISTER"
            }),

            // Pédagogie
            ("pedagogie", new[] { "ENSEIGNANT_CREER", "PROGRAMMATION_GERER", "dashboard.view" }),
            
            // Enseignant
            ("enseignant", new[]
            {
                "notes.saisie", "notes.view", "programmation.view", "dashboard.view", "PLANNING_CONSULTER_PERSO"
            }),
            
            // Étudiant
            ("etudiant", new[]
            {
                "notes.view", "programmation.view", "paiements.view", "dashboard.view",
                "PAIEMENT_EFFECTUER", "RECU_TELECHARGER"
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
            DoitChangerMotDePasse = true,
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

    private static async Task SeedFacultesEtFilieresAsync(AppDbContext context)
    {
        if (await context.Facultes.AnyAsync()) return;

        var fds = new Faculte { CodeFaculte = "FDS", NomFaculte = "Faculté des Sciences", TypeFaculte = TypeFaculte.Scientifique };
        var faseg = new Faculte { CodeFaculte = "FASEG", NomFaculte = "Faculté des Sciences Économiques et de Gestion", TypeFaculte = TypeFaculte.Litteraire };

        context.Facultes.AddRange(fds, faseg);
        await context.SaveChangesAsync();

        var filiereInfo = new Filiere { CodeFiliere = "LIC-INFO", NomFiliere = "Licence Informatique", IdFaculte = fds.IdFaculte };
        var filiereMath = new Filiere { CodeFiliere = "LIC-MATH", NomFiliere = "Licence Mathématiques", IdFaculte = fds.IdFaculte };
        var filiereGest = new Filiere { CodeFiliere = "LIC-GEST", NomFiliere = "Licence Gestion", IdFaculte = faseg.IdFaculte };

        context.Filieres.AddRange(filiereInfo, filiereMath, filiereGest);
        await context.SaveChangesAsync();

        // Ajout d'UE de test
        context.Ues.AddRange(
            new Ue { Code = "INF101", Intitule = "Programmation", Credits = 6, VolumeHoraire = 60 },
            new Ue { Code = "MAT101", Intitule = "Analyse Mathématique", Credits = 6, VolumeHoraire = 60 },
            new Ue { Code = "GES101", Intitule = "Comptabilité Générale", Credits = 6, VolumeHoraire = 60 }
        );
        await context.SaveChangesAsync();
    }
            // Dans SeedAsync, ajoute :
        // await SeedSallesAsync(context);

        private static async Task SeedSallesAsync(AppDbContext context)
        {
            if (await context.Salles.AnyAsync()) return;

            context.Salles.AddRange(
                new Salle { Code = "A101", Nom = "Salle A101", Capacite = 30, Type = TypeSalle.Cours },
                new Salle { Code = "B201", Nom = "Salle B201", Capacite = 50, Type = TypeSalle.Cours },
                new Salle { Code = "AMPHI-1", Nom = "Amphithéâtre 1", Capacite = 200, Type = TypeSalle.Amphi }
            );
            await context.SaveChangesAsync();
        }
}