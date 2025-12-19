using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortfolioProject.Data;
using PortfolioProject.Models;

namespace PortfolioProject
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // apply migrations automatically (dev only is fine)
            await db.Database.MigrateAsync();

            // --- USERS ---
            var admin = await userManager.FindByNameAsync("admin");
            if (admin == null)
            {
                admin = new User
                {
                    UserName = "admin",
                    Email = "admin@demo.local",
                    FirstName = "Ada",
                    LastName = "Admin",
                    IsActive = true,
                    IsPrivate = false
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var leo = await userManager.FindByNameAsync("leo");
            if (leo == null)
            {
                leo = new User
                {
                    UserName = "leo",
                    Email = "leo@demo.local",
                    FirstName = "Leo",
                    LastName = "Student",
                    IsActive = true,
                    IsPrivate = false
                };

                var result = await userManager.CreateAsync(leo, "Leo12345!");
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Prevent duplicating seed data
            if (await db.Cvs.AnyAsync() || await db.Messages.AnyAsync())
                return;

            // --- CVs (FK to users) ---
            var adminCv = new Cv { UserId = admin.Id };
            var leoCv = new Cv { UserId = leo.Id };

            db.Cvs.AddRange(adminCv, leoCv);

            // --- Projects ---
            var p1 = new Project { Title = "Portfolio Website", Description = "MVC + EF Core" };
            var p2 = new Project { Title = "Messaging System", Description = "User-to-user chat" };

            // many-to-many user<->project (if your model has it)
            p1.Users.Add(leo);
            p2.Users.Add(leo);
            p2.Users.Add(admin);

            db.Projects.AddRange(p1, p2);

            // --- Messages ---
            db.Messages.AddRange(
                new Message
                {
                    FromUserId = admin.Id,
                    ToUserId = leo.Id,
                    Body = "Welcome to the platform!",
                    SentAt = DateTime.UtcNow,
                    IsRead = false
                },
                new Message
                {
                    FromUserId = leo.Id,
                    ToUserId = admin.Id,
                    Body = "Thanks!",
                    SentAt = DateTime.UtcNow.AddMinutes(2),
                    IsRead = false
                }
            );

            await db.SaveChangesAsync();
        }
    }

}
