namespace UniversitySystem.Domain.Enums;

/// <summary>
/// Cycle de vie d'une inscription étudiante.
/// </summary>
public enum StatutInscription
{
    /// <summary>Brouillon non encore soumis.</summary>
    Brouillon = 1,

    /// <summary>Soumis, en attente de validation.</summary>
    Soumise = 2,

    /// <summary>Validé par la scolarité.</summary>
    Validee = 3,

    /// <summary>Rejeté par la scolarité.</summary>
    Rejetee = 4,

    /// <summary>Annulé (par l'étudiant ou l'administration).</summary>
    Annulee = 5
}