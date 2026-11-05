using System.Collections.Generic;

namespace UniversitySystem.Web.Models;

public class PermissionMatrixViewModel
{
    /// <summary>La liste des rôles (colonnes).</summary>
    public IReadOnlyList<RoleDto> Roles { get; set; } = new List<RoleDto>();

    /// <summary>Les permissions groupées par catégorie (lignes).</summary>
    public IReadOnlyDictionary<string, IReadOnlyList<PermissionDto>> PermissionsGroupedByCategory { get; set; } = new Dictionary<string, IReadOnlyList<PermissionDto>>();

    /// <summary>La matrice des cases cochées : clé = "{roleCode}:{permissionCode}" -> bool.</summary>
    public Dictionary<string, bool> Matrix { get; set; } = new Dictionary<string, bool>();

    /// <summary>Les cellules verrouillées (critiques sur SuperAdmin).</summary>
    public HashSet<string> LockedCells { get; set; } = new HashSet<string>();
}

public class RoleDto
{
    public int IdRole { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
}

public class PermissionDto
{
    public int IdPermission { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Category { get; set; } = "Général";
    public bool EstCritique { get; set; }
}