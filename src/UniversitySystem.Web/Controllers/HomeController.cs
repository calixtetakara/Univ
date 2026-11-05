using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        // 1. Si l'utilisateur est connecté, on le redirige vers son espace
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("superadmin"))
                return RedirectToAction("Index", "Home", new { area = "SuperAdmin" });

            if (User.IsInRole("scolarite"))
                return RedirectToAction("Index", "Home", new { area = "Scolarite" });

            if (User.IsInRole("pedagogie"))
                return RedirectToAction("Index", "Home", new { area = "Pedagogie" });

            if (User.IsInRole("enseignant"))
                return RedirectToAction("Index", "Dashboard", new { area = "Enseignant" });

            // Si c'est un étudiant, on le redirige vers son dashboard
            if (User.IsInRole("etudiant"))
                return RedirectToAction("Index", "Dashboard", new { area = "Student" });
        }

        // 2. Si non connecté, on affiche la page d'accueil publique
        var anneeCourante = $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Year + 1}";

        var parametres = await _context.ParametresCalendrier
            .AsNoTracking()
            .Where(p => p.AnneeAcademique == anneeCourante)
            .ToListAsync();

        var vm = new HomeViewModel();

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