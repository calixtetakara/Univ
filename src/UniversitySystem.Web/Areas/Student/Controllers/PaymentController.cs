using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Infrastructure.Services;
using UniversitySystem.Web.Authorization;

namespace UniversitySystem.Web.Areas.Student.Controllers;

[Area("Student")]
[Authorize(Roles = "etudiant")]
public class PaymentController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IPaymentGatewayClient _paymentGateway;

    public PaymentController(
        AppDbContext context, 
        IPaymentService paymentService,
        IPaymentGatewayClient paymentGateway)
    {
        _context = context;
        _paymentService = paymentService;
        _paymentGateway = paymentGateway;
    }

    [HttpGet]
    [HasPermission("PAIEMENT_EFFECTUER")]
    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return RedirectToAction("Login", "Account", new { area = "" });

        var etudiant = await _context.Etudiants
            .Include(e => e.Filiere)
            .FirstOrDefaultAsync(e => e.IdUtilisateur == userId);

        if (etudiant == null) return NotFound();

        if (etudiant.StatutDossier != UniversitySystem.Domain.Enums.StatutInscription.Validee)
        {
            TempData["Error"] = "Votre dossier n'est pas validé, vous ne pouvez pas payer.";
            return RedirectToAction("Index", "Dashboard");
        }

        var inscription = await _context.Inscriptions
            .FirstOrDefaultAsync(i => i.IdEtudiant == etudiant.IdEtudiant);

        if (inscription == null) return NotFound("Inscription introuvable.");

        var paiement = await _context.Paiements
            .Where(p => p.IdInscription == inscription.IdInscription)
            .OrderByDescending(p => p.DatePaiement)
            .FirstOrDefaultAsync();

        ViewBag.Montant = 50000;
        ViewBag.Paiement = paiement;

        return View();
    }

    [HttpPost]
    [HasPermission("PAIEMENT_EFFECTUER")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Initiate()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return RedirectToAction("Login", "Account", new { area = "" });

        var etudiant = await _context.Etudiants.FirstOrDefaultAsync(e => e.IdUtilisateur == userId);
        if (etudiant == null) return NotFound();

        var inscription = await _context.Inscriptions.FirstOrDefaultAsync(i => i.IdEtudiant == etudiant.IdEtudiant);
        if (inscription == null) return NotFound("Inscription introuvable.");

        try
        {
            var reference = await _paymentService.InitiateAsync(inscription.IdInscription, 50000, "Mobile Money");
            var redirectUrl = await _paymentGateway.CreatePaymentAsync(reference, 50000, "Mobile Money");
            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult FakeGateway(string @ref)
    {
        ViewBag.Reference = @ref;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ProcessFakePayment(string reference)
    {
        await _paymentService.HandleWebhookAsync(reference, "reussi");
        TempData["Success"] = "Paiement simulé avec succès !";
        return RedirectToAction("Index", "Dashboard");
    }
        [HttpGet]
    [HasPermission("RECU_TELECHARGER")]
    public async Task<IActionResult> Receipt(int id)
    {
        // 1. Récupérer l'ID de l'utilisateur connecté
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return Forbid();

        // 2. Charger le paiement et vérifier qu'il appartient à l'étudiant
        var paiement = await _context.Paiements
            .Include(p => p.Inscription)
                .ThenInclude(i => i.Etudiant)
                    .ThenInclude(e => e.Utilisateur)
            .Include(p => p.Inscription.Filiere)
            .FirstOrDefaultAsync(p => p.IdPaiement == id);

        if (paiement == null) return NotFound();

        // Vérification du scope étudiant (RG-11)
        if (paiement.Inscription.Etudiant.IdUtilisateur != userId)
            return Forbid();

        // Vérifier que le paiement est réussi
        if (paiement.Statut != UniversitySystem.Domain.Enums.StatutPaiement.Valide)
            return BadRequest("Le reçu n'est disponible que pour les paiements validés.");

        // 3. Générer le PDF
        var generator = new UniversitySystem.Infrastructure.Receipts.QuestPdfReceiptGenerator();
        var pdfBytes = generator.Generate(paiement, paiement.Inscription.Etudiant, paiement.Inscription);

        // 4. Retourner le fichier
        return File(pdfBytes, "application/pdf", $"recu_{paiement.ReferenceTransaction}.pdf");
    }
}