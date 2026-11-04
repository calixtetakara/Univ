using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Web.Authorization;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Areas.Scolarite.Controllers;

[Area("Scolarite")]
[Authorize(Roles = "scolarite,superadmin")]
[HasPermission("DOSSIER_LIRE")]
public class DecisionController : Controller
{
    private readonly AppDbContext _context;

    public DecisionController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? searchNom, string? codeFaculte, string? codeFiliere, int page = 1)
    {
        const int pageSize = 25;

        // 1. Construction de la requête de base
        // On cherche les étudiants dont le dossier est "Brouillon" (En cours de traitement)
        var query = _context.Etudiants
            .AsNoTracking()
            .Include(e => e.Utilisateur)
            .Include(e => e.Filiere)
                .ThenInclude(f => f.Faculte)
            .Where(e => e.StatutDossier == StatutInscription.Brouillon);

        // 2. Application des filtres
        if (!string.IsNullOrWhiteSpace(searchNom))
        {
            var nomLower = searchNom.ToLower();
            query = query.Where(e => e.Utilisateur.Nom.ToLower().Contains(nomLower) 
                                  || e.Utilisateur.Prenom.ToLower().Contains(nomLower));
        }

        if (!string.IsNullOrWhiteSpace(codeFaculte))
        {
            query = query.Where(e => e.Filiere.Faculte.CodeFaculte == codeFaculte);
        }

        if (!string.IsNullOrWhiteSpace(codeFiliere))
        {
            query = query.Where(e => e.Filiere.CodeFiliere == codeFiliere);
        }

        // 3. Pagination et tri (par date de création descendante)
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var dossiers = await query
            .OrderByDescending(e => e.DateCreation)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new DossierEnCoursDto
            {
                IdEtudiant = e.IdEtudiant,
                Matricule = e.Matricule,
                Nom = e.Utilisateur.Nom,
                Prenom = e.Utilisateur.Prenom,
                NomFaculte = e.Filiere.Faculte.NomFaculte,
                NomFiliere = e.Filiere.NomFiliere,
                // On récupère la date de soumission de l'inscription ou la date de création
                DateSoumission = _context.Inscriptions
                    .Where(i => i.IdEtudiant == e.IdEtudiant)
                    .Select(i => i.DateSoumission ?? e.DateCreation)
                    .FirstOrDefault()
            })
            .ToListAsync();

        // 4. Préparation du ViewModel
        var vm = new DecisionIndexViewModel
        {
            Dossiers = dossiers,
            SearchNom = searchNom,
            CodeFaculte = codeFaculte,
            CodeFiliere = codeFiliere,
            Page = page,
            TotalPages = totalPages,
            TotalCount = totalCount
        };

        // 5. Chargement des listes pour les filtres (facultés)
        ViewBag.Facultes = await _context.Facultes.ToListAsync();

        return View(vm);
    }
    // Ajoute cette action à la suite de Index()

[HttpGet]
public async Task<IActionResult> Traiter(int id)
{
    // 1. Charger l'étudiant avec ses relations
    var etudiant = await _context.Etudiants
        .Include(e => e.Utilisateur)
        .Include(e => e.Filiere)
            .ThenInclude(f => f.Faculte)
        .FirstOrDefaultAsync(e => e.IdEtudiant == id);

    if (etudiant == null) return NotFound();

    // 2. Vérifier que le dossier est bien en cours
    if (etudiant.StatutDossier != StatutInscription.Brouillon)
    {
        TempData["Error"] = "Ce dossier a déjà été traité.";
        return RedirectToAction(nameof(Index));
    }

    // 3. Charger l'inscription
    var inscription = await _context.Inscriptions
        .FirstOrDefaultAsync(i => i.IdEtudiant == id);

    if (inscription == null) return NotFound("Inscription introuvable.");

    // 4. Charger les notes
    var notes = await _context.NotesBacs
        .Where(n => n.IdEtudiant == id)
        .ToListAsync();

    // 5. Charger les documents
    var documents = await _context.DocumentsEtudiant
        .Where(d => d.IdEtudiant == id)
        .ToListAsync();

    // 6. Préparer le ViewModel
    var vm = new DecisionTraiterViewModel
    {
        Etudiant = etudiant,
        Inscription = inscription,
        Notes = notes,
        Documents = documents,
        TypeFaculte = etudiant.Filiere.Faculte.TypeFaculte.ToString()
    };

    return View(vm);
}
}