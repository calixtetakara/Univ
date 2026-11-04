using Microsoft.EntityFrameworkCore;
using UniversitySystem.Domain.Entities;
using UniversitySystem.Domain.Enums;
using UniversitySystem.Infrastructure.Persistence;

namespace UniversitySystem.Infrastructure.Services;

public interface IPaymentService
{
    Task<string> InitiateAsync(int idInscription, decimal montant, string moyenPaiement);
    Task HandleWebhookAsync(string reference, string statutRecu);
}

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;

    public PaymentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> InitiateAsync(int idInscription, decimal montant, string moyenPaiement)
    {
        // 1. Vérifier que le dossier est validé
        var inscription = await _context.Inscriptions
            .Include(i => i.Etudiant)
            .FirstOrDefaultAsync(i => i.IdInscription == idInscription);

        if (inscription == null || inscription.Etudiant.StatutDossier != StatutInscription.Validee)
            throw new Exception("Le dossier n'est pas validé, paiement impossible.");

        // 2. Vérifier qu'il n'y a pas déjà un paiement réussi
        var existingPayment = await _context.Paiements
            .AnyAsync(p => p.IdInscription == idInscription && p.Statut == StatutPaiement.Valide);

        if (existingPayment)
            throw new Exception("Un paiement a déjà été effectué pour cette inscription.");

        // 3. Créer la ligne de paiement
        var reference = $"PAY-{Guid.NewGuid():N}";
        
        var paiement = new Paiement
        {
            IdInscription = idInscription,
            Montant = montant,
            ReferenceTransaction = reference,
            ModePaiement = moyenPaiement,
            Statut = StatutPaiement.EnAttente,
            DatePaiement = DateTime.UtcNow
        };

        _context.Paiements.Add(paiement);
        await _context.SaveChangesAsync();

        return reference;
    }

    public async Task HandleWebhookAsync(string reference, string statutRecu)
    {
        var paiement = await _context.Paiements
            .FirstOrDefaultAsync(p => p.ReferenceTransaction == reference);

        if (paiement == null || paiement.Statut == StatutPaiement.Valide)
            return; // Idempotence

        if (statutRecu == "reussi")
        {
            paiement.Statut = StatutPaiement.Valide;
            paiement.DateTraitement = DateTime.UtcNow;

            // Email de confirmation
            var inscription = await _context.Inscriptions
                .Include(i => i.Etudiant)
                    .ThenInclude(e => e.Utilisateur)
                .FirstOrDefaultAsync(i => i.IdInscription == paiement.IdInscription);

            if (inscription != null)
            {
                _context.EmailQueues.Add(new EmailQueue
                {
                    Destinataire = inscription.Etudiant.Utilisateur.Email,
                    Sujet = "Paiement confirmé",
                    CorpsHtml = $"<p>Votre paiement de {paiement.Montant} FCFA a été confirmé.</p>",
                    Envoye = false,
                    DateCreation = DateTime.UtcNow,
                    Tentatives = 0
                });
            }
        }
        else
        {
            paiement.Statut = StatutPaiement.Echec;
            paiement.DateTraitement = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}