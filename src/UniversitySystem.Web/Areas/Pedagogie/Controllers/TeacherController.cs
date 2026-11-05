using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Entities;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Web.Authorization;
using UniversitySystem.Web.Models;

namespace UniversitySystem.Web.Areas.Pedagogie.Controllers;

[Area("Pedagogie")]
[Authorize(Roles = "pedagogie,superadmin")]
[HasPermission("ENSEIGNANT_CREER")]
public class TeacherController : Controller
{
    private readonly AppDbContext _context;

    public TeacherController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var enseignants = await _context.Enseignants
            .Include(e => e.Utilisateur)
            .OrderBy(e => e.Utilisateur.Nom)
            .ToListAsync();
        return View(enseignants);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TeacherCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeacherCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _context.Utilisateurs.AnyAsync(u => u.Email == model.Email.ToLower()))
        {
            ModelState.AddModelError(nameof(model.Email), "Cet email est déjà utilisé.");
            return View(model);
        }

        try
        {
            // 1. Génération du mot de passe temporaire (16 caractères)
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$";
            var random = new Random();
            var tempPassword = new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            
            // 2. Récupérer le rôle enseignant
            var role = await _context.Roles.FirstAsync(r => r.Code == "enseignant");

            // 3. Création de l'utilisateur
            var utilisateur = new Utilisateur
            {
                Email = model.Email.ToLower(),
                MotDePasseHash = passwordHash,
                Nom = model.Nom,
                Prenom = model.Prenom,
                Statut = StatutUtilisateur.Actif,
                DoitChangerMotDePasse = true,
                DateCreation = DateTime.UtcNow,
                IdRole = role.IdRole
            };
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            // 4. Création de l'enseignant (avec le nom complet pour éviter le conflit de namespace)
            var enseignant = new UniversitySystem.Domain.Entities.Enseignant
            {
                IdUtilisateur = utilisateur.IdUtilisateur,
                Specialite = model.Specialite,
                Statut = StatutEnseignant.Actif,
                Matricule = $"ENS-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}"
            };
            _context.Enseignants.Add(enseignant);
            await _context.SaveChangesAsync();

            // 5. Email avec le mot de passe temporaire
            _context.EmailQueues.Add(new EmailQueue
            {
                Destinataire = utilisateur.Email,
                Sujet = "Bienvenue - Vos identifiants",
                CorpsHtml = $"<p>Bonjour {utilisateur.Prenom},</p><p>Votre compte a été créé. Voici vos identifiants :</p><p>Email : {utilisateur.Email}<br>Mot de passe temporaire : <strong>{tempPassword}</strong></p><p>Veuillez changer votre mot de passe dès votre première connexion.</p>",
                Envoye = false,
                DateCreation = DateTime.UtcNow,
                Tentatives = 0
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "Enseignant créé avec succès.";
            TempData["TempPassword"] = tempPassword;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "";
            ModelState.AddModelError("", "Erreur : " + ex.Message + (string.IsNullOrEmpty(innerMessage) ? "" : " | Détails : " + innerMessage));
            return View(model);
        }
    }
}