using System.Collections.Generic;

namespace UniversitySystem.Web.Models;

public class UserPermissionsViewModel
{
    public int IdUtilisateur { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public string RoleNom { get; set; } = string.Empty;

    /// <summary>Liste des permissions avec leur état.</summary>
    public List<PermissionStateDto> Permissions { get; set; } = new List<PermissionStateDto>();
}

public class PermissionStateDto
{
    public int IdPermission { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Category { get; set; } = "Général";

    /// <summary>True si le rôle possède cette permission par défaut.</summary>
    public bool AccordeParRole { get; set; }

    /// <summary>null = pas d'override, true = accordé, false = refusé.</summary>
    public bool? Override { get; set; }

    /// <summary>La valeur effective (Override si présent, sinon AccordeParRole).</summary>
    public bool Effective => Override ?? AccordeParRole;
}