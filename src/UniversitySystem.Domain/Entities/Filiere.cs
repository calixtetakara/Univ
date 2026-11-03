namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Filière (spécialité) rattachée à une faculté.
/// </summary>
public class Filiere
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdFiliere { get; set; }

    /// <summary>Code unique de la filière (ex: "INFO", "GEST").</summary>
    public string Code { get; set; } = default!;

    /// <summary>Nom de la filière.</summary>
    public string Nom { get; set; } = default!;

    /// <summary>Description optionnelle.</summary>
    public string? Description { get; set; }

    /// <summary>Faculté de rattachement.</summary>
    public int IdFaculte { get; set; }

    /// <summary>Indique si la filière est active.</summary>
    public bool Actif { get; set; } = true;

    /// <summary>Navigation vers la faculté.</summary>
    public Faculte Faculte { get; set; } = default!;

    /// <summary>Unités d'enseignement de la filière.</summary>
    public ICollection<FiliereUe> FiliereUes { get; set; } = new List<FiliereUe>();
}