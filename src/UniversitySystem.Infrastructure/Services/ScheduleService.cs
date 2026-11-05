using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Entities;
using UniversitySystem.Infrastructure.Persistence;

namespace UniversitySystem.Infrastructure.Services;

/// <summary>
/// Exceptions métier pour les conflits de planification (RG-05).
/// </summary>
public class SalleConflitException : Exception
{
    public SalleConflitException(string message) : base(message) { }
}

public class EnseignantConflitException : Exception
{
    public EnseignantConflitException(string message) : base(message) { }
}

public class CapaciteDepasseeException : Exception
{
    public CapaciteDepasseeException(string message) : base(message) { }
}

public interface IScheduleService
{
    Task CreateAsync(int idEnseignant, string codeUe, int idSalle, int idFiliere, int jour, TimeSpan debut, TimeSpan fin, string annee, int semestre, int createdBy);
}

/// <summary>
/// Service de planification des cours (M5-04).
/// Gère les verrous consultatifs MySQL (GET_LOCK) et détecte les conflits.
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _context;

    public ScheduleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(int idEnseignant, string codeUe, int idSalle, int idFiliere, int jour, TimeSpan debut, TimeSpan fin, string annee, int semestre, int createdBy)
    {
        // 1. Vérifier que les entités existent
        var enseignant = await _context.Enseignants.FindAsync(idEnseignant);
        var salle = await _context.Salles.FindAsync(idSalle);
        var ue = await _context.Ues.FirstOrDefaultAsync(u => u.Code == codeUe);

        if (enseignant == null || salle == null || ue == null)
            throw new ArgumentException("Enseignant, salle ou UE introuvable.");

        // 2. Acquérir les verrous (dans l'ordre alphabétique pour éviter les deadlocks)
        // Clé de verrou : "sched:salle:{idSalle}" et "sched:ens:{idEnseignant}"
        var lockSalle = $"sched:salle:{idSalle}:{annee}";
        var lockEnseignant = $"sched:ens:{idEnseignant}:{annee}";

        // On trie pour éviter les deadlocks
        var locks = new[] { lockSalle, lockEnseignant }.OrderBy(x => x).ToList();

        try
        {
            // 3. Acquérir les verrous (timeout 5 secondes)
            foreach (var lockName in locks)
            {
                var acquired = await _context.Database.ExecuteSqlRawAsync(
                    "SELECT GET_LOCK({0}, 5)", lockName);

                if (acquired == 0)
                    throw new Exception($"Impossible d'acquérir le verrou {lockName} (timeout).");
            }

            // 4. Vérifier les conflits (RG-05)
            
            // 4.1 Conflit de salle
            var conflitSalle = await _context.Programmations
                .AnyAsync(p => p.IdSalle == idSalle
                    && p.AnneeAcademique == annee
                    && p.JourSemaine == jour
                    && p.HeureDebut < fin
                    && p.HeureFin > debut); // Chevauchement strict

            if (conflitSalle)
                throw new SalleConflitException("Cette salle est déjà réservée sur ce créneau.");

            // 4.2 Conflit d'enseignant
            var conflitEnseignant = await _context.Programmations
                .AnyAsync(p => p.IdEnseignant == idEnseignant
                    && p.AnneeAcademique == annee
                    && p.JourSemaine == jour
                    && p.HeureDebut < fin
                    && p.HeureFin > debut);

            if (conflitEnseignant)
                throw new EnseignantConflitException("Cet enseignant a déjà cours sur ce créneau.");

            // 4.3 Conflit de capacité (RG-05 ter)
            var effectif = await _context.Inscriptions
                .CountAsync(i => i.IdFiliere == idFiliere
                    && i.AnneeAcademique == annee
                    && i.Statut == Domain.Enums.StatutInscription.Validee);

            if (effectif > salle.Capacite)
                throw new CapaciteDepasseeException($"Capacité dépassée : {effectif} étudiants pour une salle de {salle.Capacite} places.");

            // 5. Insertion
            var programmation = new Programmation
            {
                IdEnseignant = idEnseignant,
                IdUe = ue.IdUe,
                IdSalle = idSalle,
                IdFiliere = idFiliere,
                JourSemaine = jour,
                HeureDebut = debut,
                HeureFin = fin,
                AnneeAcademique = annee,
                Semestre = semestre
            };

            _context.Programmations.Add(programmation);
            await _context.SaveChangesAsync();

            // 6. Audit
            _context.AuditLogs.Add(new AuditLog
            {
                IdUtilisateur = createdBy,
                Action = "PROGRAMMATION_CREEE",
                Entite = "Programmation",
                IdEntite = programmation.IdProgrammation.ToString(),
                Details = $"{{\"salle\":{idSalle}, \"enseignant\":{idEnseignant}, \"jour\":{jour}}}",
                DateAction = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
        finally
        {
            // 7. Libération des verrous (TOUJOURS, même en cas d'erreur)
            foreach (var lockName in locks)
            {
                await _context.Database.ExecuteSqlRawAsync("SELECT RELEASE_LOCK({0})", lockName);
            }
        }
    }
}