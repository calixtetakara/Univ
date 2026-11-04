using System.ComponentModel.DataAnnotations;
using UniversitySystem.Application.Validators;
using UniversitySystem.Domain.Enums; // Pour l'enum Genre

namespace UniversitySystem.Web.Models;

public class Step1IdentiteViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Le numéro de table est obligatoire.")]
    [StringLength(20, MinimumLength = 4)]
    [Display(Name = "Numéro de table BAC")]
    public string NumTableBac { get; set; } = string.Empty;

    [Required(ErrorMessage = "La session du BAC est obligatoire.")]
    [Range(2010, 2100)]
    [Display(Name = "Session du BAC")]
    public int SessionBac { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire.")]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Prénom")]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le genre est obligatoire.")]
    [Display(Name = "Genre")]
    public Genre Genre { get; set; } // Utilisation de ton enum

    [Required(ErrorMessage = "La date de naissance est obligatoire.")]
    [DataType(DataType.Date)]
    [MinimumAge(15)]
    [Display(Name = "Date de naissance")]
    public DateTime? DateNaissance { get; set; }

    [Required(ErrorMessage = "Le lieu de naissance est obligatoire.")]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Lieu de naissance")]
    public string LieuNaissance { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse email est obligatoire.")]
    [EmailAddress(ErrorMessage = "Format d'adresse email invalide.")]
    [Display(Name = "Adresse email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
    [DataType(DataType.Password)]
    [PasswordPolicy]
    [Display(Name = "Mot de passe")]
    public string MotDePasse { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmation est obligatoire.")]
    [DataType(DataType.Password)]
    [Compare(nameof(MotDePasse), ErrorMessage = "Les mots de passe ne correspondent pas.")]
    [Display(Name = "Confirmer le mot de passe")]
    public string ConfirmMotDePasse { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MotDePasse != ConfirmMotDePasse)
        {
            yield return new ValidationResult("Les deux mots de passe ne correspondent pas.", new[] { nameof(ConfirmMotDePasse) });
        }
    }
}