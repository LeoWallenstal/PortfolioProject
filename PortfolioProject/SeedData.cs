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
            var adminCv = new Cv { UserId = admin.Id , ViewCount = 0};
            var leoCv = new Cv { UserId = leo.Id, ViewCount = 0};

            db.Cvs.AddRange(adminCv, leoCv);

            // --- EDUCATIONS ---
            var edu1 = new Education
            {
                School = "Linnéuniversitetet",
                Degree = "BSc Computer Science",
                StartYear = new DateTime(2020, 1, 1),
                EndYear = new DateTime(2023, 1, 1)
            };

            var edu2 = new Education
            {
                School = "Yrgo",
                Degree = "Frontend Developer",
                StartYear = new DateTime(2021, 1, 1),
                EndYear = new DateTime(2023, 1, 1)
            };

            edu1.Cvs.Add(adminCv);
            edu2.Cvs.Add(leoCv);

            // --- EXPERIENCES ---
            var exp1 = new Experience
            {
                Company = "Spotify",
                Role = "Backend Developer",
                StartYear = new DateTime(2023, 1, 1),
                EndYear = DateTime.UtcNow
            };

            var exp2 = new Experience
            {
                Company = "Volvo",
                Role = "Frontend Developer",
                StartYear = new DateTime(2022, 1, 1),
                EndYear = new DateTime(2024, 1, 1)
            };

            exp1.Cvs.Add(adminCv);
            exp2.Cvs.Add(leoCv);

            // --- SKILLS ---
            var skill1 = new Skill { Name = "C#" };
            var skill2 = new Skill { Name = "ASP.NET Core" };
            var skill3 = new Skill { Name = "JavaScript" };
            var skill4 = new Skill { Name = "React" };

            skill1.Cvs.Add(adminCv);
            skill2.Cvs.Add(adminCv);

            skill3.Cvs.Add(leoCv);
            skill4.Cvs.Add(leoCv);

            // --- SAVE ALL ---
            db.Educations.AddRange(edu1, edu2);
            db.Experiences.AddRange(exp1, exp2);
            db.Skills.AddRange(skill1, skill2, skill3, skill4);

            await db.SaveChangesAsync();

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
