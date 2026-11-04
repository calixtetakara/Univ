using System;
using System.Collections.Generic;

namespace UniversitySystem.Web.Models;

/// <summary>
/// ViewModel pour la liste des dossiers en cours (Table de décision - M3-02).
/// </summary>
public class DecisionIndexViewModel
{
    /// <summary>Liste des dossiers à afficher.</summary>
    public IReadOnlyList<DossierEnCoursDto> Dossiers { get; set; } = new List<DossierEnCoursDto>();

    /// <summary>Filtre de recherche par nom.</summary>
    public string? SearchNom { get; set; }

    /// <summary>Filtre par code faculté.</summary>
    public string? CodeFaculte { get; set; }

    /// <summary>Filtre par code filière.</summary>
    public string? CodeFiliere { get; set; }

    /// <summary>Numéro de la page courante (commence à 1).</summary>
    public int Page { get; set; } = 1;

    /// <summary>Nombre total de pages.</summary>
    public int TotalPages { get; set; }

    /// <summary>Nombre total de dossiers (pour affichage).</summary>
    public int TotalCount { get; set; }
}

/// <summary>
/// DTO représentant un dossier dans la liste.
/// </summary>
public class DossierEnCoursDto
{
    public int IdEtudiant { get; set; }
    public string? Matricule { get; set; } // Sera null pour les en_cours
    public string Nom { get; set; } = default!;
    public string Prenom { get; set; } = default!;
    public string NomFaculte { get; set; } = default!;
    public string NomFiliere { get; set; } = default!;
    public DateTime DateSoumission { get; set; }
}