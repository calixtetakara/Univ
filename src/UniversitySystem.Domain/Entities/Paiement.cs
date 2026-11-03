using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Paiement de frais de scolarité.
/// </summary>
public class Paiement
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdPaiement { get; set; }

    /// <summary>Inscription concernée.</summary>
    public int IdInscription { get; set; }

    /// <summary>Montant payé.</summary>
    public decimal Montant { get; set; }

    /// <summary>Référence de transaction (fournie par le prestataire de paiement).</summary>
    public string? ReferenceTransaction { get; set; }

    /// <summary>Mode de paiement (ex: "MobileMoney", "Virement", "Espèces").</summary>
    public string ModePaiement { get; set; } = default!;

    /// <summary>Statut du paiement.</summary>
    public StatutPaiement Statut { get; set; } = StatutPaiement.EnAttente;

    /// <summary>Date du paiement.</summary>
    public DateTime DatePaiement { get; set; }

    /// <summary>Date de validation / échec.</summary>
    public DateTime? DateTraitement { get; set; }

    /// <summary>Navigation vers l'inscription.</summary>
    public Inscription Inscription { get; set; } = default!;
}