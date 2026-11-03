using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Inscription d'un étudiant pour une année académique / filière.
/// </summary>
public class Inscription
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdInscription { get; set; }

    /// <summary>Étudiant concerné.</summary>
    public int IdEtudiant { get; set; }

    /// <summary>Filière d'inscription.</summary>
    public int IdFiliere { get; set; }

    /// <summary>Année académique (ex: "2025-2026").</summary>
    public string AnneeAcademique { get; set; } = default!;

    /// <summary>Niveau (ex: 1, 2, 3 pour L1, L2, L3).</summary>
    public int Niveau { get; set; }

    /// <summary>Statut de l'inscription.</summary>
    public StatutInscription Statut { get; set; } = StatutInscription.Brouillon;

    /// <summary>Date de soumission de l'inscription.</summary>
    public DateTime? DateSoumission { get; set; }

    /// <summary>Date de validation / rejet.</summary>
    public DateTime? DateTraitement { get; set; }

    /// <summary>Commentaire de la scolarité (motif de rejet par exemple).</summary>
    public string? Commentaire { get; set; }

    /// <summary>Date de création.</summary>
    public DateTime DateCreation { get; set; }

    /// <summary>Navigation vers l'étudiant.</summary>
    public Etudiant Etudiant { get; set; } = default!;

    /// <summary>Navigation vers la filière.</summary>
    public Filiere Filiere { get; set; } = default!;
}