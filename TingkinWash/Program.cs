
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TingkinWash.Data;
using TingkinWash.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC and Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Login settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

var app = builder.Build();

// CREATE ROLES AND ADMIN ACCOUNT
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();

    // Create roles
    string[] roles = { "Admin", "Customer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Get Admin credentials from User Secrets
    var adminEmail = builder.Configuration["AdminSeed:Email"];
    var adminPassword = builder.Configuration["AdminSeed:Password"];

    if (string.IsNullOrWhiteSpace(adminEmail) ||
        string.IsNullOrWhiteSpace(adminPassword))
    {
        throw new Exception("Admin credentials are missing from User Secrets.");
    }

    // Find existing Admin account
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    // Create Admin account if it does not exist
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "TingkinWash",
            LastName = "Admin"
        };

        var createResult = await userManager.CreateAsync(
            adminUser,
            adminPassword
        );

        if (!createResult.Succeeded)
        {
            throw new Exception(string.Join(", ",
                createResult.Errors.Select(e => e.Description)));
        }
    }

    // Assign Admin role
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        var roleResult = await userManager.AddToRoleAsync(
            adminUser, "Admin");

        if (!roleResult.Succeeded)
        {
            throw new Exception(string.Join(", ",
                roleResult.Errors.Select(e => e.Description)));
        }
    }

    // Remove Customer role if this account previously had it
    if (await userManager.IsInRoleAsync(adminUser, "Customer"))
    {
        await userManager.RemoveFromRoleAsync(adminUser, "Customer");
    }
}

// HTTP pipeline
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

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
