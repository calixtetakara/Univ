namespace UniversitySystem.Domain.Enums;

/// <summary>
/// Statut d'un paiement de frais de scolarité.
/// </summary>
public enum StatutPaiement
{
    /// <summary>Paiement initié, en attente de confirmation.</summary>
    EnAttente = 1,

    /// <summary>Paiement validé avec succès.</summary>
    Valide = 2,

    /// <summary>Échec du paiement.</summary>
    Echec = 3,

    /// <summary>Paiement remboursé.</summary>
    Rembourse = 4
}