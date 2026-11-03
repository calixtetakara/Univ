using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Web.Models;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Le mot de passe actuel est obligatoire.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe actuel")]
    public string CurrentPassword { get; set; } = default!;

    [Required(ErrorMessage = "Le nouveau mot de passe est obligatoire.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nouveau mot de passe")]
    public string NewPassword { get; set; } = default!;

    [Required(ErrorMessage = "La confirmation est obligatoire.")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas.")]
    [Display(Name = "Confirmer le nouveau mot de passe")]
    public string ConfirmPassword { get; set; } = default!;
}