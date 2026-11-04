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

// 1. Base de données
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default manquante.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    options.UseMySql(connectionString, serverVersion);
});

// 2. MVC
builder.Services.AddControllersWithViews();

// 3. Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});
builder.Services.AddHttpContextAccessor();

// 4. Authentification Cookie
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

// 5. Autorisation
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// 6. Services applicatifs
builder.Services.AddMemoryCache();
builder.Services.AddDataProtection();
builder.Services.AddScoped<IRegistrationStateStore, SessionRegistrationStateStore>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IEligibilityService, EligibilityService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IMatriculeService, MatriculeService>();
// Services de paiement
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Gateway (Fake en dev, Real en prod)
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IPaymentGatewayClient, FakePaymentGateway>();
}
else
{
    builder.Services.AddScoped<IPaymentGatewayClient, RealPaymentGateway>();
}

// Autorisation dynamique
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// 7. Hangfire
builder.Services.AddHangfire(config =>
{
    config.UseInMemoryStorage();
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
    config.UseSimpleAssemblyNameTypeSerializer();
    config.UseRecommendedSerializerSettings();
});
builder.Services.AddHangfireServer();

var app = builder.Build();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseMiddleware<TeacherPasswordSetupMiddleware>(); 
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new UniversitySystem.Web.Infrastructure.HangfireAuthorizationFilter() },
    DashboardTitle = "UnivSys - Jobs"
});

// DataSeeder
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db);
}

// Routes
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();