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

    /// <summary>Matricule unique de l'étudiant (ex: 2026INFO0001).</summary>
    public string Matricule { get; set; } = default!;

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

    /// <summary>Date de création du profil.</summary>
    public DateTime DateCreation { get; set; }

    /// <summary>Navigation vers le compte utilisateur.</summary>
    public Utilisateur Utilisateur { get; set; } = default!;

    /// <summary>Navigation vers la filière.</summary>
    public Filiere? Filiere { get; set; }
}