using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Web.Authorization;

namespace UniversitySystem.Web.Areas.Enseignant.Controllers;

[Area("Enseignant")]
[Authorize(Roles = "enseignant,superadmin")]
[HasPermission("PLANNING_CONSULTER_PERSO")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return RedirectToAction("Login", "Account", new { area = "" });

        var enseignant = await _context.Enseignants
            .FirstOrDefaultAsync(e => e.IdUtilisateur == userId);

        if (enseignant == null) return NotFound("Profil enseignant introuvable.");

        var anneeCourante = $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Year + 1}";

        var programmations = await _context.Programmations
            .Include(p => p.Ue)
            .Include(p => p.Salle)
            .Include(p => p.Filiere)
                .ThenInclude(f => f.Faculte)
            .Where(p => p.IdEnseignant == enseignant.IdEnseignant && p.AnneeAcademique == anneeCourante)
            .OrderBy(p => p.JourSemaine).ThenBy(p => p.HeureDebut)
            .ToListAsync();

        var effectifs = await _context.Inscriptions
            .Where(i => i.AnneeAcademique == anneeCourante && i.Statut == UniversitySystem.Domain.Enums.StatutInscription.Validee)
            .GroupBy(i => i.IdFiliere)
            .Select(g => new { IdFiliere = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.IdFiliere, x => x.Count);

        var volumeHoraireTotal = programmations.Sum(p => (p.HeureFin - p.HeureDebut).TotalHours);

        ViewBag.Programmations = programmations;
        ViewBag.Effectifs = effectifs;
        ViewBag.VolumeHoraireTotal = volumeHoraireTotal;
        ViewBag.Annee = anneeCourante;

        return View();
    }
}