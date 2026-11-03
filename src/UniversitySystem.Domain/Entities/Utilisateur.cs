using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Compte utilisateur du système (tous les profils : admin, scolarité, enseignant, étudiant...).
/// </summary>
public class Utilisateur
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdUtilisateur { get; set; }

    /// <summary>Adresse email unique (sert de login).</summary>
    public string Email { get; set; } = default!;

    /// <summary>Hash BCrypt du mot de passe.</summary>
    public string MotDePasseHash { get; set; } = default!;

    /// <summary>Nom de famille.</summary>
    public string Nom { get; set; } = default!;

    /// <summary>Prénom.</summary>
    public string Prenom { get; set; } = default!;

    /// <summary>Numéro de téléphone (optionnel).</summary>
    public string? Telephone { get; set; }

    /// <summary>Rôle principal de l'utilisateur.</summary>
    public int IdRole { get; set; }

    /// <summary>Statut du compte (Actif, Inactif, Suspendu).</summary>
    public StatutUtilisateur Statut { get; set; } = StatutUtilisateur.Actif;

    /// <summary>
    /// Indique si l'utilisateur doit changer son mot de passe à la prochaine connexion
    /// (cas du super-admin initial ou d'un compte créé avec mot de passe temporaire).
    /// </summary>
    public bool DoitChangerMotDePasse { get; set; }

    /// <summary>Date et heure de la dernière connexion réussie.</summary>
    public DateTime? DerniereConnexion { get; set; }

    /// <summary>Date de création du compte.</summary>
    public DateTime DateCreation { get; set; }

    /// <summary>Date de dernière modification du compte.</summary>
    public DateTime? DateModification { get; set; }

    /// <summary>Navigation vers le rôle.</summary>
    public Role Role { get; set; } = default!;
}