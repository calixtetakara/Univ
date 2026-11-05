using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Web.Authorization;

namespace UniversitySystem.Web.Areas.SuperAdmin.Controllers;

[Area("SuperAdmin")]
[Authorize(Roles = "superadmin")]
[HasPermission("KPI_LIRE")]
public class KpiController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    public KpiController(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // =========================================================
    // VUE PRINCIPALE (M6-08)
    // =========================================================
    public async Task<IActionResult> Index(string? annee)
    {
        var anneeCourante = annee ?? $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Year + 1}";
        ViewBag.Annee = anneeCourante;

        // Effectifs par faculté (pour le graphique)
        var effectifs = await _context.Inscriptions
            .Include(i => i.Filiere)
                .ThenInclude(f => f.Faculte)
            .Where(i => i.AnneeAcademique == anneeCourante && i.Statut == StatutInscription.Validee)
            .GroupBy(i => i.Filiere.Faculte.NomFaculte)
            .Select(g => new
            {
                Faculte = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        ViewBag.Labels = effectifs.Select(e => e.Faculte).ToList();
        ViewBag.Values = effectifs.Select(e => e.Count).ToList();

        return View();
    }

    // =========================================================
    // ENDPOINT JSON POUR LE COMPTEUR CA (M6-09)
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> GetCa()
    {
        const string cacheKey = "kpi_ca_total";
        var total = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            // Le cache expire après 30 secondes
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);

            return await _context.Paiements
                .Where(p => p.Statut == StatutPaiement.Valide)
                .SumAsync(p => p.Montant);
        });

        return Json(new { total, devise = "FCFA", timestamp = DateTime.UtcNow });
    }
}