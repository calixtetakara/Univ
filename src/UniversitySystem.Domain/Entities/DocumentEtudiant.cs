using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Document joint au dossier d'un étudiant.
/// </summary>
public class DocumentEtudiant
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdDocument { get; set; }

    /// <summary>Étudiant propriétaire du document.</summary>
    public int IdEtudiant { get; set; }

    /// <summary>Type de document.</summary>
    public TypeDocument Type { get; set; }

    /// <summary>Nom original du fichier.</summary>
    public string NomFichier { get; set; } = default!;

    /// <summary>Chemin de stockage relatif du fichier.</summary>
    public string CheminFichier { get; set; } = default!;

    /// <summary>Taille du fichier en octets.</summary>
    public long TailleOctets { get; set; }

    /// <summary>Type MIME du fichier.</summary>
    public string ContentType { get; set; } = default!;

    /// <summary>Date d'upload.</summary>
    public DateTime DateUpload { get; set; }

    /// <summary>Navigation vers l'étudiant.</summary>
    public Etudiant Etudiant { get; set; } = default!;
}