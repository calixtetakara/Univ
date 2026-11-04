using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Profil étudiant (lié à un Utilisateur).
/// </summary>
public class Etudiant
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdEtudiant { get; set; }

    /// <summary>Compte utilisateur associé.</summary>
    public int IdUtilisateur { get; set; }

    /// <summary>Matricule unique de l'étudiant (ex: 2026INFO0001), null tant que non validé.</summary>
    public string? Matricule { get; set; }

    /// <summary>Numéro de table du baccalauréat (obligatoire, RG-01).</summary>
    public string NumTableBac { get; set; } = default!;

    /// <summary>Année de session du baccalauréat (ex: 2024).</summary>
    public int SessionBac { get; set; }

    /// <summary>Date de naissance.</summary>
    public DateTime DateNaissance { get; set; }

    /// <summary>Lieu de naissance.</summary>
    public string? LieuNaissance { get; set; }

    /// <summary>Genre.</summary>
    public Genre Genre { get; set; }

    /// <summary>Adresse complète.</summary>
    public string? Adresse { get; set; }

    /// <summary>Nationalité.</summary>
    public string? Nationalite { get; set; }

    /// <summary>Filière d'inscription principale.</summary>
    public int? IdFiliere { get; set; }

    /// <summary>
    /// Statut du dossier d'inscription.
    /// Utilise l'enum StatutInscription existant.
    /// </summary>
    public StatutInscription StatutDossier { get; set; } = StatutInscription.Brouillon;

    /// <summary>Motif de rejet si le dossier est rejeté (RG-09).</summary>
    public string? MotifRejet { get; set; }

    /// <summary>Date de création du profil.</summary>
    public DateTime DateCreation { get; set; }

    /// <summary>Navigation vers le compte utilisateur.</summary>
    public Utilisateur Utilisateur { get; set; } = default!;

    /// <summary>Navigation vers la filière.</summary>
    public Filiere? Filiere { get; set; }
}