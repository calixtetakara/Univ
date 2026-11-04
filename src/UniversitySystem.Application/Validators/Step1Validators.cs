using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Application.Validators;

/// <summary>
/// Valide que l'utilisateur a au moins l'âge minimum requis (15 ans).
/// Règle métier SFD.
/// </summary>
public class MinimumAgeAttribute : ValidationAttribute
{
    private readonly int _minimumAge;

    public MinimumAgeAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
        ErrorMessage = $"Vous devez avoir au moins {minimumAge} ans pour vous inscrire.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime dateNaissance)
            return new ValidationResult("Date de naissance invalide.");

        var age = DateTime.UtcNow.Year - dateNaissance.Year;
        if (dateNaissance.Date > DateTime.UtcNow.AddYears(-age)) age--;

        return age >= _minimumAge 
            ? ValidationResult.Success 
            : new ValidationResult(ErrorMessage);
    }
}

/// <summary>
/// Valide la politique de mot de passe (RG-13) :
/// Minimum 8 caractères, 1 majuscule, 1 chiffre, 1 caractère spécial.
/// </summary>
public class PasswordPolicyAttribute : ValidationAttribute
{
    public PasswordPolicyAttribute()
    {
        ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères, une majuscule, un chiffre et un caractère spécial.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string password || string.IsNullOrWhiteSpace(password))
            return new ValidationResult(ErrorMessage);

        bool hasUpper = password.Any(char.IsUpper);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

        return password.Length >= 8 && hasUpper && hasDigit && hasSpecial
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage);
    }
}