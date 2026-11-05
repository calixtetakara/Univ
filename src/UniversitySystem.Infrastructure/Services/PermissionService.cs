using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Entities;
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
        var user = await _context.Utilisateurs
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdUtilisateur == userId);

        if (user is null) return false;

        return await _context.RolePermissions
            .AsNoTracking()
            .AnyAsync(rp => rp.IdRole == user.IdRole && rp.Permission.Code == permissionCode);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(int userId)
    {
        var user = await _context.Utilisateurs
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdUtilisateur == userId);

        if (user is null) return Array.Empty<string>();

        return await _context.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.IdRole == user.IdRole)
            .Select(rp => rp.Permission.Code)
            .ToListAsync();
    }

    /// <summary>
    /// Met à jour les permissions d'un rôle (M6-03).
    /// Gère l'anti-lockout RG-10.1 et l'audit RG-18.
    /// </summary>
    public async Task UpdateRolePermissionsAsync(int idRole, Dictionary<string, bool> changes, int modifiedBy)
    {
        var role = await _context.Roles.FirstAsync(r => r.IdRole == idRole);
        
        // Récupérer les permissions concernées par les changements
        var permissionCodes = changes.Keys.ToList();
        var permissions = await _context.Permissions
            .Where(p => permissionCodes.Contains(p.Code))
            .ToListAsync();

        // 1. Vérification anti-lockout (Double sécurité backend - RG-10.1)
        if (role.Code == "superadmin")
        {
            var criticalCodes = new[] { "roles.manage", "permissions.view" };
            foreach (var code in criticalCodes)
            {
                // Si on tente de retirer (false) une permission critique
                if (changes.ContainsKey(code) && changes[code] == false)
                {
                    throw new Exception("Impossible de retirer les permissions critiques du SuperAdmin (RG-10).");
                }
            }
        }

        // 2. Application des changements
        foreach (var perm in permissions)
        {
            var newValue = changes[perm.Code];
            var existing = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.IdRole == idRole && rp.IdPermission == perm.IdPermission);

            // On mémorise l'état avant pour l'audit
            bool? before = existing != null;

            // Si pas de changement, on passe
            if ((newValue == true && existing != null) || (newValue == false && existing == null))
                continue;

            // Application
            if (newValue == true && existing == null)
            {
                _context.RolePermissions.Add(new RolePermission { IdRole = idRole, IdPermission = perm.IdPermission });
            }
            else if (newValue == false && existing != null)
            {
                _context.RolePermissions.Remove(existing);
            }

            // 3. Audit (RG-18) - Une ligne par cellule modifiée
            _context.AuditLogs.Add(new AuditLog
            {
                IdUtilisateur = modifiedBy,
                Action = "ROLE_PERMISSION_UPDATED",
                Entite = "Role",
                IdEntite = idRole.ToString(),
                Details = $"{{\"permission\":\"{perm.Code}\", \"before\":{(before == true ? "true" : "null")}, \"after\":{newValue.ToString().ToLower()}}}",
                DateAction = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }
}