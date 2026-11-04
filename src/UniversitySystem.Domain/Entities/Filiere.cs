using System.Collections.Generic;

namespace UniversitySystem.Domain.Entities;

/// <summary>
/// Filière de l'université rattachée à une faculté.
/// </summary>
public class Filiere
{
    public int IdFiliere { get; set; }
    public string CodeFiliere { get; set; } = default!;
    public string NomFiliere { get; set; } = default!;

    // Clé étrangère vers Faculte (int)
    public int IdFaculte { get; set; }
    public Faculte Faculte { get; set; } = default!;

    // Navigation
    public ICollection<FiliereUe> FiliereUes { get; set; } = new List<FiliereUe>();
}