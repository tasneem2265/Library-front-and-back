// Data/SeedData.cs
public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        // Roles
        string[] roles = { "Admin", "Customer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Admin User
        if (await userManager.FindByEmailAsync("admin@library.com") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@library.com",
                Email = "admin@library.com",
                FullName = "مديرة المكتبة",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin@123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Categories
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "روايات", Description = "روايات عربية وعالمية" },
                new Category { Name = "علمي", Description = "كتب علمية وتقنية" },
                new Category { Name = "أطفال", Description = "قصص وكتب للأطفال" },
                new Category { Name = "ديني", Description = "كتب دينية وإسلامية" },
                new Category { Name = "تاريخ", Description = "كتب تاريخية" }
            );
            await context.SaveChangesAsync();
        }
    }
}