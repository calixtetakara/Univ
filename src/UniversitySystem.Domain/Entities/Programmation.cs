namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Programmation d'un cours (emploi du temps).
/// </summary>
public class Programmation
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdProgrammation { get; set; }

    /// <summary>UE enseignée.</summary>
    public int IdUe { get; set; }

    /// <summary>Enseignant assigné.</summary>
    public int IdEnseignant { get; set; }

    /// <summary>Salle réservée.</summary>
    public int IdSalle { get; set; }

    /// <summary>Filière concernée.</summary>
    public int IdFiliere { get; set; }

    /// <summary>Jour de la semaine (1 = Lundi ... 7 = Dimanche).</summary>
    public int JourSemaine { get; set; }

    /// <summary>Heure de début.</summary>
    public TimeSpan HeureDebut { get; set; }

    /// <summary>Heure de fin.</summary>
    public TimeSpan HeureFin { get; set; }

    /// <summary>Année académique.</summary>
    public string AnneeAcademique { get; set; } = default!;

    /// <summary>Semestre (1 ou 2).</summary>
    public int Semestre { get; set; }

    /// <summary>Navigation vers l'UE.</summary>
    public Ue Ue { get; set; } = default!;

    /// <summary>Navigation vers l'enseignant.</summary>
    public Enseignant Enseignant { get; set; } = default!;

    /// <summary>Navigation vers la salle.</summary>
    public Salle Salle { get; set; } = default!;

    /// <summary>Navigation vers la filière.</summary>
    public Filiere Filiere { get; set; } = default!;
}