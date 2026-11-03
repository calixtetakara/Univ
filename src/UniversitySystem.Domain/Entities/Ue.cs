namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Unité d'Enseignement (matière / module).
/// </summary>
public class Ue
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdUe { get; set; }

    /// <summary>Code unique de l'UE (ex: "ALG1", "BD2").</summary>
    public string Code { get; set; } = default!;

    /// <summary>Intitulé de l'UE.</summary>
    public string Intitule { get; set; } = default!;

    /// <summary>Nombre de crédits ECTS.</summary>
    public int Credits { get; set; }

    /// <summary>Volume horaire total (heures).</summary>
    public int VolumeHoraire { get; set; }

    /// <summary>Description optionnelle.</summary>
    public string? Description { get; set; }

    /// <summary>Indique si l'UE est active.</summary>
    public bool Actif { get; set; } = true;

    /// <summary>Liaisons avec les filières.</summary>
    public ICollection<FiliereUe> FiliereUes { get; set; } = new List<FiliereUe>();
}