using System.Collections.Generic;

namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Unité d'Enseignement (UE).
/// </summary>
public class Ue
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdUe { get; set; }

    /// <summary>Code unique de l'UE (ex: "INF101").</summary>
    public string Code { get; set; } = default!;

    /// <summary>Intitulé complet de l'UE.</summary>
    public string Intitule { get; set; } = default!;

    /// <summary>Nombre de crédits ECTS.</summary>
    public int Credits { get; set; }

    /// <summary>Volume horaire total (en heures).</summary>
    public int VolumeHoraire { get; set; }

    /// <summary>Description optionnelle.</summary>
    public string? Description { get; set; }

    /// <summary>Indique si l'UE est active.</summary>
    public bool Actif { get; set; } = true;

    /// <summary>Navigation vers les associations Filière-UE.</summary>
    public ICollection<FiliereUe> FiliereUes { get; set; } = new List<FiliereUe>();
}