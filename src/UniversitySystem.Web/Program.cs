using Hangfire;
using Hangfire.InMemory;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Infrastructure.Persistence;
using UniversitySystem.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. Base de données — MariaDB via Pomelo
// ============================================================
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default manquante.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var serverVersion = builder.Environment.IsDevelopment()
        ? ServerVersion.AutoDetect(connectionString)
        : new MariaDbServerVersion(new Version(10, 4, 32));

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
// 4. Autorisation — FallbackPolicy (toute page exige d'être authentifié)
// ============================================================
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// ============================================================
// 5. Hangfire (InMemory pour M0 — suffisant pour le développement)
// ============================================================
builder.Services.AddHangfire(config =>
{
    config.UseInMemoryStorage();
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
    config.UseSimpleAssemblyNameTypeSerializer();
    config.UseRecommendedSerializerSettings();
});

builder.Services.AddHangfireServer();

var app = builder.Build();

// ============================================================
// 6. Pipeline HTTP
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
app.UseAuthorization();

// Dashboard Hangfire (uniquement en Development)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new UniversitySystem.Web.Infrastructure.HangfireAuthorizationFilter() },
        DashboardTitle = "UnivSystem - Jobs"
    });
}

// ============================================================
// 7. DataSeeder (uniquement en Development)
// ============================================================
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db);
}

// ============================================================
// 8. Routes
// ============================================================
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();