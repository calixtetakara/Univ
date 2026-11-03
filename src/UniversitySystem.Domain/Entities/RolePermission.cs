namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Table de liaison many-to-many entre Role et Permission.
/// </summary>
public class RolePermission
{
    /// <summary>Clé étrangère vers Role.</summary>
    public int IdRole { get; set; }

    /// <summary>Clé étrangère vers Permission.</summary>
    public int IdPermission { get; set; }

    /// <summary>Navigation vers le rôle.</summary>
    public Role Role { get; set; } = default!;

    /// <summary>Navigation vers la permission.</summary>
    public Permission Permission { get; set; } = default!;
}