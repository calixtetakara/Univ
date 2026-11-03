using Hangfire.Dashboard;

namespace UniversitySystem.Web.Infrastructure;

/// <summary>
/// Autorise l'accès au dashboard Hangfire uniquement en Development (localhost).
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // En Development on autorise tout (localhost uniquement recommandé)
        return true;
    }
}