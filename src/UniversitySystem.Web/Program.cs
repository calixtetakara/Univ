using Hangfire;
using Hangfire.InMemory;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Infrastructure.Persistence.Seed;
using UniversitySystem.Infrastructure.Services;
using UniversitySystem.Web.Authorization;
using UniversitySystem.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. Base de données — MariaDB via Pomelo
// ============================================================
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default manquante.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Utilisation de ServerVersion.AutoDetect (compatible .NET 10)
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    options.UseMySql(connectionString, serverVersion);
});

// ============================================================
// 2. MVC
// ============================================================
builder.Services.AddControllersWithViews();

// ============================================================
// 3. Authentification Cookie
// ============================================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = "Univ.Auth";
    });

// ============================================================
// 4. Autorisation — FallbackPolicy
// ============================================================
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// ============================================================
// 5. Services applicatifs (Auth + Permissions)
// ============================================================
builder.Services.AddMemoryCache();

// Ajout pour le Wizard d'inscription (M2-01)
builder.Services.AddDataProtection();
builder.Services.AddScoped<IRegistrationStateStore, TempDataRegistrationStateStore>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// Autorisation dynamique par permission
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// ============================================================
// 6. Hangfire (InMemory)
// ============================================================
builder.Services.AddHangfire(config =>
{
    config.UseInMemoryStorage();
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
    config.UseSimpleAssemblyNameTypeSerializer();
    config.UseRecommendedSerializerSettings();
});

builder.Services.AddHangfireServer();

// ============================================================
// Construction de l'application
// ============================================================
var app = builder.Build();

// ============================================================
// Pipeline HTTP
// ============================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();

// Application de la règle RG-04 (enseignant avec mot de passe temporaire)
app.UseMiddleware<UniversitySystem.Web.Middleware.TeacherPasswordSetupMiddleware>(); 

app.UseAuthorization();

// Dashboard Hangfire
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new UniversitySystem.Web.Infrastructure.HangfireAuthorizationFilter() },
    DashboardTitle = "UnivSystem - Jobs"
});

// ============================================================
// DataSeeder (au démarrage)
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Application des migrations (uniquement si nécessaire)
    await db.Database.MigrateAsync();
    
    // Lancement du seeder (idempotent)
    await DataSeeder.SeedAsync(db);
}

// ============================================================
// Routes
// ============================================================
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();