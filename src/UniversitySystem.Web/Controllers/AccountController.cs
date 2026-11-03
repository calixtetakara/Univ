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
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
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
            return View(model);

        var user = await _context.Utilisateurs
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        // Utilisateur inexistant ou mot de passe incorrect
        if (user is null || !BCrypt.Net.BCrypt.Verify(model.Password, user.MotDePasseHash))
        {
            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            return View(model);
        }

        // Compte non actif
        if (user.Statut != StatutUtilisateur.Actif)
        {
            ModelState.AddModelError(string.Empty, "Votre compte est désactivé ou suspendu.");
            return View(model);
        }

        // Création des claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.IdUtilisateur.ToString()),
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.GivenName, $"{user.Prenom} {user.Nom}"),
            new(ClaimTypes.Role, user.Role.Code),
            new("FullName", $"{user.Prenom} {user.Nom}")
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
            return RedirectToAction(nameof(ChangePassword));

        // Redirection
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
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
        return RedirectToAction(nameof(Login));

    // Si l'utilisateur n'est pas obligé de changer son mot de passe,
    // on peut quand même l'autoriser (changement volontaire)
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
        return RedirectToAction(nameof(Login));

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
    return RedirectToAction("Index", "Home");
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
        return RedirectToAction(nameof(Login));
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