using UniversitySystem.Domain.Enums;

namespace UniversitySystem.Application.DTOs;

public class RegistrationStateDto
{
    public string? NumTableBac { get; set; }
    public int? SessionBac { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public Genre? Genre { get; set; }
    public DateTime? DateNaissance { get; set; }
    public string? LieuNaissance { get; set; }
    public string? Email { get; set; }
    public string? MotDePasseHash { get; set; }

    public string? Niveau { get; set; }
    public string? CodeFaculte { get; set; }
    public string? CodeFiliere { get; set; }
    public decimal? NoteMaths { get; set; }
    public decimal? NoteSvt { get; set; }
    public decimal? NotePhysique { get; set; }
    public decimal? NoteFrancais { get; set; }
    public decimal? NoteHistoireGeo { get; set; }
    public decimal? NotePhilo { get; set; }
    public decimal? NoteAnglais { get; set; }

    public bool IsStep1Completed => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(MotDePasseHash);
    public bool IsStep2Completed => !string.IsNullOrWhiteSpace(CodeFiliere) && !string.IsNullOrWhiteSpace(Niveau);
}