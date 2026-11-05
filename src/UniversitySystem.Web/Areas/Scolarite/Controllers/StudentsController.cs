using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Export;
using UniversitySystem.Infrastructure.Export.Dtos;
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
    private readonly ICsvGenerator _csvGenerator;
    private readonly IPdfGenerator _pdfGenerator;

    public StudentsController(AppDbContext context, ICsvGenerator csvGenerator, IPdfGenerator pdfGenerator)
    {
        _context = context;
        _csvGenerator = csvGenerator;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<IActionResult> Index(StatutInscription? statutDossier, string? codeFaculte, string? codeFiliere, int page = 1)
    {
        // ... (le code existant pour la liste)
        const int pageSize = 50;
        var query = _context.Etudiants.AsNoTracking()
            .Include(e => e.Utilisateur)
            .Include(e => e.Filiere).ThenInclude(f => f.Faculte)
            .Where(e => e.Filiere != null);

        if (statutDossier.HasValue)
            query = query.Where(e => e.StatutDossier == statutDossier.Value);

        if (!string.IsNullOrWhiteSpace(codeFaculte))
            query = query.Where(e => e.Filiere.Faculte.CodeFaculte == codeFaculte);

        if (!string.IsNullOrWhiteSpace(codeFiliere))
            query = query.Where(e => e.Filiere.CodeFiliere == codeFiliere);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var etudiants = await query.OrderBy(e => e.Utilisateur.Nom)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new EtudiantListDto
            {
                IdEtudiant = e.IdEtudiant,
                Matricule = e.Matricule,
                Nom = e.Utilisateur.Nom,
                Prenom = e.Utilisateur.Prenom,
                NomFaculte = e.Filiere.Faculte.NomFaculte,
                NomFiliere = e.Filiere.NomFiliere,
                Niveau = "Licence",
                StatutDossier = e.StatutDossier
            }).ToListAsync();

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

        ViewBag.Facultes = await _context.Facultes.ToListAsync();
        return View(vm);
    }

    // =========================================================
    // EXPORT (M7-06 / M7-10)
    // =========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Export(StudentExportRequestDto request)
    {
        // 1. Construire la requête avec les filtres
        var query = _context.Etudiants.AsNoTracking()
            .Include(e => e.Utilisateur)
            .Include(e => e.Filiere).ThenInclude(f => f.Faculte)
            .Where(e => e.Filiere != null);

        if (!string.IsNullOrWhiteSpace(request.SearchNom))
        {
            var nomLower = request.SearchNom.ToLower();
            query = query.Where(e => e.Utilisateur.Nom.ToLower().Contains(nomLower) || e.Utilisateur.Prenom.ToLower().Contains(nomLower));
        }

        if (!string.IsNullOrWhiteSpace(request.CodeFaculte))
            query = query.Where(e => e.Filiere.Faculte.CodeFaculte == request.CodeFaculte);

        if (!string.IsNullOrWhiteSpace(request.CodeFiliere))
            query = query.Where(e => e.Filiere.CodeFiliere == request.CodeFiliere);

        // 2. Charger les données
        var etudiants = await query.Select(e => new EtudiantExportDto
        {
            Matricule = e.Matricule ?? "N/A",
            Nom = e.Utilisateur.Nom,
            Prenom = e.Utilisateur.Prenom,
            Faculte = e.Filiere.Faculte.NomFaculte,
            Filiere = e.Filiere.NomFiliere,
            Niveau = "Licence",
            StatutDossier = e.StatutDossier.ToString(),
            StatutPaiement = "Payé" // À calculer avec la table paiements
        }).ToListAsync();

        // 3. Générer le fichier selon le format
        byte[] bytes;
        string contentType;
        string fileName;

        switch (request.Format.ToLower())
        {
            case "pdf":
                bytes = _pdfGenerator.GenerateStudentsPdf(etudiants, User.Identity?.Name ?? "Agent", User.Identity?.Name ?? "");
                contentType = "application/pdf";
                fileName = $"etudiants_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
                break;
            case "csv":
            default:
                bytes = _csvGenerator.GenerateStudentsCsv(etudiants);
                contentType = "text/csv";
                fileName = $"etudiants_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
                break;
        }

        return File(bytes, contentType, fileName);
    }
}