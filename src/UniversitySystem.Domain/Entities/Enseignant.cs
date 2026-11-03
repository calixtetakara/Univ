using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Profil enseignant (lié à un Utilisateur).
/// </summary>
public class Enseignant
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdEnseignant { get; set; }

    /// <summary>Compte utilisateur associé.</summary>
    public int IdUtilisateur { get; set; }

    /// <summary>Matricule interne de l'enseignant.</summary>
    public string Matricule { get; set; } = default!;

    /// <summary>Grade / titre (ex: "Maître Assistant", "Professeur").</summary>
    public string? Grade { get; set; }

    /// <summary>Spécialité principale.</summary>
    public string? Specialite { get; set; }

    /// <summary>Statut de l'enseignant.</summary>
    public StatutEnseignant Statut { get; set; } = StatutEnseignant.Actif;

    /// <summary>Date de création du profil.</summary>
    public DateTime DateCreation { get; set; }

    /// <summary>Navigation vers le compte utilisateur.</summary>
    public Utilisateur Utilisateur { get; set; } = default!;
}