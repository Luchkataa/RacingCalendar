using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RacingCalendar.Services.Core;
using RacingCalendar.Services.Core.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICircuitService, CircuitService>();
builder.Services.AddScoped<ISeriesService, SeriesService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IRaceService, RaceService>();

builder.Services.AddDbContext<RacingCalendarDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<RacingCalendarDbContext>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RolesSeeder.SeedRolesAndAdminAsync(services);
}

app.Run();
