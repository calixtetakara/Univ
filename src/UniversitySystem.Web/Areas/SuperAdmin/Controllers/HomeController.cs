using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Infrastructure.Persistence;

namespace UniversitySystem.Web.Areas.SuperAdmin.Controllers;

[Area("SuperAdmin")]
[Authorize(Roles = "superadmin")]
public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // 1. Compter les étudiants validés
        var totalEtudiants = await _context.Etudiants
            .CountAsync(e => e.StatutDossier == Domain.Enums.StatutInscription.Validee);

        // 2. Compter les enseignants
        var totalEnseignants = await _context.Enseignants.CountAsync();

        // 3. Compter les paiements réussis et calculer le CA
        var totalPaiements = await _context.Paiements
            .Where(p => p.Statut == Domain.Enums.StatutPaiement.Valide)
            .SumAsync(p => p.Montant);

        // 4. Compter les dossiers en attente
        var dossiersEnAttente = await _context.Etudiants
            .CountAsync(e => e.StatutDossier == Domain.Enums.StatutInscription.Brouillon);

        ViewBag.TotalEtudiants = totalEtudiants;
        ViewBag.TotalEnseignants = totalEnseignants;
        ViewBag.TotalPaiements = totalPaiements;
        ViewBag.DossiersEnAttente = dossiersEnAttente;

        return View();
    }
}