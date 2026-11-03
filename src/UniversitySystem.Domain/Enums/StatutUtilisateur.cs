namespace UniversitySystem.Domain.Enums;

/// <summary>
/// Statut d'un compte utilisateur dans le système.
/// </summary>
public enum StatutUtilisateur
{
    /// <summary>Compte actif, peut se connecter.</summary>
    Actif = 1,

    /// <summary>Compte désactivé manuellement.</summary>
    Inactif = 2,

    /// <summary>Compte temporairement suspendu.</summary>
    Suspendu = 3
}