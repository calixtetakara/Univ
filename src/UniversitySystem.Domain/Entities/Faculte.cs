namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Faculté de l'université.
/// </summary>
public class Faculte
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdFaculte { get; set; }

    /// <summary>Code unique de la faculté (ex: "FSI", "FSEG").</summary>
    public string Code { get; set; } = default!;

    /// <summary>Nom complet de la faculté.</summary>
    public string Nom { get; set; } = default!;

    /// <summary>Description optionnelle.</summary>
    public string? Description { get; set; }

    /// <summary>Indique si la faculté est active.</summary>
    public bool Actif { get; set; } = true;

    /// <summary>Filieres rattachées à cette faculté.</summary>
    public ICollection<Filiere> Filieres { get; set; } = new List<Filiere>();
}