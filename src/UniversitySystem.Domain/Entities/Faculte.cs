using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Faculté de l'université.
/// </summary>
public class Faculte
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdFaculte { get; set; }

    /// <summary>Code unique de la faculté (ex: "FDS", "FASEG").</summary>
    public string CodeFaculte { get; set; } = default!;

    /// <summary>Nom complet de la faculté.</summary>
    public string NomFaculte { get; set; } = default!;

    /// <summary>
    /// Type de faculté (Scientifique ou Littéraire).
    /// Détermine quelles notes du BAC sont demandées.
    /// </summary>
    public TypeFaculte TypeFaculte { get; set; }

    /// <summary>Description optionnelle.</summary>
    public string? Description { get; set; }

    /// <summary>Indique si la faculté est active.</summary>
    public bool Actif { get; set; } = true;

    /// <summary>Filières rattachées à cette faculté.</summary>
    public ICollection<Filiere> Filieres { get; set; } = new List<Filiere>();
}