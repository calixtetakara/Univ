namespace UniversitySystem.Domain.Entities;

/// <summary>
/// File d'attente des emails à envoyer (traités par Hangfire).
/// </summary>
public class EmailQueue
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdEmail { get; set; }

    /// <summary>Destinataire.</summary>
    public string Destinataire { get; set; } = default!;

    /// <summary>Sujet de l'email.</summary>
    public string Sujet { get; set; } = default!;

    /// <summary>Corps HTML de l'email.</summary>
    public string CorpsHtml { get; set; } = default!;

    /// <summary>Indique si l'email a été envoyé.</summary>
    public bool Envoye { get; set; }

    /// <summary>Date d'envoi effectif.</summary>
    public DateTime? DateEnvoi { get; set; }

    /// <summary>Nombre de tentatives d'envoi.</summary>
    public int Tentatives { get; set; }

    /// <summary>Dernière erreur rencontrée (si échec).</summary>
    public string? DerniereErreur { get; set; }

    /// <summary>Date de création dans la file.</summary>
    public DateTime DateCreation { get; set; }
}