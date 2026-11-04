namespace UniversitySystem.Domain.Enums;

/// <summary>
/// Type de faculté, utilisé pour déterminer quelles notes du BAC demander.
/// </summary>
public enum TypeFaculte
{
    /// <summary>Faculté scientifique (Maths, SVT, Physique...).</summary>
    Scientifique = 1,

    /// <summary>Faculté littéraire (Français, Histoire, Philo...).</summary>
    Litteraire = 2
}