using Microsoft.EntityFrameworkCore;
using UniversitySystem.Infrastructure.Persistence;

namespace UniversitySystem.Infrastructure.Services;

public interface IMatriculeService
{
    Task<string> GenerateAsync(string codeFaculte);
}

public class MatriculeService : IMatriculeService
{
    private readonly AppDbContext _context;

    public MatriculeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateAsync(string codeFaculte)
    {
        var annee = DateTime.UtcNow.Year;
        var prefix = $"{annee}-{codeFaculte}";

        // On utilise le pattern EF Core standard.
        // Le verrou de transaction est géré par le contrôleur appelant (ou par défaut par EF Core).
        var compteur = await _context.Compteurs
            .FirstOrDefaultAsync(c => c.Cle == prefix);

        if (compteur == null)
        {
            compteur = new Domain.Entities.Compteur { Cle = prefix, Valeur = 1 };
            _context.Compteurs.Add(compteur);
        }
        else
        {
            compteur.Valeur += 1;
        }

        await _context.SaveChangesAsync();

        return $"{prefix}-{compteur.Valeur:D5}";
    }
}