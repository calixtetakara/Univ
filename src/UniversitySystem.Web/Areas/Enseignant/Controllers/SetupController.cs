using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Areas.Enseignant.Controllers;

[Area("Enseignant")]
[Authorize(Roles = "enseignant")]
public class SetupController : Controller
{
    private readonly AppDbContext _context;

    public SetupController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Utilisateurs.FindAsync(userId);

        if (user == null) return NotFound();

        // Si déjà configuré, on redirige vers le dashboard
        if (!user.DoitChangerMotDePasse)
            return RedirectToAction("Index", "Dashboard");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Utilisateurs.FindAsync(userId);

        if (user == null) return NotFound();

        // Vérifier que le nouveau mot de passe diffère de l'ancien
        if (BCrypt.Net.BCrypt.Verify(model.NewPassword, user.MotDePasseHash))
        {
            ModelState.AddModelError("", "Le nouveau mot de passe doit être différent du temporaire.");
            return View(model);
        }

        // Mettre à jour le mot de passe
        user.MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        user.DoitChangerMotDePasse = false;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Mot de passe changé avec succès. Bienvenue !";
        return RedirectToAction("Index", "Dashboard");
    }
}