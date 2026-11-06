using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;

namespace UniversitySystem.Web.Areas.Directeur.Controllers;

[Area("Directeur")]
[Authorize(Roles = "directeur,superadmin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Charger toutes les filières
        var filieres = await _context.Filieres
            .Include(f => f.Faculte)
            .ToListAsync();

        // Calculer les effectifs par filière
        var effectifs = await _context.Inscriptions
            .Where(i => i.Statut == StatutInscription.Validee)
            .GroupBy(i => i.IdFiliere)
            .Select(g => new { IdFiliere = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.IdFiliere, x => x.Count);

        ViewBag.Filieres = filieres;
        ViewBag.Effectifs = effectifs;

        return View();
    }
}