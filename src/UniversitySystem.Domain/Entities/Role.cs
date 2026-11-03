namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Rôle système du RBAC (SFD §2.2).
/// Table : roles (MLD §6.2.1).
/// </summary>
public class Role
{
    /// <summary>Identifiant technique du rôle (PK).</summary>
    public int IdRole { get; set; }

    /// <summary>Code technique unique (ex : "superadmin", "scolarite").</summary>
    public string Code { get; set; } = default!;

    /// <summary>Libellé lisible du rôle.</summary>
    public string Nom { get; set; } = default!;

    /// <summary>Description optionnelle.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indique si le rôle est un rôle système non supprimable (RG-10.3).
    /// </summary>
    public bool EstSysteme { get; set; }

    /// <summary>Ordre d'affichage dans les interfaces.</summary>
    public int Ordre { get; set; }
}