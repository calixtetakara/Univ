namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Jeton de réinitialisation de mot de passe.
/// </summary>
public class PasswordResetToken
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdToken { get; set; }

    /// <summary>Utilisateur concerné.</summary>
    public int IdUtilisateur { get; set; }

    /// <summary>Jeton unique (GUID ou token sécurisé).</summary>
    public string Token { get; set; } = default!;

    /// <summary>Date d'expiration du jeton.</summary>
    public DateTime DateExpiration { get; set; }

    /// <summary>Indique si le jeton a déjà été utilisé.</summary>
    public bool Utilise { get; set; }

    /// <summary>Date de création.</summary>
    public DateTime DateCreation { get; set; }

    /// <summary>Navigation vers l'utilisateur.</summary>
    public Utilisateur Utilisateur { get; set; } = default!;
}