using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversitySystem.Infrastructure.Services;

namespace UniversitySystem.Web.Areas.Pedagogie.Controllers;

[Area("Pedagogie")]
[Authorize(Roles = "pedagogie,superadmin")]
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

        if (await _permissionService.HasPermissionAsync(userId, "ENSEIGNANT_CREER"))
            return RedirectToAction("Index", "Teacher");

        if (await _permissionService.HasPermissionAsync(userId, "PROGRAMMATION_GERER"))
            return RedirectToAction("Index", "Schedule");

        return RedirectToAction("AccessDenied", "Account", new { area = "" });
    }
}