using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UniversitySystem.Domain.Entities;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;

namespace UniversitySystem.Infrastructure.Services;

/// <summary>
/// Implémentation du service d'authentification.
/// - Vérifie email + mot de passe (BCrypt)
/// - Applique un rate limiting par email (RG-12)
/// - Écrit les événements dans la table audit_logs
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    /// <summary>Nombre maximum de tentatives échouées avant verrouillage.</summary>
    private const int MaxFailedAttempts = 5;

    /// <summary>Durée du verrouillage après trop de tentatives.</summary>
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public AuthService(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<LoginResult> ValidateCredentialsAsync(string email, string password, string? ipAddress = null)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var cacheKey = $"login_failures:{normalizedEmail}";

        // -------------------------------------------------------
        // 1. Rate limiting applicatif par email (RG-12)
        // -------------------------------------------------------
        if (_cache.TryGetValue(cacheKey, out int failures) && failures >= MaxFailedAttempts)
        {
            await WriteAuditAsync(null, "LOGIN_LOCKED",
                details: $"email={normalizedEmail}", ipAddress);

            return new LoginResult
            {
                Success = false,
                IsLockedOut = true,
                ErrorMessage = "Trop de tentatives. Réessayez dans 15 minutes."
            };
        }

        // -------------------------------------------------------
        // 2. Recherche de l'utilisateur
        // -------------------------------------------------------
        var user = await _context.Utilisateurs
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        // Message générique pour éviter l'énumération d'emails
        const string genericError = "Email ou mot de passe incorrect.";

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.MotDePasseHash))
        {
            // Incrémentation du compteur d'échecs
            failures = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = LockoutDuration;
                return 0;
            });

            failures++;
            _cache.Set(cacheKey, failures, LockoutDuration);

            await WriteAuditAsync(user?.IdUtilisateur, "LOGIN_FAILED",
                details: $"email={normalizedEmail}; attempts={failures}", ipAddress);

            return new LoginResult
            {
                Success = false,
                ErrorMessage = genericError
            };
        }

        // -------------------------------------------------------
        // 3. Vérification du statut du compte
        // -------------------------------------------------------
        if (user.Statut != StatutUtilisateur.Actif)
        {
            await WriteAuditAsync(user.IdUtilisateur, "LOGIN_DISABLED",
                details: $"email={normalizedEmail}; statut={user.Statut}", ipAddress);

            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Votre compte est désactivé ou suspendu."
            };
        }

        // -------------------------------------------------------
        // 4. Succès → on remet le compteur à zéro
        // -------------------------------------------------------
        _cache.Remove(cacheKey);

        await WriteAuditAsync(user.IdUtilisateur, "LOGIN_SUCCESS",
            details: $"email={normalizedEmail}", ipAddress);

        return new LoginResult
        {
            Success = true,
            User = user
        };
    }

    /// <summary>
    /// Écrit une ligne dans la table audit_logs.
    /// </summary>
    private async Task WriteAuditAsync(int? userId, string action, string? details = null, string? ip = null)
    {
        _context.AuditLogs.Add(new AuditLog
        {
            IdUtilisateur = userId,
            Action = action,
            Entite = "Utilisateur",
            Details = details,
            AdresseIp = ip,
            DateAction = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }
}