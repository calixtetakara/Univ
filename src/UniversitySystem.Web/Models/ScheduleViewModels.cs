using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Web.Models;

public class ScheduleCreateViewModel
{
    [Required(ErrorMessage = "L'enseignant est obligatoire.")]
    public int IdEnseignant { get; set; }

    [Required(ErrorMessage = "L'UE est obligatoire.")]
    public string CodeUe { get; set; } = string.Empty;

    [Required(ErrorMessage = "La salle est obligatoire.")]
    public int IdSalle { get; set; }

    [Required(ErrorMessage = "La filière est obligatoire.")]
    public int IdFiliere { get; set; }

    [Required(ErrorMessage = "Le jour est obligatoire.")]
    [Range(1, 7)]
    public int JourSemaine { get; set; }

    [Required(ErrorMessage = "L'heure de début est obligatoire.")]
    public TimeSpan HeureDebut { get; set; }

    [Required(ErrorMessage = "L'heure de fin est obligatoire.")]
    public TimeSpan HeureFin { get; set; }

    [Required(ErrorMessage = "L'année académique est obligatoire.")]
    public string AnneeAcademique { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le semestre est obligatoire.")]
    [Range(1, 2)]
    public int Semestre { get; set; }
}