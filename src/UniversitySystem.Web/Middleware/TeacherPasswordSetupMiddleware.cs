using System.Security.Claims;

namespace UniversitySystem.Web.Middleware;

/// <summary>
/// Middleware de sécurité appliquant la règle de gestion RG-04.
/// Si un utilisateur avec le rôle "enseignant" a un mot de passe temporaire
/// (claim "MustChangePassword" == "true"), il est redirigé de force vers
/// la page de changement de mot de passe, sauf sur les pages autorisées.
/// </summary>
public class TeacherPasswordSetupMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="TeacherPasswordSetupMiddleware"/>.
    /// </summary>
    /// <param name="next">Le délégué suivant dans le pipeline HTTP.</param>
    public TeacherPasswordSetupMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Intercepte la requête HTTP pour vérifier les claims de l'utilisateur.
    /// </summary>
    /// <param name="context">Le contexte HTTP courant.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // On ne fait ça que si l'utilisateur est authentifié
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var role = context.User.FindFirstValue(ClaimTypes.Role);
            var mustChange = context.User.FindFirstValue("MustChangePassword");

            // Condition RG-04 : Enseignant + Mot de passe temporaire
            if (string.Equals(role, "enseignant", StringComparison.OrdinalIgnoreCase) 
                && string.Equals(mustChange, "true", StringComparison.OrdinalIgnoreCase))
            {
                var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

                // Liste blanche des URLs accessibles même avec un mot de passe temporaire
                var isAllowedPath = 
                    path.StartsWith("/account/changepassword") || 
                    path.StartsWith("/account/logout") || 
                    path.StartsWith("/account/accessdenied") ||
                    path.StartsWith("/favicon.ico") ||
                    path.StartsWith("/css") || 
                    path.StartsWith("/js") || 
                    path.StartsWith("/lib");

                if (!isAllowedPath)
                {
                    // Redirection forcée vers le formulaire de changement de mot de passe
                    context.Response.Redirect("/Account/ChangePassword");
                    return; // On coupe le pipeline ici
                }
            }
        }

        // Si les conditions ne sont pas remplies ou si on est sur une page autorisée
        await _next(context);
    }
}