using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Salle de cours de l'université.
/// </summary>
public class Salle
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdSalle { get; set; }

    /// <summary>Code unique de la salle (ex: "A101", "AMPHI-B").</summary>
    public string Code { get; set; } = default!;

    /// <summary>Nom / libellé de la salle.</summary>
    public string Nom { get; set; } = default!;

    /// <summary>Capacité d'accueil (nombre de places).</summary>
    public int Capacite { get; set; }

    /// <summary>Type de salle.</summary>
    public TypeSalle Type { get; set; }

    /// <summary>Bâtiment ou localisation.</summary>
    public string? Batiment { get; set; }

    /// <summary>Indique si la salle est active et disponible.</summary>
    public bool Actif { get; set; } = true;
}