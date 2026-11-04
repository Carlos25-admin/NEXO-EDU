using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NEXO_EDU.Data;
using NEXO_EDU.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.Name = "NexoEdu.Progress.Dev";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

var demoLocal = builder.Environment.IsDevelopment()
    && builder.Configuration.GetValue<bool>("DemoMode:Enabled");

var connection = builder.Configuration.GetConnectionString("NexoEdu");
if (demoLocal)
{
    builder.Services.AddAuthentication("NexoDemoLocal")
        .AddCookie("NexoDemoLocal", options =>
        {
            options.LoginPath = "/Cuenta/Ingresar";
            options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
            options.Cookie.Name = "NexoEdu.Dev";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.ExpireTimeSpan = TimeSpan.FromHours(2);
            options.SlidingExpiration = true;
        });
}
else
{
    if (string.IsNullOrWhiteSpace(connection))
        throw new InvalidOperationException("Para usar Identity se requiere ConnectionStrings:NexoEdu. En desarrollo puedes habilitar DemoMode:Enabled.");

    builder.Services.AddDbContext<NexoDbContext>(options => options.UseNpgsql(connection));
    builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    }).AddEntityFrameworkStores<NexoDbContext>().AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Cuenta/Ingresar";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });
}

var app = builder.Build();
if (!demoLocal && app.Environment.IsDevelopment()
    && app.Configuration.GetValue<bool>("DemoUsers:Initialize"))
{
    await DemoIdentitySeeder.InitializeAsync(app.Services, app.Configuration);
}
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
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=NexoDemo}/{action=Index}/{id?}");
app.Run();
