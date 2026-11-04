namespace UniversitySystem.Infrastructure.Services;

/// <summary>
/// Service de gestion des permissions RBAC.
/// Version actuelle : permissions basées uniquement sur le rôle (2 niveaux).
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Vérifie si un utilisateur possède une permission donnée via son rôle.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="permissionCode">Code de la permission (ex: "students.manage").</param>
    Task<bool> HasPermissionAsync(int userId, string permissionCode);

    /// <summary>
    /// Retourne la liste de tous les codes de permission accordés à l'utilisateur
    /// via son rôle.
    /// </summary>
    Task<IReadOnlyList<string>> GetUserPermissionsAsync(int userId);
}