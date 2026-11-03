namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Table de liaison Filière ↔ UE (avec semestre et caractère obligatoire).
/// </summary>
public class FiliereUe
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdFiliereUe { get; set; }

    /// <summary>Filière concernée.</summary>
    public int IdFiliere { get; set; }

    /// <summary>UE concernée.</summary>
    public int IdUe { get; set; }

    /// <summary>Semestre dans lequel l'UE est enseignée (1 à 6 par exemple).</summary>
    public int Semestre { get; set; }

    /// <summary>Indique si l'UE est obligatoire dans cette filière.</summary>
    public bool Obligatoire { get; set; } = true;

    /// <summary>Navigation vers la filière.</summary>
    public Filiere Filiere { get; set; } = default!;

    /// <summary>Navigation vers l'UE.</summary>
    public Ue Ue { get; set; } = default!;
}