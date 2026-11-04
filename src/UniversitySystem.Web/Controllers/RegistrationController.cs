using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Application.DTOs;
using UniversitySystem.Domain.Entities;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Infrastructure.Services;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Controllers;

[AllowAnonymous]
public class RegistrationController : Controller
{
    private readonly IRegistrationStateStore _stateStore;
    private readonly IEligibilityService _eligibilityService;
    private readonly IFileStorageService _fileStorage;
    private readonly AppDbContext _context;

    public RegistrationController(
        IRegistrationStateStore stateStore,
        IEligibilityService eligibilityService,
        IFileStorageService fileStorage,
        AppDbContext context)
    {
        _stateStore = stateStore;
        _eligibilityService = eligibilityService;
        _fileStorage = fileStorage;
        _context = context;
    }

    // =========================================================
    // ÉTAPE 1
    // =========================================================

    [HttpGet]
    public IActionResult Step1()
    {
        var state = _stateStore.Load() ?? new RegistrationStateDto();
        return View(new Step1IdentiteViewModel
        {
            NumTableBac = state.NumTableBac ?? "",
            SessionBac = state.SessionBac ?? DateTime.UtcNow.Year,
            Nom = state.Nom ?? "",
            Prenom = state.Prenom ?? "",
            Genre = state.Genre ?? Genre.Masculin,
            DateNaissance = state.DateNaissance,
            LieuNaissance = state.LieuNaissance ?? "",
            Email = state.Email ?? ""
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Step1(Step1IdentiteViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _eligibilityService.EmailExistsAsync(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Cet email est déjà utilisé.");
            return View(model);
        }

        if (await _eligibilityService.NumTableBacExistsAsync(model.NumTableBac, model.SessionBac))
        {
            ModelState.AddModelError(nameof(model.NumTableBac), "Ce numéro de table + session est déjà enregistré.");
            return View(model);
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.MotDePasse);
        var state = _stateStore.Load() ?? new RegistrationStateDto();
        state.NumTableBac = model.NumTableBac;
        state.SessionBac = model.SessionBac;
        state.Nom = model.Nom;
        state.Prenom = model.Prenom;
        state.Genre = model.Genre;
        state.DateNaissance = model.DateNaissance;
        state.LieuNaissance = model.LieuNaissance;
        state.Email = model.Email.ToLowerInvariant();
        state.MotDePasseHash = passwordHash;

        _stateStore.Save(state);
        return RedirectToAction(nameof(Step2));
    }

    // =========================================================
    // ÉTAPE 2
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Step2()
    {
        var state = _stateStore.Load();
        if (state == null || !state.IsStep1Completed)
            return RedirectToAction(nameof(Step1));

        ViewBag.Facultes = await _context.Facultes.ToListAsync();
        return View(new Step2ParcoursViewModel
        {
            Niveau = state.Niveau ?? "Licence",
            CodeFaculte = state.CodeFaculte ?? "",
            CodeFiliere = state.CodeFiliere ?? ""
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Step2(Step2ParcoursViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Facultes = await _context.Facultes.ToListAsync();
            return View(model);
        }

        var faculte = await _context.Facultes.FirstOrDefaultAsync(f => f.CodeFaculte == model.CodeFaculte);
        if (faculte == null)
        {
            ModelState.AddModelError("CodeFaculte", "Faculté invalide.");
            ViewBag.Facultes = await _context.Facultes.ToListAsync();
            return View(model);
        }

        var state = _stateStore.Load() ?? new RegistrationStateDto();
        state.Niveau = model.Niveau;
        state.CodeFaculte = model.CodeFaculte;
        state.CodeFiliere = model.CodeFiliere;
        state.NoteMaths = model.NoteMaths;
        state.NoteSvt = model.NoteSvt;
        state.NotePhysique = model.NotePhysique;
        state.NoteFrancais = model.NoteFrancais;
        state.NoteHistoireGeo = model.NoteHistoireGeo;
        state.NotePhilo = model.NotePhilo;
        state.NoteAnglais = model.NoteAnglais;

        _stateStore.Save(state);
        return RedirectToAction(nameof(Step3));
    }

    // =========================================================
    // AJAX
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetFilieres(string codeFaculte)
    {
        var faculte = await _context.Facultes.FirstOrDefaultAsync(f => f.CodeFaculte == codeFaculte);
        if (faculte == null) return BadRequest("Faculté introuvable.");

        var filieres = await _context.Filieres
            .Where(f => f.IdFaculte == faculte.IdFaculte)
            .Select(f => new { f.CodeFiliere, f.NomFiliere })
            .ToListAsync();

        return Json(new { typeFaculte = faculte.TypeFaculte.ToString(), filieres });
    }

    // =========================================================
    // ÉTAPE 3
    // =========================================================

    [HttpGet]
    public IActionResult Step3()
    {
        var state = _stateStore.Load();
        if (state == null || !state.IsStep2Completed)
            return RedirectToAction(nameof(Step2));
        return View(new Step3DocumentsViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Step3(Step3DocumentsViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var state = _stateStore.Load();
        if (state == null || !state.IsStep2Completed) return RedirectToAction(nameof(Step1));

        if (await _eligibilityService.EmailExistsAsync(state.Email!))
        {
            ModelState.AddModelError("", "Cet email vient d'être utilisé. Veuillez recommencer.");
            return View(model);
        }

        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            // 1. Utilisateur
            var roleEtudiant = await _context.Roles.FirstAsync(r => r.Code == "etudiant");
            var utilisateur = new Utilisateur
            {
                Email = state.Email!,
                MotDePasseHash = state.MotDePasseHash!,
                Nom = state.Nom!,
                Prenom = state.Prenom!,
                Statut = StatutUtilisateur.Actif,
                DoitChangerMotDePasse = false,
                DateCreation = DateTime.UtcNow,
                IdRole = roleEtudiant.IdRole
            };
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            // 2. Récupération de la faculté ET de la filière AVANT de créer l'étudiant
            var faculte = await _context.Facultes.FirstAsync(f => f.CodeFaculte == state.CodeFaculte);
            var filiere = await _context.Filieres.FirstAsync(f => f.CodeFiliere == state.CodeFiliere); // <-- AJOUT

            // 3. Étudiant (avec IdFiliere)
            var etudiant = new Etudiant
            {
                IdUtilisateur = utilisateur.IdUtilisateur,
                NumTableBac = state.NumTableBac!,
                SessionBac = state.SessionBac!.Value,
                DateNaissance = state.DateNaissance!.Value,
                LieuNaissance = state.LieuNaissance,
                Genre = state.Genre ?? Genre.Masculin,
                StatutDossier = StatutInscription.Brouillon,
                DateCreation = DateTime.UtcNow,
                Matricule = null, // RG-03
                IdFiliere = filiere.IdFiliere // <-- CORRECTION ICI
            };
            _context.Etudiants.Add(etudiant);
            await _context.SaveChangesAsync();

            // 4. Notes
            var notes = new List<NotesBac>();
            if (faculte.TypeFaculte == TypeFaculte.Scientifique)
            {
                if (state.NoteMaths.HasValue) notes.Add(new NotesBac { IdEtudiant = etudiant.IdEtudiant, Matiere = "Maths", Note = state.NoteMaths.Value, Coefficient = 1 });
                if (state.NoteSvt.HasValue) notes.Add(new NotesBac { IdEtudiant = etudiant.IdEtudiant, Matiere = "SVT", Note = state.NoteSvt.Value, Coefficient = 1 });
                if (state.NotePhysique.HasValue) notes.Add(new NotesBac { IdEtudiant = etudiant.IdEtudiant, Matiere = "Physique", Note = state.NotePhysique.Value, Coefficient = 1 });
            }
            else
            {
                if (state.NoteFrancais.HasValue) notes.Add(new NotesBac { IdEtudiant = etudiant.IdEtudiant, Matiere = "Français", Note = state.NoteFrancais.Value, Coefficient = 1 });
                if (state.NoteHistoireGeo.HasValue) notes.Add(new NotesBac { IdEtudiant = etudiant.IdEtudiant, Matiere = "Histoire-Géo", Note = state.NoteHistoireGeo.Value, Coefficient = 1 });
                if (state.NotePhilo.HasValue) notes.Add(new NotesBac { IdEtudiant = etudiant.IdEtudiant, Matiere = "Philo", Note = state.NotePhilo.Value, Coefficient = 1 });
                if (state.NoteAnglais.HasValue) notes.Add(new NotesBac { IdEtudiant = etudiant.IdEtudiant, Matiere = "Anglais", Note = state.NoteAnglais.Value, Coefficient = 1 });
            }
            _context.NotesBacs.AddRange(notes);
            await _context.SaveChangesAsync();

            // 5. Upload
            var relevePath = await _fileStorage.SaveUploadAsync(model.ReleveBac, $"documents/{etudiant.IdEtudiant}");
            var actePath = await _fileStorage.SaveUploadAsync(model.ActeNaissance, $"documents/{etudiant.IdEtudiant}");

            // 6. Documents
            _context.DocumentsEtudiant.AddRange(
                new DocumentEtudiant { IdEtudiant = etudiant.IdEtudiant, Type = TypeDocument.ReleveNotes, NomFichier = model.ReleveBac.FileName, CheminFichier = relevePath, ContentType = model.ReleveBac.ContentType, TailleOctets = model.ReleveBac.Length, DateUpload = DateTime.UtcNow },
                new DocumentEtudiant { IdEtudiant = etudiant.IdEtudiant, Type = TypeDocument.PieceIdentite, NomFichier = model.ActeNaissance.FileName, CheminFichier = actePath, ContentType = model.ActeNaissance.ContentType, TailleOctets = model.ActeNaissance.Length, DateUpload = DateTime.UtcNow }
            );

            // 7. Inscription
            _context.Inscriptions.Add(new Inscription
            {
                IdEtudiant = etudiant.IdEtudiant,
                IdFiliere = filiere.IdFiliere,
                AnneeAcademique = $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Year + 1}",
                Niveau = state.Niveau == "Licence" ? 1 : 4,
                Statut = StatutInscription.Soumise,
                DateSoumission = DateTime.UtcNow,
                DateCreation = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _stateStore.Clear();
            TempData["RegistrationSuccess"] = true;
            return RedirectToAction(nameof(Success));
        }
        catch (Exception ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : "";
            ModelState.AddModelError("", "Erreur : " + ex.Message + (string.IsNullOrEmpty(innerException) ? "" : " | Détails : " + innerException));
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Success()
    {
        if (TempData["RegistrationSuccess"] == null)
            return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Back(string currentStep)
    {
        return currentStep switch
        {
            "Step2" => RedirectToAction(nameof(Step1)),
            "Step3" => RedirectToAction(nameof(Step2)),
            _ => RedirectToAction(nameof(Step1))
        };
    }
}