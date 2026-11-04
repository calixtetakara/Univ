using Microsoft.EntityFrameworkCore;
using UniversitySystem.Infrastructure.Persistence;

namespace UniversitySystem.Infrastructure.Services;

/// <summary>
/// Service de vérification d'éligibilité d'un étudiant.
/// Implémente RG-01 : unicité de l'email et du couple (num_table_bac, session_bac).
/// </summary>
public interface IEligibilityService
{
    Task<bool> EmailExistsAsync(string email);
    Task<bool> NumTableBacExistsAsync(string numTableBac, int sessionBac);
}

public class EligibilityService : IEligibilityService
{
    private readonly AppDbContext _context;

    public EligibilityService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<bool> EmailExistsAsync(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _context.Utilisateurs
            .AnyAsync(u => u.Email == normalized);
    }

    /// <inheritdoc />
    public async Task<bool> NumTableBacExistsAsync(string numTableBac, int sessionBac)
    {
        // Vérifie si un étudiant a déjà ce numéro de table pour cette session
        return await _context.Etudiants
            .AnyAsync(e => e.NumTableBac == numTableBac && e.SessionBac == sessionBac);
    }
}