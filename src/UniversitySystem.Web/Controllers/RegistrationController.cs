using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySystem.Application.DTOs;
using UniversitySystem.Infrastructure.Services;

namespace UniversitySystem.Web.Controllers;

/// <summary>
/// Contrôleur public gérant le parcours d'inscription (Wizard en 3 étapes).
/// Conforme au SFD Écran 1 — Parcours Nouveau Bachelier.
/// </summary>
[AllowAnonymous] // Accessible aux visiteurs non authentifiés
public class RegistrationController : Controller
{
    private readonly IRegistrationStateStore _stateStore;

    public RegistrationController(IRegistrationStateStore stateStore)
    {
        _stateStore = stateStore;
    }

    // =========================================================
    // ÉTAPE 1 : Identité & création de compte
    // =========================================================
    
    /// <summary>
    /// Affiche le formulaire de l'étape 1 (Identité).
    /// Si un état existe déjà, on le recharge pour ne pas perdre la saisie.
    /// </summary>
    [HttpGet]
    public IActionResult Step1()
    {
        var state = _stateStore.Load(TempData) ?? new RegistrationStateDto();
        return View(state);
    }

    // =========================================================
    // ÉTAPE 2 : Choix parcours + notes
    // =========================================================
    
    /// <summary>
    /// Affiche le formulaire de l'étape 2 (Parcours & Notes).
    /// </summary>
    [HttpGet]
    public IActionResult Step2()
    {
        var state = _stateStore.Load(TempData);

        // GUARD (RG-01) : Impossible d'accéder à l'étape 2 sans avoir validé l'étape 1
        if (state == null || !state.IsStep1Completed)
        {
            return RedirectToAction(nameof(Step1));
        }

        return View(state);
    }

    // =========================================================
    // ÉTAPE 3 : Pièces jointes
    // =========================================================
    
    /// <summary>
    /// Affiche le formulaire de l'étape 3 (Documents).
    /// </summary>
    [HttpGet]
    public IActionResult Step3()
    {
        var state = _stateStore.Load(TempData);

        // GUARD : Impossible d'accéder à l'étape 3 sans avoir validé l'étape 2
        if (state == null || !state.IsStep2Completed)
        {
            return RedirectToAction(nameof(Step2));
        }

        return View(state);
    }

    // =========================================================
    // NAVIGATION
    // =========================================================
    
    /// <summary>
    /// Gère le bouton "Retour" en conservant l'état actuel.
    /// </summary>
    /// <param name="currentStep">L'étape depuis laquelle l'utilisateur clique sur "Retour".</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Back(string currentStep)
    {
        // On ne supprime pas l'état : on redirige simplement vers l'étape précédente.
        return currentStep switch
        {
            "Step2" => RedirectToAction(nameof(Step1)),
            "Step3" => RedirectToAction(nameof(Step2)),
            _ => RedirectToAction(nameof(Step1))
        };
    }
}