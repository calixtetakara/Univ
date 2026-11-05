using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversitySystem.Infrastructure.Services;

namespace UniversitySystem.Web.Areas.SuperAdmin.Controllers;

[Area("SuperAdmin")]
[Authorize(Roles = "superadmin")]
public class HomeController : Controller
{
    private readonly IPermissionService _permissionService;

    public HomeController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return RedirectToAction("Login", "Account", new { area = "" });

        // Redirection selon la 1ère permission accordée
        if (await _permissionService.HasPermissionAsync(userId, "PERMISSIONS_GERER"))
            return RedirectToAction("Roles", "Permissions");

        if (await _permissionService.HasPermissionAsync(userId, "FINANCES_LIRE"))
            return RedirectToAction("Index", "Finances");

        if (await _permissionService.HasPermissionAsync(userId, "KPI_LIRE"))
            return RedirectToAction("Index", "Kpi");

        // Aucun accès (théorique)
        return RedirectToAction("AccessDenied", "Account", new { area = "" });
    }
}