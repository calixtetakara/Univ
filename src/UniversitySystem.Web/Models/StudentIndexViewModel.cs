using System.Collections.Generic;
using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Web.Models;

public class StudentIndexViewModel
{
    public IReadOnlyList<EtudiantListDto> Etudiants { get; set; } = new List<EtudiantListDto>();
    
    // Filtres
    public StatutInscription? StatutDossier { get; set; }
    public string? CodeFaculte { get; set; }
    public string? CodeFiliere { get; set; }
    
    // Pagination
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}

public class EtudiantListDto
{
    public int IdEtudiant { get; set; }
    public string? Matricule { get; set; }
    public string Nom { get; set; } = default!;
    public string Prenom { get; set; } = default!;
    public string NomFaculte { get; set; } = default!;
    public string NomFiliere { get; set; } = default!;
    public string Niveau { get; set; } = default!;
    public StatutInscription StatutDossier { get; set; }
}