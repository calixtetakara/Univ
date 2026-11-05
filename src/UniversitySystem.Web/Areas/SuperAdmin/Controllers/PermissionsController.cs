using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversitySystem.Domain.Entities;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Infrastructure.Services;
using UniversitySystem.Web.Authorization;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Areas.SuperAdmin.Controllers;

[Area("SuperAdmin")]
[Authorize(Roles = "superadmin")]
[HasPermission("PERMISSIONS_GERER")]
public class PermissionsController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPermissionService _permissionService;

    public PermissionsController(AppDbContext context, IPermissionService permissionService)
    {
        _context = context;
        _permissionService = permissionService;
    }

    // =========================================================
    // 6.A : MATRICE DES PERMISSIONS PAR RÔLE
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Roles()
    {
        // 1. Charger les rôles
        var roles = await _context.Roles
            .OrderBy(r => r.Ordre)
            .Select(r => new RoleDto { IdRole = r.IdRole, Code = r.Code, Nom = r.Nom })
            .ToListAsync();

        // 2. Charger les permissions (groupées par module)
        var allPermissions = await _context.Permissions.ToListAsync();
        var permissionsGrouped = allPermissions
            .GroupBy(p => p.Module ?? "Général")
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => new PermissionDto
                {
                    IdPermission = p.IdPermission,
                    Code = p.Code,
                    Nom = p.Nom,
                    Category = p.Module ?? "Général",
                    EstCritique = (p.Code == "roles.manage" || p.Code == "permissions.view")
                }).ToList() as IReadOnlyList<PermissionDto>
            );

        // 3. Charger la matrice actuelle
        var rolePermissions = await _context.RolePermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .ToListAsync();

        var matrix = new Dictionary<string, bool>();
        var lockedCells = new HashSet<string>();

        foreach (var role in roles)
        {
            foreach (var perm in allPermissions)
            {
                var key = $"{role.Code}:{perm.Code}";
                var exists = rolePermissions.Any(rp => rp.IdRole == role.IdRole && rp.IdPermission == perm.IdPermission);
                matrix[key] = exists;

                // Verrouillage des permissions critiques sur SuperAdmin (Anti-Lockout RG-10)
                if (role.Code == "superadmin" && (perm.Code == "roles.manage" || perm.Code == "permissions.view"))
                {
                    lockedCells.Add(key);
                }
            }
        }

        var vm = new PermissionMatrixViewModel
        {
            Roles = roles,
            PermissionsGroupedByCategory = permissionsGrouped,
            Matrix = matrix,
            LockedCells = lockedCells
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRoles(Dictionary<string, bool> matrix)
    {
        if (matrix == null) matrix = new Dictionary<string, bool>();

        // 1. Vérification anti-lockout côté contrôleur (Double sécurité RG-10.1)
        var criticalKeys = new[] { "superadmin:roles.manage", "superadmin:permissions.view" };
        foreach (var key in criticalKeys)
        {
            if (matrix.ContainsKey(key) && matrix[key] == false)
            {
                TempData["Error"] = "Impossible de retirer les permissions critiques du SuperAdmin (Anti-Lockout RG-10).";
                return RedirectToAction(nameof(Roles));
            }
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var roles = await _context.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var changesForRole = new Dictionary<string, bool>();
                foreach (var kvp in matrix)
                {
                    var parts = kvp.Key.Split(':');
                    if (parts[0] == role.Code)
                    {
                        changesForRole[parts[1]] = kvp.Value;
                    }
                }

                if (changesForRole.Any())
                {
                    await _permissionService.UpdateRolePermissionsAsync(role.IdRole, changesForRole, userId);
                }
            }

            TempData["Success"] = "Permissions mises à jour avec succès.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erreur lors de la mise à jour : " + ex.Message;
        }

        return RedirectToAction(nameof(Roles));
    }

    // =========================================================
    // 6.B : OVERRIDES INDIVIDUELS
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Users(string? search)
    {
        // 1. Charger les agents (scolarite et pedagogie, actifs)
        var query = _context.Utilisateurs
            .Include(u => u.Role)
            .Where(u => (u.Role.Code == "scolarite" || u.Role.Code == "pedagogie") && u.Statut == UniversitySystem.Domain.Enums.StatutUtilisateur.Actif);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => u.Email.Contains(search) || u.Nom.Contains(search));
        }

        var users = await query
            .OrderBy(u => u.Nom)
            .ToListAsync();

        ViewBag.Search = search;
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> UserDetails(int id)
    {
        var user = await _context.Utilisateurs
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.IdUtilisateur == id);

        if (user == null) return NotFound();

        // Charger les permissions du rôle
        var rolePermissions = await _context.RolePermissions
            .Where(rp => rp.IdRole == user.IdRole)
            .Select(rp => rp.Permission.Code)
            .ToListAsync();

        // Charger les overrides de l'utilisateur (avec Include pour éviter NullReferenceException)
        var userOverrides = await _context.UtilisateurPermissions
            .Include(up => up.Permission)
            .Where(up => up.IdUtilisateur == id)
            .ToDictionaryAsync(up => up.Permission.Code, up => up.Accorde);

        // Charger toutes les permissions
        var allPermissions = await _context.Permissions.ToListAsync();

        var model = new UserPermissionsViewModel
        {
            IdUtilisateur = user.IdUtilisateur,
            Email = user.Email,
            NomComplet = $"{user.Prenom} {user.Nom}",
            RoleCode = user.Role.Code,
            RoleNom = user.Role.Nom,
            Permissions = allPermissions.Select(p => new PermissionStateDto
            {
                IdPermission = p.IdPermission,
                Code = p.Code,
                Nom = p.Nom,
                Category = p.Module ?? "Général",
                AccordeParRole = rolePermissions.Contains(p.Code),
                Override = userOverrides.ContainsKey(p.Code) ? userOverrides[p.Code] : null
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUserPermissions(int id, Dictionary<string, bool?> changes)
    {
        // Anti-lockout (RG-10.2) : On ne peut pas modifier ses propres overrides
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (id == currentUserId)
        {
            TempData["Error"] = "Vous ne pouvez pas modifier vos propres permissions individuelles (Anti-Lockout RG-10.2).";
            return RedirectToAction(nameof(Users));
        }

        try
        {
            foreach (var kvp in changes)
            {
                var permCode = kvp.Key;
                var value = kvp.Value;

                var permission = await _context.Permissions.FirstAsync(p => p.Code == permCode);
                var existing = await _context.UtilisateurPermissions
                    .FirstOrDefaultAsync(up => up.IdUtilisateur == id && up.IdPermission == permission.IdPermission);

                bool? before = existing?.Accorde;

                if (value == null)
                {
                    // Suppression de l'override (retour au rôle)
                    if (existing != null)
                    {
                        _context.UtilisateurPermissions.Remove(existing);
                        
                        // Audit
                        _context.AuditLogs.Add(new AuditLog
                        {
                            IdUtilisateur = currentUserId,
                            Action = "USER_OVERRIDE_RESET",
                            Entite = "Utilisateur",
                            IdEntite = id.ToString(),
                            Details = $"{{\"permission\":\"{permCode}\", \"before\":{(before == true ? "true" : "false")}, \"after\":null}}",
                            DateAction = DateTime.UtcNow
                        });
                    }
                }
                else
                {
                    // Création ou mise à jour
                    if (existing != null)
                    {
                        existing.Accorde = value.Value;
                    }
                    else
                    {
                        _context.UtilisateurPermissions.Add(new UtilisateurPermission
                        {
                            IdUtilisateur = id,
                            IdPermission = permission.IdPermission,
                            Accorde = value.Value,
                            ModifiePar = currentUserId
                        });
                    }

                    // Audit
                    _context.AuditLogs.Add(new AuditLog
                    {
                        IdUtilisateur = currentUserId,
                        Action = "USER_PERMISSION_UPDATED",
                        Entite = "Utilisateur",
                        IdEntite = id.ToString(),
                        Details = $"{{\"permission\":\"{permCode}\", \"before\":{(before.HasValue ? before.Value.ToString().ToLower() : "null")}, \"after\":{value.Value.ToString().ToLower()}}}",
                        DateAction = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Permissions individuelles mises à jour.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erreur : " + ex.Message;
        }

        return RedirectToAction(nameof(UserDetails), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetUserOverrides(int id)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (id == currentUserId)
        {
            TempData["Error"] = "Vous ne pouvez pas réinitialiser vos propres permissions.";
            return RedirectToAction(nameof(Users));
        }

        var overrides = await _context.UtilisateurPermissions
            .Include(up => up.Permission)
            .Where(up => up.IdUtilisateur == id)
            .ToListAsync();

        foreach (var o in overrides)
        {
            // Audit pour chaque suppression
            _context.AuditLogs.Add(new AuditLog
            {
                IdUtilisateur = currentUserId,
                Action = "USER_OVERRIDE_RESET",
                Entite = "Utilisateur",
                IdEntite = id.ToString(),
                Details = $"{{\"permission\":\"{o.Permission.Code}\", \"before\":{o.Accorde.ToString().ToLower()}, \"after\":null}}",
                DateAction = DateTime.UtcNow
            });
        }

        if (overrides.Any())
        {
            _context.UtilisateurPermissions.RemoveRange(overrides);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Toutes les permissions individuelles ont été réinitialisées.";
        }

        return RedirectToAction(nameof(UserDetails), new { id });
    }
}