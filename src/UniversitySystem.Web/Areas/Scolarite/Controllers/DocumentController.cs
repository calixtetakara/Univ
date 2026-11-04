using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Infrastructure.Services;

namespace UniversitySystem.Web.Controllers;

[Authorize] // Exige d'être connecté (Fallback Policy)
public class DocumentController : Controller
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly IPermissionService _permissionService;

    public DocumentController(AppDbContext context, IFileStorageService fileStorage, IPermissionService permissionService)
    {
        _context = context;
        _fileStorage = fileStorage;
        _permissionService = permissionService;
    }

    /// <summary>
    /// Affiche un document (PDF, JPG, PNG) de manière sécurisée (RG-11).
    /// </summary>
    [HttpGet("/Document/View/{id}")]
    public async Task<IActionResult> View(int id)
    {
        // 1. Charger le document
        var document = await _context.DocumentsEtudiant
            .Include(d => d.Etudiant)
            .FirstOrDefaultAsync(d => d.IdDocument == id);

        if (document == null) return NotFound("Document introuvable.");

        // 2. Récupérer l'ID de l'utilisateur connecté
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return Forbid();

        // 3. Vérification des autorisations (RG-11)
        bool isAuthorized = false;

        if (User.IsInRole("superadmin") || User.IsInRole("scolarite"))
        {
            isAuthorized = await _permissionService.HasPermissionAsync(userId, "DOSSIER_LIRE");
        }
        else if (User.IsInRole("etudiant"))
        {
            // L'étudiant ne peut voir que SES documents
            isAuthorized = (document.Etudiant.IdUtilisateur == userId);
        }

        if (!isAuthorized)
            return Forbid();

        // 4. Récupérer le chemin physique du fichier
        var physicalPath = _fileStorage.GetPhysicalPath(document.CheminFichier);
        
        if (!System.IO.File.Exists(physicalPath))
            return NotFound("Fichier introuvable sur le disque.");

        // 5. Headers de sécurité
        Response.Headers.Add("Cache-Control", "private, max-age=300");
        Response.Headers.Add("X-Content-Type-Options", "nosniff");
        Response.Headers.Add("Content-Disposition", "inline");

        // 6. Retourner le fichier
        return PhysicalFile(physicalPath, document.ContentType);
    }
}