using LMSPlatform.Models;
using Microsoft.AspNetCore.Identity;

namespace LMSPlatform.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            string[] roles = { "Admin", "Egitmen", "Ogrenci" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var adminEmail = "admin@lms.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Ad = "Site",
                    Soyad = "Admin",
                    JetonMiktari = 9999,
                    EgitmenOnaylandi = true
                };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (!context.Kategoriler.Any())
            {
                context.Kategoriler.AddRange(
                    new Kategori { Ad = "Programlama", Aciklama = "Yazılım geliştirme ve kodlama kursları" },
                    new Kategori { Ad = "Tasarım", Aciklama = "Grafik ve UI/UX tasarım kursları" },
                    new Kategori { Ad = "Pazarlama", Aciklama = "Dijital pazarlama ve SEO kursları" },
                    new Kategori { Ad = "Kişisel Gelişim", Aciklama = "Liderlik ve kişisel gelişim kursları" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
