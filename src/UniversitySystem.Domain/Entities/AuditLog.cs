namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Journal d'audit des actions importantes du système.
/// </summary>
public class AuditLog
{
    /// <summary>Identifiant technique (PK).</summary>
    public long IdAudit { get; set; }

    /// <summary>Utilisateur ayant effectué l'action (null si système).</summary>
    public int? IdUtilisateur { get; set; }

    /// <summary>Action réalisée (ex: "INSCRIPTION_VALIDEE", "PAIEMENT_RECU").</summary>
    public string Action { get; set; } = default!;

    /// <summary>Entité concernée (ex: "Inscription", "Paiement").</summary>
    public string? Entite { get; set; }

    /// <summary>Identifiant de l'entité concernée.</summary>
    public string? IdEntite { get; set; }

    /// <summary>Détails supplémentaires (JSON ou texte).</summary>
    public string? Details { get; set; }

    /// <summary>Adresse IP de l'utilisateur.</summary>
    public string? AdresseIp { get; set; }

    /// <summary>Date et heure de l'action.</summary>
    public DateTime DateAction { get; set; }
}