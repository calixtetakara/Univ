using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using UniversitySystem.Application.DTOs;

namespace UniversitySystem.Infrastructure.Services;

/// <summary>
/// Stockage de l'état du wizard via la Session serveur.
/// Plus robuste que TempData pour les parcours multi-étapes.
/// </summary>
public class SessionRegistrationStateStore : IRegistrationStateStore
{
    private const string SessionKey = "RegistrationState";
    private readonly IDataProtector _protector;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initialise le service avec le fournisseur de protection et l'accès au contexte HTTP.
    /// </summary>
    public SessionRegistrationStateStore(
        IDataProtectionProvider provider,
        IHttpContextAccessor httpContextAccessor)
    {
        _protector = provider.CreateProtector("UniversitySystem.RegistrationState.v1");
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public void Save(RegistrationStateDto state)
    {
        var json = JsonSerializer.Serialize(state);
        var protectedData = _protector.Protect(json);
        // Stockage dans la session (côté serveur)
        _httpContextAccessor.HttpContext!.Session.SetString(SessionKey, protectedData);
    }

    /// <inheritdoc />
    public RegistrationStateDto? Load()
    {
        var protectedData = _httpContextAccessor.HttpContext!.Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(protectedData)) return null;

        try
        {
            var json = _protector.Unprotect(protectedData);
            return JsonSerializer.Deserialize<RegistrationStateDto>(json);
        }
        catch
        {
            // Si le cookie/session a été altéré, on renvoie null (sécurité)
            return null;
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        _httpContextAccessor.HttpContext!.Session.Remove(SessionKey);
    }
}