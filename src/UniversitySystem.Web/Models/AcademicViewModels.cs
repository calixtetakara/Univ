using System.ComponentModel.DataAnnotations;
using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Web.Models;

// =========================================================
// Faculté
// =========================================================
public class FaculteEditViewModel
{
    [Required(ErrorMessage = "Le code est obligatoire.")]
    [StringLength(10, MinimumLength = 2)]
    [RegularExpression("^[A-Z0-9]{2,10}$", ErrorMessage = "Le code doit contenir uniquement des majuscules et des chiffres (2-10 caractères).")]
    [Display(Name = "Code de la faculté")]
    public string CodeFaculte { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(150)]
    [Display(Name = "Nom de la faculté")]
    public string NomFaculte { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le type est obligatoire.")]
    [Display(Name = "Type de faculté")]
    public TypeFaculte TypeFaculte { get; set; }

    [Display(Name = "Description")]
    [StringLength(500)]
    public string? Description { get; set; }

    // Indique si le type peut être modifié (false si des inscriptions existent)
    public bool CanEditType { get; set; } = true;
}

// =========================================================
// Filière
// =========================================================
public class FiliereEditViewModel
{
    [Required(ErrorMessage = "Le code est obligatoire.")]
    [StringLength(20, MinimumLength = 2)]
    [RegularExpression("^[A-Z0-9_]{2,20}$", ErrorMessage = "Le code doit contenir uniquement des majuscules, chiffres et underscores.")]
    [Display(Name = "Code de la filière")]
    public string CodeFiliere { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(150)]
    [Display(Name = "Nom de la filière")]
    public string NomFiliere { get; set; } = string.Empty;

    [Required(ErrorMessage = "La faculté est obligatoire.")]
    [Display(Name = "Faculté")]
    public string CodeFaculte { get; set; } = string.Empty;
}

// =========================================================
// UE
// =========================================================
public class UeEditViewModel
{
    [Required(ErrorMessage = "Le code est obligatoire.")]
    [StringLength(20, MinimumLength = 2)]
    [RegularExpression("^[A-Z0-9_]{2,20}$", ErrorMessage = "Le code doit contenir uniquement des majuscules, chiffres et underscores.")]
    [Display(Name = "Code de l'UE")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'intitulé est obligatoire.")]
    [StringLength(200)]
    [Display(Name = "Intitulé de l'UE")]
    public string Intitule { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nombre de crédits est obligatoire.")]
    [Range(1, 60, ErrorMessage = "Entre 1 et 60 crédits.")]
    [Display(Name = "Crédits ECTS")]
    public int Credits { get; set; }

    [Required]
    [Display(Name = "Volume horaire (heures)")]
    public int VolumeHoraire { get; set; }

    [Display(Name = "Description")]
    [StringLength(500)]
    public string? Description { get; set; }
}

// =========================================================
// Affectation Filière-UE
// =========================================================
public class FiliereUeAffectationViewModel
{
    public string CodeFiliere { get; set; } = string.Empty;
    public string NomFiliere { get; set; } = string.Empty;
    public List<UeCheckboxItem> AvailableUes { get; set; } = new List<UeCheckboxItem>();
}

public class UeCheckboxItem
{
    public string Code { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public int Credits { get; set; }
    public bool IsChecked { get; set; }
}