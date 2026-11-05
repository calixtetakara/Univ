using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Infrastructure.Services;
using UniversitySystem.Web.Authorization;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Areas.Pedagogie.Controllers;

[Area("Pedagogie")]
[Authorize(Roles = "pedagogie,superadmin")]
[HasPermission("PROGRAMMATION_GERER")]
public class ScheduleController : Controller
{
    private readonly AppDbContext _context;
    private readonly IScheduleService _scheduleService;

    public ScheduleController(AppDbContext context, IScheduleService scheduleService)
    {
        _context = context;
        _scheduleService = scheduleService;
    }

    public async Task<IActionResult> Index()
    {
        var programmations = await _context.Programmations
            .Include(p => p.Enseignant).ThenInclude(e => e.Utilisateur)
            .Include(p => p.Ue)
            .Include(p => p.Salle)
            .Include(p => p.Filiere)
            .OrderBy(p => p.JourSemaine).ThenBy(p => p.HeureDebut)
            .ToListAsync();

        return View(programmations);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadViewBags();
        return View(new ScheduleCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ScheduleCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewBags();
            return View(model);
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _scheduleService.CreateAsync(
                model.IdEnseignant,
                model.CodeUe,
                model.IdSalle,
                model.IdFiliere,
                model.JourSemaine,
                model.HeureDebut,
                model.HeureFin,
                model.AnneeAcademique,
                model.Semestre,
                userId);

            TempData["Success"] = "Créneau créé avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (SalleConflitException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadViewBags();
            return View(model);
        }
        catch (EnseignantConflitException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadViewBags();
            return View(model);
        }
        catch (CapaciteDepasseeException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadViewBags();
            return View(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Erreur : " + ex.Message);
            await LoadViewBags();
            return View(model);
        }
    }

    private async Task LoadViewBags()
    {
        ViewBag.Enseignants = await _context.Enseignants.Include(e => e.Utilisateur).ToListAsync();
        ViewBag.Ues = await _context.Ues.ToListAsync();
        ViewBag.Salles = await _context.Salles.ToListAsync();
        ViewBag.Filieres = await _context.Filieres.ToListAsync();
    }
}