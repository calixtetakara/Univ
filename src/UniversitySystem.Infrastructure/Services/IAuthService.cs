using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Services;

/// <summary>
/// Service d'authentification.
/// Gère la validation des identifiants, le rate limiting par email (RG-12)
/// et l'écriture des logs d'audit liés à la connexion.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Valide l'email et le mot de passe d'un utilisateur.
    /// Applique un rate limiting applicatif (5 tentatives / 15 min).
    /// </summary>
    /// <param name="email">Adresse email saisie.</param>
    /// <param name="password">Mot de passe en clair.</param>
    /// <param name="ipAddress">Adresse IP de la requête (pour l'audit).</param>
    /// <returns>Résultat de la tentative de connexion.</returns>
    Task<LoginResult> ValidateCredentialsAsync(string email, string password, string? ipAddress = null);
}

/// <summary>
/// Résultat retourné par le service d'authentification.
/// </summary>
public class LoginResult
{
    /// <summary>Indique si la connexion a réussi.</summary>
    public bool Success { get; set; }

    /// <summary>Utilisateur authentifié (null en cas d'échec).</summary>
    public Utilisateur? User { get; set; }

    /// <summary>Indique si le compte est temporairement verrouillé (trop de tentatives).</summary>
    public bool IsLockedOut { get; set; }

    /// <summary>Message d'erreur à afficher à l'utilisateur.</summary>
    public string? ErrorMessage { get; set; }
}