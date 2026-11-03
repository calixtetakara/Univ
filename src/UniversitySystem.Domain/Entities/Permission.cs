namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Permission atomique du système RBAC.
/// Exemple de codes : "etudiant.creer", "inscription.valider", "paiement.voir".
/// </summary>
public class Permission
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdPermission { get; set; }

    /// <summary>Code technique unique de la permission.</summary>
    public string Code { get; set; } = default!;

    /// <summary>Libellé lisible de la permission.</summary>
    public string Nom { get; set; } = default!;

    /// <summary>Description optionnelle.</summary>
    public string? Description { get; set; }

    /// <summary>Module fonctionnel auquel appartient la permission (ex: "Scolarite", "Finance").</summary>
    public string? Module { get; set; }
}