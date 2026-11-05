using System.Security.Claims;

namespace UniversitySystem.Web.Middleware;

/// <summary>
/// Middleware de sécurité appliquant la règle de gestion RG-04.
/// Si un utilisateur avec le rôle "enseignant" a un mot de passe temporaire
/// (claim "MustChangePassword" == "true"), il est redirigé de force vers
/// la page de setup dédiée à l'enseignant, sauf sur les pages autorisées.
/// </summary>
public class TeacherPasswordSetupMiddleware
{
    private readonly RequestDelegate _next;

    public TeacherPasswordSetupMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var role = context.User.FindFirstValue(ClaimTypes.Role);
            var mustChange = context.User.FindFirstValue("MustChangePassword");

            // Condition RG-04 : Enseignant + Mot de passe temporaire
            if (string.Equals(role, "enseignant", StringComparison.OrdinalIgnoreCase) 
                && string.Equals(mustChange, "true", StringComparison.OrdinalIgnoreCase))
            {
                var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

                // Liste blanche : autorise la page de setup de l'enseignant
                var isAllowedPath = 
                    path.StartsWith("/enseignant/setup") ||       // <-- Page de setup enseignant
                    path.StartsWith("/account/changepassword") || 
                    path.StartsWith("/account/logout") || 
                    path.StartsWith("/account/accessdenied") ||
                    path.StartsWith("/favicon.ico") ||
                    path.StartsWith("/css") || 
                    path.StartsWith("/js") || 
                    path.StartsWith("/lib");

                if (!isAllowedPath)
                {
                    // Redirection forcée vers le SETUP ENSEIGNANT (pas /Account/ChangePassword)
                    context.Response.Redirect("/Enseignant/Setup");
                    return;
                }
            }
        }

        await _next(context);
    }
}