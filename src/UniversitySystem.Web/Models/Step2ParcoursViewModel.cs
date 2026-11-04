using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Web.Models;

/// <summary>
/// ViewModel pour l'étape 2 du wizard d'inscription (Parcours & Notes).
/// </summary>
public class Step2ParcoursViewModel
{
    [Required(ErrorMessage = "Le niveau est obligatoire.")]
    [Display(Name = "Niveau d'études")]
    public string Niveau { get; set; } = string.Empty; // "Licence" ou "Master"

    [Required(ErrorMessage = "La faculté est obligatoire.")]
    [Display(Name = "Faculté")]
    public string CodeFaculte { get; set; } = string.Empty;

    [Required(ErrorMessage = "La filière est obligatoire.")]
    [Display(Name = "Filière")]
    public string CodeFiliere { get; set; } = string.Empty;

    // === Notes Scientifiques ===
    [Range(0, 20, ErrorMessage = "La note doit être entre 0 et 20.")]
    [Display(Name = "Mathématiques")]
    public decimal? NoteMaths { get; set; }

    [Range(0, 20, ErrorMessage = "La note doit être entre 0 et 20.")]
    [Display(Name = "SVT")]
    public decimal? NoteSvt { get; set; }

    [Range(0, 20, ErrorMessage = "La note doit être entre 0 et 20.")]
    [Display(Name = "Physique-Chimie")]
    public decimal? NotePhysique { get; set; }

    // === Notes Littéraires ===
    [Range(0, 20, ErrorMessage = "La note doit être entre 0 et 20.")]
    [Display(Name = "Français")]
    public decimal? NoteFrancais { get; set; }

    [Range(0, 20, ErrorMessage = "La note doit être entre 0 et 20.")]
    [Display(Name = "Histoire-Géographie")]
    public decimal? NoteHistoireGeo { get; set; }

    [Range(0, 20, ErrorMessage = "La note doit être entre 0 et 20.")]
    [Display(Name = "Philosophie")]
    public decimal? NotePhilo { get; set; }

    [Range(0, 20, ErrorMessage = "La note doit être entre 0 et 20.")]
    [Display(Name = "Anglais")]
    public decimal? NoteAnglais { get; set; }
}