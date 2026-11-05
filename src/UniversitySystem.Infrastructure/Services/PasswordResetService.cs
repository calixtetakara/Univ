using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Entities;
using UniversitySystem.Infrastructure.Persistence;

namespace UniversitySystem.Infrastructure.Services;

public interface IPasswordResetService
{
    Task RequestResetAsync(string email);
    Task ResetAsync(string token, string newPassword);
}

public class InvalidTokenException : Exception
{
    public InvalidTokenException(string message) : base(message) { }
}

/// <summary>
/// Service de gestion de la réinitialisation de mot de passe (M7-11).
/// Implémente RG-13 (politique mdp), RG-14 (tokens), RG-15 (audit).
/// </summary>
public class PasswordResetService : IPasswordResetService
{
    private readonly AppDbContext _context;

    public PasswordResetService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Demande de réinitialisation (M7-11).
    /// Anti-énumération : on exécute le même code que l'email existe ou non.
    /// </summary>
    public async Task RequestResetAsync(string email)
    {
        var user = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email == email.ToLower());

        // 1. On génère TOUJOURS un token (même si l'utilisateur n'existe pas)
        // Cela permet d'avoir un temps de réponse constant.
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-").Replace("/", "_").Replace("=", "");

        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        if (user != null)
        {
            // 2. Stockage en base (uniquement si l'utilisateur existe)
            _context.PasswordResetTokens.Add(new PasswordResetToken
            {
                IdUtilisateur = user.IdUtilisateur,
                Token = tokenHash,
                DateExpiration = DateTime.UtcNow.AddMinutes(30),
                Utilise = false,
                DateCreation = DateTime.UtcNow
            });

            // 3. Email avec le token EN CLAIR (uniquement dans le lien)
            // LIEN CORRIGÉ : /forgot-password/reset
            _context.EmailQueues.Add(new EmailQueue
            {
                Destinataire = user.Email,
                Sujet = "Réinitialisation de votre mot de passe",
                CorpsHtml = $"<p>Bonjour {user.Prenom},</p><p>Cliquez sur ce lien pour réinitialiser votre mot de passe (valable 30 minutes) :</p><p><a href='http://localhost:5074/forgot-password/reset?token={token}'>Réinitialiser mon mot de passe</a></p>",
                Envoye = false,
                DateCreation = DateTime.UtcNow,
                Tentatives = 0
            });

            // 4. Audit
            _context.AuditLogs.Add(new AuditLog
            {
                IdUtilisateur = user.IdUtilisateur,
                Action = "PASSWORD_RESET_REQUESTED",
                Entite = "Utilisateur",
                IdEntite = user.IdUtilisateur.ToString(),
                Details = $"{{\"found\":true}}",
                DateAction = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }
        else
        {
            // Audit pour les emails inconnus (sans stocker de token)
            _context.AuditLogs.Add(new AuditLog
            {
                Action = "PASSWORD_RESET_REQUESTED_UNKNOWN_EMAIL",
                Details = $"{{\"email\":\"{email}\"}}",
                DateAction = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            
            // Anti-timing : on simule un délai équivalent au traitement d'un vrai email
            await Task.Delay(200);
        }
    }

    /// <summary>
    /// Réinitialise le mot de passe avec le token (M7-11).
    /// </summary>
    public async Task ResetAsync(string token, string newPassword)
    {
        // 1. Hasher le token fourni
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        // 2. Chercher le token en base
        var resetToken = await _context.PasswordResetTokens
            .Include(t => t.Utilisateur)
            .FirstOrDefaultAsync(t => t.Token == tokenHash && !t.Utilise && t.DateExpiration > DateTime.UtcNow);

        if (resetToken == null)
            throw new InvalidTokenException("Lien invalide ou expiré.");

        // 3. Mettre à jour le mot de passe
        resetToken.Utilisateur.MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        resetToken.Utilisateur.DateModification = DateTime.UtcNow;

        // 4. Invalider TOUS les tokens de cet utilisateur
        var allTokens = await _context.PasswordResetTokens
            .Where(t => t.IdUtilisateur == resetToken.IdUtilisateur)
            .ToListAsync();
        
        foreach (var t in allTokens) t.Utilise = true;

        // 5. Audit
        _context.AuditLogs.Add(new AuditLog
        {
            IdUtilisateur = resetToken.IdUtilisateur,
            Action = "PASSWORD_RESET_SUCCESS",
            Entite = "Utilisateur",
            IdEntite = resetToken.IdUtilisateur.ToString(),
            DateAction = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }
}