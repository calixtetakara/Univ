using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Controllers;

/// <summary>
/// Contrôleur de la page d'accueil (Écran 0).
/// Accessible à tous (visiteurs non authentifiés compris).
/// </summary>
public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Affiche la page d'accueil publique avec les dates d'inscription dynamiques.
    /// </summary>
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var anneeCourante = $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Year + 1}";

        // On récupère les paramètres de calendrier depuis la base
        var parametres = await _context.ParametresCalendrier
            .AsNoTracking()
            .Where(p => p.AnneeAcademique == anneeCourante)
            .ToListAsync();

        var vm = new HomeViewModel();

        // Extraction des dates (si elles existent)
        var dateLicenceDebut = parametres.FirstOrDefault(p => p.Cle == "DATE_DEBUT_INSCRIPTION_LICENCE")?.Valeur;
        var dateLicenceFin = parametres.FirstOrDefault(p => p.Cle == "DATE_FIN_INSCRIPTION_LICENCE")?.Valeur;
        var dateMasterDebut = parametres.FirstOrDefault(p => p.Cle == "DATE_DEBUT_INSCRIPTION_MASTER")?.Valeur;
        var dateMasterFin = parametres.FirstOrDefault(p => p.Cle == "DATE_FIN_INSCRIPTION_MASTER")?.Valeur;

        if (DateTime.TryParse(dateLicenceDebut, out var lDeb)) vm.DateLicenceDebut = lDeb;
        if (DateTime.TryParse(dateLicenceFin, out var lFin)) vm.DateLicenceFin = lFin;
        if (DateTime.TryParse(dateMasterDebut, out var mDeb)) vm.DateMasterDebut = mDeb;
        if (DateTime.TryParse(dateMasterFin, out var mFin)) vm.DateMasterFin = mFin;

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}