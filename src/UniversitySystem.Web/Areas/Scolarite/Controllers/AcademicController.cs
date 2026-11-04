using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Entities;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Web.Authorization;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Areas.Scolarite.Controllers;

[Area("Scolarite")]
[Authorize(Roles = "scolarite,superadmin")]
[HasPermission("CONFIG_ACADEMIQUE")]
public class AcademicController : Controller
{
    private readonly AppDbContext _context;

    public AcademicController(AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // PAGE D'ACCUEIL (Redirection vers la liste des facultés)
    // =========================================================
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Facultes));
    }

    // =========================================================
    // CRUD FACULTÉS (M3-08)
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Facultes()
    {
        var facultes = await _context.Facultes.ToListAsync();
        return View(facultes);
    }

    [HttpGet]
    public IActionResult CreateFaculte()
    {
        return View(new FaculteEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFaculte(FaculteEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _context.Facultes.AnyAsync(f => f.CodeFaculte == model.CodeFaculte))
        {
            ModelState.AddModelError(nameof(model.CodeFaculte), "Ce code existe déjà.");
            return View(model);
        }

        var faculte = new Faculte
        {
            CodeFaculte = model.CodeFaculte,
            NomFaculte = model.NomFaculte,
            TypeFaculte = model.TypeFaculte,
            Description = model.Description
        };

        _context.Facultes.Add(faculte);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"La faculté {faculte.CodeFaculte} a été créée.";
        return RedirectToAction(nameof(Facultes));
    }

    [HttpGet]
    public async Task<IActionResult> EditFaculte(string id)
    {
        var faculte = await _context.Facultes.FirstOrDefaultAsync(f => f.CodeFaculte == id);
        if (faculte == null) return NotFound();

        // Q6 : Vérifier si des inscriptions existent pour cette faculté
        var hasInscriptions = await _context.Inscriptions
            .AnyAsync(i => i.Filiere.Faculte.CodeFaculte == id);

        var model = new FaculteEditViewModel
        {
            CodeFaculte = faculte.CodeFaculte,
            NomFaculte = faculte.NomFaculte,
            TypeFaculte = faculte.TypeFaculte,
            Description = faculte.Description,
            CanEditType = !hasInscriptions
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditFaculte(string id, FaculteEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var faculte = await _context.Facultes.FirstOrDefaultAsync(f => f.CodeFaculte == id);
        if (faculte == null) return NotFound();

        // Vérification backend de la règle Q6
        var hasInscriptions = await _context.Inscriptions
            .AnyAsync(i => i.Filiere.Faculte.CodeFaculte == id);

        if (hasInscriptions && faculte.TypeFaculte != model.TypeFaculte)
        {
            ModelState.AddModelError(nameof(model.TypeFaculte), "Impossible de modifier le type : des étudiants y sont inscrits.");
            model.CanEditType = false;
            return View(model);
        }

        faculte.NomFaculte = model.NomFaculte;
        faculte.Description = model.Description;
        
        if (!hasInscriptions)
            faculte.TypeFaculte = model.TypeFaculte;

        await _context.SaveChangesAsync();

        TempData["Success"] = "La faculté a été modifiée.";
        return RedirectToAction(nameof(Facultes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFaculte(string id)
    {
        // Include permet de charger les filières associées
        var faculte = await _context.Facultes
            .Include(f => f.Filieres)
            .FirstOrDefaultAsync(f => f.CodeFaculte == id);

        if (faculte == null) return NotFound();

        // Interdire la suppression si des filières existent
        if (faculte.Filieres.Any())
        {
            TempData["Error"] = "Impossible de supprimer cette faculté : elle contient des filières.";
            return RedirectToAction(nameof(Facultes));
        }

        _context.Facultes.Remove(faculte);
        await _context.SaveChangesAsync();

        TempData["Success"] = "La faculté a été supprimée.";
        return RedirectToAction(nameof(Facultes));
    }
        // =========================================================
    // CRUD FILIÈRES (M3-09)
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Filieres()
    {
        var filieres = await _context.Filieres
            .Include(f => f.Faculte)
            .ToListAsync();
        return View(filieres);
    }

    [HttpGet]
    public async Task<IActionResult> CreateFiliere()
    {
        ViewBag.Facultes = await _context.Facultes.ToListAsync();
        return View(new FiliereEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFiliere(FiliereEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Facultes = await _context.Facultes.ToListAsync();
            return View(model);
        }

        if (await _context.Filieres.AnyAsync(f => f.CodeFiliere == model.CodeFiliere))
        {
            ModelState.AddModelError(nameof(model.CodeFiliere), "Ce code existe déjà.");
            ViewBag.Facultes = await _context.Facultes.ToListAsync();
            return View(model);
        }

        var faculte = await _context.Facultes.FirstOrDefaultAsync(f => f.CodeFaculte == model.CodeFaculte);
        if (faculte == null)
        {
            ModelState.AddModelError(nameof(model.CodeFaculte), "Faculté invalide.");
            ViewBag.Facultes = await _context.Facultes.ToListAsync();
            return View(model);
        }

        var filiere = new Filiere
        {
            CodeFiliere = model.CodeFiliere,
            NomFiliere = model.NomFiliere,
            IdFaculte = faculte.IdFaculte
        };

        _context.Filieres.Add(filiere);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"La filière {filiere.CodeFiliere} a été créée.";
        return RedirectToAction(nameof(Filieres));
    }

    [HttpGet]
    public async Task<IActionResult> EditFiliere(string id)
    {
        var filiere = await _context.Filieres
            .Include(f => f.Faculte)
            .FirstOrDefaultAsync(f => f.CodeFiliere == id);

        if (filiere == null) return NotFound();

        ViewBag.Facultes = await _context.Facultes.ToListAsync();
        var model = new FiliereEditViewModel
        {
            CodeFiliere = filiere.CodeFiliere,
            NomFiliere = filiere.NomFiliere,
            CodeFaculte = filiere.Faculte.CodeFaculte
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditFiliere(string id, FiliereEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Facultes = await _context.Facultes.ToListAsync();
            return View(model);
        }

        var filiere = await _context.Filieres.FirstOrDefaultAsync(f => f.CodeFiliere == id);
        if (filiere == null) return NotFound();

        var faculte = await _context.Facultes.FirstOrDefaultAsync(f => f.CodeFaculte == model.CodeFaculte);
        if (faculte == null)
        {
            ModelState.AddModelError(nameof(model.CodeFaculte), "Faculté invalide.");
            ViewBag.Facultes = await _context.Facultes.ToListAsync();
            return View(model);
        }

        filiere.NomFiliere = model.NomFiliere;
        filiere.IdFaculte = faculte.IdFaculte;

        await _context.SaveChangesAsync();

        TempData["Success"] = "La filière a été modifiée.";
        return RedirectToAction(nameof(Filieres));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFiliere(string id)
    {
        var filiere = await _context.Filieres.FirstOrDefaultAsync(f => f.CodeFiliere == id);
        if (filiere == null) return NotFound();

        // Vérifier s'il y a des inscriptions
        var hasInscriptions = await _context.Inscriptions.AnyAsync(i => i.IdFiliere == filiere.IdFiliere);
        if (hasInscriptions)
        {
            TempData["Error"] = "Impossible de supprimer cette filière : des étudiants y sont inscrits.";
            return RedirectToAction(nameof(Filieres));
        }

        _context.Filieres.Remove(filiere);
        await _context.SaveChangesAsync();

        TempData["Success"] = "La filière a été supprimée.";
        return RedirectToAction(nameof(Filieres));
    }
        // =========================================================
    // CRUD UE (M3-09)
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Ues()
    {
        var ues = await _context.Ues.ToListAsync();
        return View(ues);
    }

    [HttpGet]
    public IActionResult CreateUe()
    {
        return View(new UeEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUe(UeEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _context.Ues.AnyAsync(u => u.Code == model.Code))
        {
            ModelState.AddModelError(nameof(model.Code), "Ce code existe déjà.");
            return View(model);
        }

        var ue = new Ue
        {
            Code = model.Code,
            Intitule = model.Intitule,
            Credits = model.Credits,
            VolumeHoraire = model.VolumeHoraire,
            Description = model.Description
        };

        _context.Ues.Add(ue);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"L'UE {ue.Code} a été créée.";
        return RedirectToAction(nameof(Ues));
    }

    [HttpGet]
    public async Task<IActionResult> EditUe(string id)
    {
        var ue = await _context.Ues.FirstOrDefaultAsync(u => u.Code == id);
        if (ue == null) return NotFound();

        var model = new UeEditViewModel
        {
            Code = ue.Code,
            Intitule = ue.Intitule,
            Credits = ue.Credits,
            VolumeHoraire = ue.VolumeHoraire,
            Description = ue.Description
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUe(string id, UeEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var ue = await _context.Ues.FirstOrDefaultAsync(u => u.Code == id);
        if (ue == null) return NotFound();

        ue.Intitule = model.Intitule;
        ue.Credits = model.Credits;
        ue.VolumeHoraire = model.VolumeHoraire;
        ue.Description = model.Description;

        await _context.SaveChangesAsync();

        TempData["Success"] = "L'UE a été modifiée.";
        return RedirectToAction(nameof(Ues));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUe(string id)
    {
        var ue = await _context.Ues.FirstOrDefaultAsync(u => u.Code == id);
        if (ue == null) return NotFound();

        // Vérifier si l'UE est affectée à une filière
        var isAffected = await _context.FiliereUes.AnyAsync(fu => fu.Ue.Code == id);
        if (isAffected)
        {
            TempData["Error"] = "Impossible de supprimer cette UE : elle est affectée à une filière.";
            return RedirectToAction(nameof(Ues));
        }

        _context.Ues.Remove(ue);
        await _context.SaveChangesAsync();

        TempData["Success"] = "L'UE a été supprimée.";
        return RedirectToAction(nameof(Ues));
    }

    // =========================================================
    // AFFECTATION UE ↔ FILIÈRE
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> AffecterUe(string codeFiliere)
    {
        var filiere = await _context.Filieres
            .Include(f => f.FiliereUes)
            .FirstOrDefaultAsync(f => f.CodeFiliere == codeFiliere);

        if (filiere == null) return NotFound();

        var allUes = await _context.Ues.ToListAsync();
        var affectedUeIds = filiere.FiliereUes.Select(fu => fu.Ue.IdUe).ToList();

        var model = new FiliereUeAffectationViewModel
        {
            CodeFiliere = filiere.CodeFiliere,
            NomFiliere = filiere.NomFiliere,
            AvailableUes = allUes.Select(u => new UeCheckboxItem
            {
                Code = u.Code,
                Intitule = u.Intitule,
                Credits = u.Credits,
                IsChecked = affectedUeIds.Contains(u.IdUe)
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AffecterUe(FiliereUeAffectationViewModel model)
    {
        var filiere = await _context.Filieres
            .Include(f => f.FiliereUes)
            .FirstOrDefaultAsync(f => f.CodeFiliere == model.CodeFiliere);

        if (filiere == null) return NotFound();

        // 1. Supprimer les affectations existantes
        _context.FiliereUes.RemoveRange(filiere.FiliereUes);

        // 2. Ajouter les nouvelles affectations
        var selectedUeCodes = model.AvailableUes.Where(x => x.IsChecked).Select(x => x.Code).ToList();
        var uesToAdd = await _context.Ues.Where(u => selectedUeCodes.Contains(u.Code)).ToListAsync();

        foreach (var ue in uesToAdd)
        {
            _context.FiliereUes.Add(new FiliereUe
            {
                IdFiliere = filiere.IdFiliere,
                IdUe = ue.IdUe
            });
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Les UE ont été affectées à la filière.";
        return RedirectToAction(nameof(Filieres));
    }
}