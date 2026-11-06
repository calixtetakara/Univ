using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    // -------------------------------------------------------
    // GET: /Account/Login
    // -------------------------------------------------------
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Si l'utilisateur est DÉJÀ connecté, on le redirige vers l'accueil
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home", new { area = "" });

        // Puisque le formulaire est sur la page d'accueil, 
        // on redirige automatiquement vers l'accueil.
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    // -------------------------------------------------------
    // POST: /Account/Login
    // -------------------------------------------------------
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Index", "Home", new { area = "" }); // Retour à l'accueil si erreur de validation

        var user = await _context.Utilisateurs
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        // Utilisateur inexistant ou mot de passe incorrect
        if (user is null || !BCrypt.Net.BCrypt.Verify(model.Password, user.MotDePasseHash))
        {
            TempData["Error"] = "Email ou mot de passe incorrect.";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // Compte non actif
        if (user.Statut != StatutUtilisateur.Actif)
        {
            TempData["Error"] = "Votre compte est désactivé ou suspendu.";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // Création des claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.IdUtilisateur.ToString()),
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.GivenName, $"{user.Prenom} {user.Nom}"),
            new(ClaimTypes.Role, user.Role.Code),
            new("FullName", $"{user.Prenom} {user.Nom}"),
            new("MustChangePassword", user.DoitChangerMotDePasse.ToString().ToLower())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe
                ? DateTimeOffset.UtcNow.AddDays(14)
                : DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);

        // Mise à jour dernière connexion
        user.DerniereConnexion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Forcer le changement de mot de passe si nécessaire
        if (user.DoitChangerMotDePasse)
        {
            if (user.Role.Code == "enseignant")
                return RedirectToAction("Index", "Setup", new { area = "Enseignant" });
            
            return RedirectToAction(nameof(ChangePassword));
        }

        // Si un ReturnUrl est fourni
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        // Redirection selon le rôle
        return user.Role.Code switch
        {
            "superadmin" => RedirectToAction("Index", "Home", new { area = "SuperAdmin" }),
            "scolarite" => RedirectToAction("Index", "Home", new { area = "Scolarite" }),
            "pedagogie" => RedirectToAction("Index", "Home", new { area = "Pedagogie" }),
            "enseignant" => RedirectToAction("Index", "Dashboard", new { area = "Enseignant" }),
            "etudiant" => RedirectToAction("Index", "Dashboard", new { area = "Student" }),
            "directeur" => RedirectToAction("Index", "Dashboard", new { area = "Directeur" }),
            _ => RedirectToAction("Index", "Home", new { area = "" })
        };
    }

    // -------------------------------------------------------
    // GET: /Account/ChangePassword
    // -------------------------------------------------------
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Utilisateurs.FindAsync(userId);

        if (user is null)
            return RedirectToAction("Index", "Home", new { area = "" });

        ViewBag.IsForced = user.DoitChangerMotDePasse;

        return View();
    }

    // -------------------------------------------------------
    // POST: /Account/ChangePassword
    // -------------------------------------------------------
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Utilisateurs.FindAsync(userId);

        if (user is null)
            return RedirectToAction("Index", "Home", new { area = "" });

        if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.MotDePasseHash))
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Mot de passe actuel incorrect.");
            return View(model);
        }

        user.MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        user.DoitChangerMotDePasse = false;
        user.DateModification = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Mot de passe modifié avec succès.";
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    // -------------------------------------------------------
    // POST: /Account/Logout
    // -------------------------------------------------------
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    // -------------------------------------------------------
    // GET: /Account/ForceLogout (MÉTHODE DE SECOURS INFAILLIBLE)
    // -------------------------------------------------------
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ForceLogout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    // -------------------------------------------------------
    // GET: /Account/AccessDenied
    // -------------------------------------------------------
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}