namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Paramètres du calendrier académique (dates importantes).
/// </summary>
public class ParametreCalendrier
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdParametre { get; set; }

    /// <summary>Clé du paramètre (ex: "DATE_DEBUT_INSCRIPTIONS", "DATE_FIN_S1").</summary>
    public string Cle { get; set; } = default!;

    /// <summary>Valeur du paramètre (date ou texte).</summary>
    public string Valeur { get; set; } = default!;

    /// <summary>Description du paramètre.</summary>
    public string? Description { get; set; }

    /// <summary>Année académique concernée.</summary>
    public string AnneeAcademique { get; set; } = default!;
}