namespace UniversitySystem.Domain.Enums;

/// <summary>
/// Type de salle de cours.
/// </summary>
public enum TypeSalle
{
    /// <summary>Salle de cours classique.</summary>
    Cours = 1,

    /// <summary>Amphithéâtre.</summary>
    Amphi = 2,

    /// <summary>Salle informatique / laboratoire.</summary>
    Labo = 3,

    /// <summary>Salle de TD / travaux dirigés.</summary>
    TD = 4
}