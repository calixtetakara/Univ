using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySystem.Infrastructure.Services;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Controllers;

[AllowAnonymous]
[Route("/forgot-password")] // <-- ROUTE EXPLICITE
public class ForgotPasswordController : Controller
{
    private readonly IPasswordResetService _resetService;

    public ForgotPasswordController(IPasswordResetService resetService)
    {
        _resetService = resetService;
    }

    [HttpGet("")] // Racine : /forgot-password
    public IActionResult Index()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _resetService.RequestResetAsync(model.Email);

        TempData["Success"] = "Si l'adresse existe, un email a été envoyé.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("reset")] // URL : /forgot-password/reset
    public IActionResult Reset(string? token)
    {
        if (string.IsNullOrEmpty(token))
            return RedirectToAction(nameof(Index));

        return View(new ResetPasswordViewModel { Token = token });
    }

    [HttpPost("reset")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reset(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _resetService.ResetAsync(model.Token, model.NewPassword);
            TempData["Success"] = "Votre mot de passe a été réinitialisé. Vous pouvez vous connecter.";
            return RedirectToAction("Login", "Account");
        }
        catch (InvalidTokenException)
        {
            ModelState.AddModelError("", "Lien invalide ou expiré.");
            return View(model);
        }
    }
}