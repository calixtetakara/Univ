using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UniversitySystem.Web.Models;

/// <summary>
/// ViewModel pour l'étape 3 du wizard : upload des pièces jointes.
/// </summary>
public class Step3DocumentsViewModel
{
    /// <summary>
    /// Relevé de notes du baccalauréat (obligatoire).
    /// </summary>
    [Required(ErrorMessage = "Le relevé de notes est obligatoire.")]
    [Display(Name = "Relevé de notes du BAC (PDF/JPG/PNG)")]
    public IFormFile ReleveBac { get; set; } = default!;

    /// <summary>
    /// Acte de naissance (obligatoire).
    /// </summary>
    [Required(ErrorMessage = "L'acte de naissance est obligatoire.")]
    [Display(Name = "Acte de naissance (PDF/JPG/PNG)")]
    public IFormFile ActeNaissance { get; set; } = default!;
}