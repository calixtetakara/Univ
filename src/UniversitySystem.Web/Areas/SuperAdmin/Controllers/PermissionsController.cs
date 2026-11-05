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
    // AFFICHAGE DE LA MATRICE (GET)
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
                    // On identifie les permissions critiques pour le verrouillage UI
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

    // =========================================================
    // ENREGISTREMENT DES MODIFICATIONS (POST - M6-03)
    // =========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRoles(Dictionary<string, bool> matrix)
    {
        // Si aucune modification n'est envoyée (cas où tout est décoché)
        if (matrix == null) matrix = new Dictionary<string, bool>();

        // 1. Vérification anti-lockout côté contrôleur (Double sécurité RG-10.1)
        // Empêche de décocher les permissions critiques sur superadmin
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

            // 2. Appliquer les modifications rôle par rôle
            foreach (var role in roles)
            {
                // Filtrer les changements pour ce rôle
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
                    // Appel du service (qui gère l'audit et le anti-lockout backend)
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
    // ONGLET 6.B (PLACEHOLDER POUR M6-04)
    // =========================================================
    [HttpGet]
    public IActionResult Users()
    {
        return View();
    }
}