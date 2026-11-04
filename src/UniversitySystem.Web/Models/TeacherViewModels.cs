using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Web.Models;

public class TeacherCreateViewModel
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(50, MinimumLength = 2)]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire.")]
    [StringLength(50, MinimumLength = 2)]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est obligatoire.")]
    [EmailAddress(ErrorMessage = "Format d'email invalide.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La spécialité est obligatoire.")]
    [StringLength(100, MinimumLength = 2)]
    public string Specialite { get; set; } = string.Empty;
}

public class TeacherEditViewModel
{
    public int IdEnseignant { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Prenom { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Specialite { get; set; } = string.Empty;

    // Email en lecture seule
    public string Email { get; set; } = string.Empty;
}