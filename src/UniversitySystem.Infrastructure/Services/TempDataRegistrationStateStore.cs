using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using UniversitySystem.Application.DTOs;

namespace UniversitySystem.Infrastructure.Services;

/// <summary>
/// Contrat définissant comment l'état du wizard doit être stocké et récupéré.
/// </summary>
public interface IRegistrationStateStore
{
    /// <summary>Sauvegarde l'état chiffré dans le TempData.</summary>
    void Save(RegistrationStateDto state, ITempDataDictionary tempData);

    /// <summary>Charge et déchiffre l'état depuis le TempData (retourne null si invalide).</summary>
    RegistrationStateDto? Load(ITempDataDictionary tempData);

    /// <summary>Supprime l'état du TempData (nettoyage après succès).</summary>
    void Clear(ITempDataDictionary tempData);
}

/// <summary>
/// Implémentation sécurisée du stockage d'état.
/// Utilise IDataProtector pour empêcher la falsification des données du wizard.
/// </summary>
public class TempDataRegistrationStateStore : IRegistrationStateStore
{
    /// <summary>Clé unique utilisée dans le dictionnaire TempData.</summary>
    private const string Key = "RegistrationState";

    private readonly IDataProtector _protector;

    /// <summary>
    /// Initialise une nouvelle instance du store.
    /// </summary>
    /// <param name="provider">Fournisseur de protection des données ASP.NET Core.</param>
    public TempDataRegistrationStateStore(IDataProtectionProvider provider)
    {
        // On crée un "protecteur" dédié à cette fonctionnalité.
        // Le nom "UniversitySystem.RegistrationState.v1" est une convention.
        _protector = provider.CreateProtector("UniversitySystem.RegistrationState.v1");
    }

    /// <inheritdoc />
    public void Save(RegistrationStateDto state, ITempDataDictionary tempData)
    {
        // 1. On sérialise le DTO en JSON
        var json = JsonSerializer.Serialize(state);

        // 2. On chiffre le JSON
        var protectedData = _protector.Protect(json);

        // 3. On stocke la chaîne chiffrée dans TempData
        tempData[Key] = protectedData;
    }

    /// <inheritdoc />
    public RegistrationStateDto? Load(ITempDataDictionary tempData)
    {
        // Si rien n'est stocké, on renvoie null
        if (!tempData.TryGetValue(Key, out var protectedDataObj) || protectedDataObj is not string protectedData)
            return null;

        try
        {
            // 1. On déchiffre
            var json = _protector.Unprotect(protectedData);

            // 2. On désérialise
            return JsonSerializer.Deserialize<RegistrationStateDto>(json);
        }
        catch
        {
            // Si le cookie a été modifié manuellement (CryptographicException),
            // le déchiffrement échoue → on retourne null pour la sécurité.
            return null;
        }
    }

    /// <inheritdoc />
    public void Clear(ITempDataDictionary tempData)
    {
        // On retire la clé pour effacer l'état (évite les rejeux).
        tempData.Remove(Key);
    }
}