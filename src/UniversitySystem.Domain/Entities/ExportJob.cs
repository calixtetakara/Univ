using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Job d'export asynchrone (PDF, Excel, CSV).
/// </summary>
public class ExportJob
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdExportJob { get; set; }

    /// <summary>Utilisateur ayant demandé l'export.</summary>
    public int IdUtilisateur { get; set; }

    /// <summary>Type d'export demandé.</summary>
    public TypeExport Type { get; set; }

    /// <summary>Nom du fichier généré.</summary>
    public string? NomFichier { get; set; }

    /// <summary>Chemin du fichier généré.</summary>
    public string? CheminFichier { get; set; }

    /// <summary>Statut du job (Pending, Processing, Completed, Failed).</summary>
    public string Statut { get; set; } = "Pending";

    /// <summary>Message d'erreur en cas d'échec.</summary>
    public string? MessageErreur { get; set; }

    /// <summary>Date de création du job.</summary>
    public DateTime DateCreation { get; set; }

    /// <summary>Date de fin du traitement.</summary>
    public DateTime? DateFin { get; set; }

    /// <summary>Navigation vers l'utilisateur.</summary>
    public Utilisateur Utilisateur { get; set; } = default!;
}