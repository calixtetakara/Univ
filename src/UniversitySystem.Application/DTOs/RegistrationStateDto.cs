namespace UniversitySystem.Application.DTOs;

/// <summary>
/// Représente l'état complet du dossier d'inscription d'un étudiant
/// à travers les 3 étapes du wizard.
/// Ces données sont sérialisées, chiffrées et stockées temporairement
/// dans le TempData (cookie) entre chaque étape.
/// </summary>
public class RegistrationStateDto
{
    // =========================================================
    // ÉTAPE 1 : Identité
    // =========================================================
    
    /// <summary>Numéro de table du baccalauréat (obligatoire).</summary>
    public string? NumTableBac { get; set; }

    /// <summary>Année de session du baccalauréat (ex: 2024).</summary>
    public int? SessionBac { get; set; }

    /// <summary>Nom de famille de l'étudiant.</summary>
    public string? Nom { get; set; }

    /// <summary>Prénom de l'étudiant.</summary>
    public string? Prenom { get; set; }

    /// <summary>Sexe de l'étudiant (M ou F).</summary>
    public string? Sexe { get; set; }

    /// <summary>Date de naissance (vérifiée ≥ 15 ans).</summary>
    public DateTime? DateNaissance { get; set; }

    /// <summary>Lieu de naissance.</summary>
    public string? LieuNaissance { get; set; }

    /// <summary>Adresse email (servira d'identifiant de connexion).</summary>
    public string? Email { get; set; }

    /// <summary>
    /// Hash BCrypt du mot de passe.
    /// On stocke UNIQUEMENT le hash, jamais le mot de passe en clair,
    /// même dans le TempData chiffré.
    /// </summary>
    public string? MotDePasseHash { get; set; }


    // =========================================================
    // ÉTAPE 2 : Parcours & Notes
    // =========================================================
    
    /// <summary>Niveau d'études (Licence ou Master).</summary>
    public string? Niveau { get; set; }

    /// <summary>Code de la faculté choisie (ex: FDS).</summary>
    public string? CodeFaculte { get; set; }

    /// <summary>Code de la filière choisie (ex: LIC-INFO).</summary>
    public string? CodeFiliere { get; set; }

    // --- Notes Scientifiques ---
    /// <summary>Note de Mathématiques (0-20).</summary>
    public decimal? NoteMaths { get; set; }
    
    /// <summary>Note de SVT (0-20).</summary>
    public decimal? NoteSvt { get; set; }
    
    /// <summary>Note de Physique-Chimie (0-20).</summary>
    public decimal? NotePhysique { get; set; }

    // --- Notes Littéraires ---
    /// <summary>Note de Français (0-20).</summary>
    public decimal? NoteFrancais { get; set; }
    
    /// <summary>Note d'Histoire-Géographie (0-20).</summary>
    public decimal? NoteHistoireGeo { get; set; }
    
    /// <summary>Note de Philosophie (0-20).</summary>
    public decimal? NotePhilo { get; set; }
    
    /// <summary>Note d'Anglais (0-20).</summary>
    public decimal? NoteAnglais { get; set; }


    // =========================================================
    // HELPERS (Propriétés calculées pour les Guards)
    // =========================================================

    /// <summary>
    /// Indique si l'étape 1 (Identité) a bien été complétée.
    /// Utilisé par le Controller pour bloquer l'accès à l'étape 2.
    /// </summary>
    public bool IsStep1Completed => 
        !string.IsNullOrWhiteSpace(Email) && 
        !string.IsNullOrWhiteSpace(MotDePasseHash) &&
        !string.IsNullOrWhiteSpace(Nom);

    /// <summary>
    /// Indique si l'étape 2 (Parcours) a bien été complétée.
    /// Utilisé par le Controller pour bloquer l'accès à l'étape 3.
    /// </summary>
    public bool IsStep2Completed => 
        !string.IsNullOrWhiteSpace(CodeFiliere) && 
        !string.IsNullOrWhiteSpace(Niveau);
}