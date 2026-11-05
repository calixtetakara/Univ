namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Override individuel d'une permission pour un utilisateur (M6-04).
/// Si une ligne existe, elle gagne sur la permission du rôle.
/// </summary>
public class UtilisateurPermission
{
    public int IdUtilisateur { get; set; }
    public int IdPermission { get; set; }
    
    /// <summary>True = accordé, False = refusé.</summary>
    public bool Accorde { get; set; }

    /// <summary>L'ID du SuperAdmin qui a fait la modification.</summary>
    public int? ModifiePar { get; set; }

    // Navigation
    public Utilisateur Utilisateur { get; set; } = default!;
    public Permission Permission { get; set; } = default!;
}