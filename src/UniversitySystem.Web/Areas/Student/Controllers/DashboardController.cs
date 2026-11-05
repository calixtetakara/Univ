using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Infrastructure.Services;

namespace UniversitySystem.Web.Areas.Student.Controllers;

[Area("Student")]
[Authorize(Roles = "etudiant")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPermissionService _permissionService;

    public DashboardController(AppDbContext context, IPermissionService permissionService)
    {
        _context = context;
        _permissionService = permissionService;
    }

    public async Task<IActionResult> Index()
    {
        // 1. Récupérer l'ID de l'utilisateur connecté
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return RedirectToAction("Login", "Account", new { area = "" });

        // 2. Charger l'étudiant avec ses relations
        var etudiant = await _context.Etudiants
            .Include(e => e.Utilisateur)
            .Include(e => e.Filiere)
                .ThenInclude(f => f.Faculte)
            .FirstOrDefaultAsync(e => e.IdUtilisateur == userId);

        if (etudiant == null)
        {
            // L'utilisateur est connecté mais n'a pas de profil étudiant
            return RedirectToAction("AccessDenied", "Account", new { area = "" });
        }

        // 3. Compter le nombre d'inscriptions (pour l'historique)
        var nbInscriptions = await _context.Inscriptions.CountAsync(i => i.IdEtudiant == etudiant.IdEtudiant);

        // 4. Préparer les données pour le Layout
        ViewData["CanSeeEmploiDuTemps"] = (etudiant.StatutDossier == StatutInscription.Validee) || (nbInscriptions > 0);
        ViewData["HasHistorique"] = nbInscriptions > 1;
        ViewData["CanPay"] = etudiant.StatutDossier == StatutInscription.Validee &&
                             await _permissionService.HasPermissionAsync(userId, "PAIEMENT_EFFECTUER");

        // 5. Déterminer la variante
        if (etudiant.StatutDossier == StatutInscription.Brouillon)
        {
            // Variante A : Dossier en cours
            var timeLeft = TimeSpan.Zero;
            var inscription = await _context.Inscriptions
                .FirstOrDefaultAsync(i => i.IdEtudiant == etudiant.IdEtudiant);

            if (inscription != null && inscription.DateSoumission.HasValue)
            {
                var deadline = inscription.DateSoumission.Value.AddHours(48);
                timeLeft = deadline - DateTime.UtcNow;
                if (timeLeft < TimeSpan.Zero) timeLeft = TimeSpan.Zero;
            }

            ViewData["TimeLeft"] = timeLeft;
            return View("VarianteA", etudiant);
        }
        else if (etudiant.StatutDossier == StatutInscription.Validee)
        {
            // Variante B : Dossier validé
            return View("VarianteB", etudiant);
        }
        else if (etudiant.StatutDossier == StatutInscription.Rejetee)
        {
            // Variante C : Dossier rejeté
            return View("VarianteC", etudiant);
        }

        return View("Error");
    }
}