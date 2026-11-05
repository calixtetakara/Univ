using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Web.Authorization;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Areas.SuperAdmin.Controllers;

[Area("SuperAdmin")]
[Authorize(Roles = "superadmin")]
[HasPermission("FINANCES_LIRE")]
public class FinancesController : Controller
{
    private readonly AppDbContext _context;

    public FinancesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? moyenPaiement, 
        string? typePaiement, 
        DateTime? dateDebut, 
        DateTime? dateFin, 
        string? codeFaculte, 
        string? codeFiliere, 
        int page = 1)
    {
        const int pageSize = 50;

        // 1. Requête de base : paiements réussis (statut = Valide)
        var query = _context.Paiements
            .AsNoTracking()
            .Include(p => p.Inscription)
                .ThenInclude(i => i.Etudiant)
                    .ThenInclude(e => e.Utilisateur)
            .Include(p => p.Inscription.Filiere)
                .ThenInclude(f => f.Faculte)
            .Where(p => p.Statut == Domain.Enums.StatutPaiement.Valide);

        // 2. Application des filtres
        if (!string.IsNullOrWhiteSpace(moyenPaiement))
            query = query.Where(p => p.ModePaiement == moyenPaiement);

        if (!string.IsNullOrWhiteSpace(codeFaculte))
            query = query.Where(p => p.Inscription.Filiere.Faculte.CodeFaculte == codeFaculte);

        if (!string.IsNullOrWhiteSpace(codeFiliere))
            query = query.Where(p => p.Inscription.Filiere.CodeFiliere == codeFiliere);

        if (dateDebut.HasValue)
            query = query.Where(p => p.DatePaiement >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(p => p.DatePaiement <= dateFin.Value.AddDays(1)); // Inclure la journée

        // 3. Calcul des totaux (avant pagination)
        var allPaiements = await query.ToListAsync();
        var totalGlobal = allPaiements.Sum(p => p.Montant);
        
        var totalParMoyen = allPaiements
            .GroupBy(p => p.ModePaiement)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Montant));

        // On n'a pas de champ TypePaiement dans l'entité, on met tout en "Scolarité" pour l'instant
        var totalParType = new Dictionary<string, decimal> { { "Scolarité", totalGlobal } };

        // 4. Pagination
        var totalCount = allPaiements.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var transactions = allPaiements
            .OrderByDescending(p => p.DatePaiement)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new TransactionDto
            {
                IdPaiement = p.IdPaiement,
                Reference = p.ReferenceTransaction,
                Matricule = p.Inscription.Etudiant.Matricule ?? "N/A",
                NomComplet = $"{p.Inscription.Etudiant.Utilisateur.Nom} {p.Inscription.Etudiant.Utilisateur.Prenom}",
                Faculte = p.Inscription.Filiere.Faculte.NomFaculte,
                Filiere = p.Inscription.Filiere.NomFiliere,
                TypePaiement = "Scolarité",
                MoyenPaiement = p.ModePaiement,
                Montant = p.Montant,
                DatePaiement = p.DatePaiement
            })
            .ToList();

        // 5. Préparation du ViewModel
        var vm = new FinanceIndexViewModel
        {
            Transactions = transactions,
            TotalGlobal = totalGlobal,
            TotalParMoyen = totalParMoyen,
            TotalParType = totalParType,
            MoyenPaiement = moyenPaiement,
            TypePaiement = typePaiement,
            DateDebut = dateDebut,
            DateFin = dateFin,
            CodeFaculte = codeFaculte,
            CodeFiliere = codeFiliere,
            Page = page,
            TotalPages = totalPages,
            TotalCount = totalCount
        };

        // Charger les listes pour les filtres
        ViewBag.Facultes = await _context.Facultes.ToListAsync();
        ViewBag.MoyensPaiement = new[] { "Mobile Money", "Banque", "Espèces", "Autre" };

        return View(vm);
    }
}