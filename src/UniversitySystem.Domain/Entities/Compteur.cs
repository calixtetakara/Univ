namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Compteur atomique pour générer des matricules séquentiels (ex: 2026INFO0001).
/// Utilise le pattern LAST_INSERT_ID pour garantir l'atomicité.
/// </summary>
public class Compteur
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdCompteur { get; set; }

    /// <summary>Clé du compteur (ex: "MATRICULE_ETUDIANT_2026_INFO").</summary>
    public string Cle { get; set; } = default!;

    /// <summary>Valeur courante du compteur.</summary>
    public int Valeur { get; set; }
}