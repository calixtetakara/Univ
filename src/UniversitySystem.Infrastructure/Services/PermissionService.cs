using Microsoft.EntityFrameworkCore;
using UniversitySystem.Infrastructure.Persistence;

namespace UniversitySystem.Infrastructure.Services;

/// <summary>
/// Implémentation du service de permissions.
/// S'appuie sur la table role_permissions (RBAC à 2 niveaux).
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly AppDbContext _context;

    public PermissionService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<bool> HasPermissionAsync(int userId, string permissionCode)
    {
        // Récupération du rôle de l'utilisateur
        var user = await _context.Utilisateurs
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdUtilisateur == userId);

        if (user is null)
            return false;

        // Vérification de la présence de la permission dans le rôle
        return await _context.RolePermissions
            .AsNoTracking()
            .AnyAsync(rp =>
                rp.IdRole == user.IdRole &&
                rp.Permission.Code == permissionCode);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(int userId)
    {
        var user = await _context.Utilisateurs
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdUtilisateur == userId);

        if (user is null)
            return Array.Empty<string>();

        return await _context.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.IdRole == user.IdRole)
            .Select(rp => rp.Permission.Code)
            .ToListAsync();
    }
}