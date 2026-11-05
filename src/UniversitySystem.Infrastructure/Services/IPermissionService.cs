namespace UniversitySystem.Infrastructure.Services;

/// <summary>
/// Contrat du service de permissions.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Vérifie si un utilisateur possède une permission donnée.
    /// </summary>
    Task<bool> HasPermissionAsync(int userId, string permissionCode);

    /// <summary>
    /// Retourne la liste des codes de permissions d'un utilisateur.
    /// </summary>
    Task<IReadOnlyList<string>> GetUserPermissionsAsync(int userId);

    /// <summary>
    /// Met à jour les permissions d'un rôle (M6-03).
    /// </summary>
    /// <param name="idRole">L'ID du rôle à modifier.</param>
    /// <param name="changes">Dictionnaire { codePermission: true/false }.</param>
    /// <param name="modifiedBy">L'ID du SuperAdmin qui effectue la modification.</param>
    Task UpdateRolePermissionsAsync(int idRole, Dictionary<string, bool> changes, int modifiedBy);
}