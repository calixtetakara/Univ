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
[HasPermission("ETUDIANTS_LISTER")]
public class StudentsController : Controller
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(StatutInscription? statutDossier, string? codeFaculte, string? codeFiliere, int page = 1)
    {
        const int pageSize = 50;

        // 1. Requête de base (jointure Etudiants -> Inscriptions -> Filieres -> Facultes)
        var query = _context.Etudiants
            .AsNoTracking()
            .Include(e => e.Utilisateur)
            .Include(e => e.Filiere)
                .ThenInclude(f => f.Faculte)
            .Where(e => e.Filiere != null); // On ne garde que ceux qui ont une filière

        // 2. Application des filtres
        if (statutDossier.HasValue)
            query = query.Where(e => e.StatutDossier == statutDossier.Value);

        if (!string.IsNullOrWhiteSpace(codeFaculte))
            query = query.Where(e => e.Filiere.Faculte.CodeFaculte == codeFaculte);

        if (!string.IsNullOrWhiteSpace(codeFiliere))
            query = query.Where(e => e.Filiere.CodeFiliere == codeFiliere);

        // 3. Pagination
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var etudiants = await query
            .OrderBy(e => e.Utilisateur.Nom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EtudiantListDto
            {
                IdEtudiant = e.IdEtudiant,
                Matricule = e.Matricule,
                Nom = e.Utilisateur.Nom,
                Prenom = e.Utilisateur.Prenom,
                NomFaculte = e.Filiere.Faculte.NomFaculte,
                NomFiliere = e.Filiere.NomFiliere,
                Niveau = "Licence", // À adapter si tu as un champ Niveau
                StatutDossier = e.StatutDossier
            })
            .ToListAsync();

        // 4. Préparation du ViewModel
        var vm = new StudentIndexViewModel
        {
            Etudiants = etudiants,
            StatutDossier = statutDossier,
            CodeFaculte = codeFaculte,
            CodeFiliere = codeFiliere,
            Page = page,
            TotalPages = totalPages,
            TotalCount = totalCount
        };

        // 5. Chargement des listes pour les filtres
        ViewBag.Facultes = await _context.Facultes.ToListAsync();
        
        return View(vm);
    }
}