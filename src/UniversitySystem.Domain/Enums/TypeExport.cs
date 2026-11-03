namespace UniversitySystem.Domain.Enums;

/// <summary>
/// Formats d'export supportés par le système.
/// </summary>
public enum TypeExport
{
    /// <summary>Export au format PDF.</summary>
    Pdf = 1,

    /// <summary>Export au format Excel (.xlsx).</summary>
    Excel = 2,

    /// <summary>Export au format CSV.</summary>
    Csv = 3
}