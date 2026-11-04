using Microsoft.EntityFrameworkCore;
using NEXO_EDU.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var connection = builder.Configuration.GetConnectionString("NexoEdu");
if (!string.IsNullOrWhiteSpace(connection))
    builder.Services.AddDbContext<NexoDbContext>(options => options.UseNpgsql(connection));

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// NEXO EDU es la página inicial, no el template Welcome de MVC.
app.MapControllerRoute(name: "default", pattern: "{controller=NexoDemo}/{action=Index}/{id?}");
app.Run();
