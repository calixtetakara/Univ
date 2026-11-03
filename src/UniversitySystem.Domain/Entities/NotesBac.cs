namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Notes du baccalauréat d'un étudiant (utilisées pour le calcul de la moyenne d'admission).
/// </summary>
public class NotesBac
{
    /// <summary>Identifiant technique (PK).</summary>
    public int IdNotesBac { get; set; }

    /// <summary>Étudiant concerné.</summary>
    public int IdEtudiant { get; set; }

    /// <summary>Matière du bac.</summary>
    public string Matiere { get; set; } = default!;

    /// <summary>Note obtenue (sur 20).</summary>
    public decimal Note { get; set; }

    /// <summary>Coefficient de la matière.</summary>
    public decimal Coefficient { get; set; }

    /// <summary>Navigation vers l'étudiant.</summary>
    public Etudiant Etudiant { get; set; } = default!;
}