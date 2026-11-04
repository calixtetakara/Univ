using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversitySystem.Domain.Entities;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Infrastructure.Services;
using UniversitySystem.Web.Authorization;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Areas.Scolarite.Controllers;

[Area("Scolarite")]
[Authorize(Roles = "scolarite,superadmin")]
[HasPermission("DOSSIER_LIRE")]
public class DecisionController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMatriculeService _matriculeService;

    public DecisionController(AppDbContext context, IMatriculeService matriculeService)
    {
        _context = context;
        _matriculeService = matriculeService;
    }

    // =========================================================
    // LISTE DES DOSSIERS (M3-02)
    // =========================================================
    public async Task<IActionResult> Index(string? searchNom, string? codeFaculte, string? codeFiliere, int page = 1)
    {
        const int pageSize = 25;

        var query = _context.Etudiants
            .AsNoTracking()
            .Include(e => e.Utilisateur)
            .Include(e => e.Filiere)
                .ThenInclude(f => f.Faculte)
            .Where(e => e.StatutDossier == StatutInscription.Brouillon);

        if (!string.IsNullOrWhiteSpace(searchNom))
        {
            var nomLower = searchNom.ToLower();
            query = query.Where(e => e.Utilisateur.Nom.ToLower().Contains(nomLower) 
                                  || e.Utilisateur.Prenom.ToLower().Contains(nomLower));
        }

        if (!string.IsNullOrWhiteSpace(codeFaculte))
            query = query.Where(e => e.Filiere.Faculte.CodeFaculte == codeFaculte);

        if (!string.IsNullOrWhiteSpace(codeFiliere))
            query = query.Where(e => e.Filiere.CodeFiliere == codeFiliere);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var dossiers = await query
            .OrderByDescending(e => e.DateCreation)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new DossierEnCoursDto
            {
                IdEtudiant = e.IdEtudiant,
                Matricule = e.Matricule,
                Nom = e.Utilisateur.Nom,
                Prenom = e.Utilisateur.Prenom,
                NomFaculte = e.Filiere.Faculte.NomFaculte,
                NomFiliere = e.Filiere.NomFiliere,
                DateSoumission = e.DateCreation
            })
            .ToListAsync();

        var vm = new DecisionIndexViewModel
        {
            Dossiers = dossiers,
            SearchNom = searchNom,
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
    // TRAITER UN DOSSIER (M3-03)
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> Traiter(int id)
    {
        var etudiant = await _context.Etudiants
            .Include(e => e.Utilisateur)
            .FirstOrDefaultAsync(e => e.IdEtudiant == id);

        if (etudiant == null) return NotFound();

        if (etudiant.StatutDossier != StatutInscription.Brouillon)
        {
            TempData["Error"] = "Ce dossier a déjà été traité.";
            return RedirectToAction(nameof(Index));
        }

        var inscription = await _context.Inscriptions
            .Include(i => i.Filiere)
                .ThenInclude(f => f.Faculte)
            .FirstOrDefaultAsync(i => i.IdEtudiant == id);

        if (inscription == null || inscription.Filiere == null)
        {
            TempData["Error"] = "Erreur : L'inscription ou la filière est introuvable pour cet étudiant.";
            return RedirectToAction(nameof(Index));
        }

        var notes = await _context.NotesBacs
            .Where(n => n.IdEtudiant == id)
            .ToListAsync();

        var documents = await _context.DocumentsEtudiant
            .Where(d => d.IdEtudiant == id)
            .ToListAsync();

        var vm = new DecisionTraiterViewModel
        {
            Etudiant = etudiant,
            Inscription = inscription,
            Notes = notes,
            Documents = documents,
            TypeFaculte = inscription.Filiere.Faculte?.TypeFaculte.ToString() ?? "Inconnu"
        };

        etudiant.Filiere = inscription.Filiere;
        return View(vm);
    }

    // =========================================================
    // APPROUVER (M3-06)
    // =========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [HasPermission("DOSSIER_TRAITER")]
    public async Task<IActionResult> Approuver(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userIdClaim, out int currentUserId);

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var etudiant = await _context.Etudiants
                .Include(e => e.Utilisateur)
                .FirstOrDefaultAsync(e => e.IdEtudiant == id);

            if (etudiant == null) return NotFound();

            var inscription = await _context.Inscriptions
                .Include(i => i.Filiere)
                    .ThenInclude(f => f.Faculte)
                .FirstOrDefaultAsync(i => i.IdEtudiant == id);

            if (inscription == null || inscription.Filiere == null)
            {
                TempData["Error"] = "Inscription ou filière introuvable.";
                return RedirectToAction(nameof(Traiter), new { id });
            }

            if (etudiant.StatutDossier != StatutInscription.Brouillon)
            {
                TempData["Error"] = "Ce dossier a déjà été traité.";
                return RedirectToAction(nameof(Index));
            }

            // Générer le matricule (le service utilise la même transaction)
            var codeFaculte = inscription.Filiere.Faculte?.CodeFaculte ?? "UNK";
            var matricule = await _matriculeService.GenerateAsync(codeFaculte);

            etudiant.Matricule = matricule;
            etudiant.StatutDossier = StatutInscription.Validee;

            inscription.Statut = StatutInscription.Validee;
            inscription.DateTraitement = DateTime.UtcNow;

            _context.EmailQueues.Add(new EmailQueue
            {
                Destinataire = etudiant.Utilisateur.Email,
                Sujet = "Inscription validée",
                CorpsHtml = $"<p>Félicitations, votre inscription a été validée.</p><p>Votre matricule est : <strong>{matricule}</strong></p>",
                Envoye = false,
                DateCreation = DateTime.UtcNow,
                Tentatives = 0
            });

            _context.AuditLogs.Add(new AuditLog
            {
                IdUtilisateur = currentUserId,
                Action = "DOSSIER_APPROUVE",
                Entite = "Etudiant",
                IdEntite = id.ToString(),
                Details = $"{{\"matricule\":\"{matricule}\", \"approvedBy\":{currentUserId}}}",
                AdresseIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DateAction = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = $"Dossier approuvé ! Matricule généré : {matricule}";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["Error"] = $"Erreur lors de l'approbation : {ex.Message}";
            return RedirectToAction(nameof(Traiter), new { id });
        }
    }

        // =========================================================
    // REJETER (M3-07)
    // =========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [HasPermission("DOSSIER_TRAITER")]
    public async Task<IActionResult> Rejeter(int id, string motif)
    {
        // 1. Validation du motif
        if (string.IsNullOrWhiteSpace(motif) || motif.Length < 10)
        {
            TempData["Error"] = "Le motif de rejet est obligatoire (minimum 10 caractères).";
            return RedirectToAction(nameof(Traiter), new { id });
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userIdClaim, out int currentUserId);

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var etudiant = await _context.Etudiants
                .Include(e => e.Utilisateur)
                .FirstOrDefaultAsync(e => e.IdEtudiant == id);

            if (etudiant == null) return NotFound();

            var inscription = await _context.Inscriptions
                .FirstOrDefaultAsync(i => i.IdEtudiant == id);

            if (inscription == null)
            {
                TempData["Error"] = "Inscription introuvable.";
                return RedirectToAction(nameof(Traiter), new { id });
            }

            // 2. Idempotence
            if (etudiant.StatutDossier != StatutInscription.Brouillon)
            {
                TempData["Error"] = "Ce dossier a déjà été traité.";
                return RedirectToAction(nameof(Index));
            }

            // 3. Mise à jour de l'étudiant
            etudiant.StatutDossier = StatutInscription.Rejetee;
            etudiant.MotifRejet = motif;

            // 4. Mise à jour de l'inscription
            inscription.Statut = StatutInscription.Rejetee;
            inscription.DateTraitement = DateTime.UtcNow;
            inscription.Commentaire = motif;

            // 5. Email de rejet (RG-06)
            _context.EmailQueues.Add(new EmailQueue
            {
                Destinataire = etudiant.Utilisateur.Email,
                Sujet = "Inscription rejetée",
                CorpsHtml = $"<p>Votre dossier a été rejeté.</p><p><strong>Motif :</strong> {motif}</p>",
                Envoye = false,
                DateCreation = DateTime.UtcNow,
                Tentatives = 0
            });

            // 6. Audit (RG-15)
            _context.AuditLogs.Add(new AuditLog
            {
                IdUtilisateur = currentUserId,
                Action = "DOSSIER_REJETE",
                Entite = "Etudiant",
                IdEntite = id.ToString(),
                Details = $"{{\"motif\":\"{motif}\", \"rejectedBy\":{currentUserId}}}",
                AdresseIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DateAction = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = "Le dossier a été rejeté et l'étudiant a été notifié.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["Error"] = $"Erreur lors du rejet : {ex.Message}";
            return RedirectToAction(nameof(Traiter), new { id });
        }
    }
}