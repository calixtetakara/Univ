using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversitySystem.Infrastructure.Services;

namespace UniversitySystem.Web.Areas.Scolarite.Controllers;

/// <summary>
/// Contrôleur d'accueil de l'Area Scolarité.
/// Gère la redirection intelligente vers le premier onglet autorisé.
/// </summary>
[Area("Scolarite")]
[Authorize(Roles = "scolarite,superadmin")]
public class HomeController : Controller
{
    private readonly IPermissionService _permissionService;

    public HomeController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public async Task<IActionResult> Index()
    {
        // Récupération de l'ID de l'agent
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return RedirectToAction("Login", "Account", new { area = "" });

        // Redirection selon la 1ère permission accordée (M3-01)
        if (await _permissionService.HasPermissionAsync(userId, "DOSSIER_LIRE"))
            return RedirectToAction("Index", "Decision");

        if (await _permissionService.HasPermissionAsync(userId, "CONFIG_ACADEMIQUE"))
            return RedirectToAction("Index", "Academic");

        if (await _permissionService.HasPermissionAsync(userId, "ETUDIANTS_LISTER"))
            return RedirectToAction("Index", "Students");

        // Aucun accès : renvoi vers la page d'erreur
        return RedirectToAction("AccessDenied", "Account", new { area = "" });
    }
}