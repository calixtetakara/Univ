namespace UniversitySystem.Domain.Enums;

/// <summary>
/// Types de documents pouvant être associés à un étudiant.
/// </summary>
public enum TypeDocument
{
    /// <summary>Pièce d'identité (CNI, passeport...).</summary>
    PieceIdentite = 1,

    /// <summary>Diplôme du baccalauréat.</summary>
    DiplomeBac = 2,

    /// <summary>Relevé de notes du bac.</summary>
    ReleveNotes = 3,

    /// <summary>Photo d'identité.</summary>
    Photo = 4,

    /// <summary>Autre document.</summary>
    Autre = 5
}