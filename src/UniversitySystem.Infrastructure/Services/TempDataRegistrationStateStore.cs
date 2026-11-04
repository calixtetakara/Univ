using UniversitySystem.Application.DTOs;

namespace UniversitySystem.Infrastructure.Services;

/// <summary>
/// Contrat pour le stockage de l'état du wizard.
/// </summary>
public interface IRegistrationStateStore
{
    void Save(RegistrationStateDto state);
    RegistrationStateDto? Load();
    void Clear();
}