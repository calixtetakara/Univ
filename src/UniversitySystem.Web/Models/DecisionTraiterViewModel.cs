using System.Collections.Generic;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Web.Models;

public class DecisionTraiterViewModel
{
    public Etudiant Etudiant { get; set; } = default!;
    public Inscription Inscription { get; set; } = default!;
    public IReadOnlyList<NotesBac> Notes { get; set; } = new List<NotesBac>();
    public IReadOnlyList<DocumentEtudiant> Documents { get; set; } = new List<DocumentEtudiant>();
    public string TypeFaculte { get; set; } = string.Empty; // "Scientifique" ou "Litteraire"
}