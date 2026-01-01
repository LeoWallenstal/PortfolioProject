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
                    ProfileImageUrl = "/cv/images/hero.png",
                    IsActive = true,
                    IsPrivate = false
                };

                var result = await userManager.CreateAsync(leo, "Leo12345!");
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var anna = await userManager.FindByNameAsync("anna");
            if (anna == null)
            {
                anna = new User
                {
                    UserName = "anna",
                    Email = "anna@demo.local",
                    FirstName = "Anna",
                    LastName = "Student",
                    IsActive = true,
                    IsPrivate = false
                };

                var result = await userManager.CreateAsync(anna, "Anna12345!");
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // ===== Alice =====
            var alice = await userManager.FindByNameAsync("alice");
            if (alice == null)
            {
                alice = new User
                {
                    UserName = "alice",
                    Email = "alice@demo.local",
                    FirstName = "Alice",
                    LastName = "Andersson",
                    IsActive = true,
                    IsPrivate = false
                };

                var result = await userManager.CreateAsync(alice, "Alice123!");
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // ===== Bob =====
            var bob = await userManager.FindByNameAsync("bob");
            if (bob == null)
            {
                bob = new User
                {
                    UserName = "bob",
                    Email = "bob@demo.local",
                    FirstName = "Bob",
                    LastName = "Berg",
                    IsActive = true,
                    IsPrivate = true   // private profile example
                };

                var result = await userManager.CreateAsync(bob, "Bob123!");
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // ===== Clara =====
            var clara = await userManager.FindByNameAsync("clara");
            if (clara == null)
            {
                clara = new User
                {
                    UserName = "clara",
                    Email = "clara@demo.local",
                    FirstName = "Clara",
                    LastName = "Carlsson",
                    IsActive = true,
                    IsPrivate = false
                };

                var result = await userManager.CreateAsync(clara, "Clara123!");
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // ===== David =====
            var david = await userManager.FindByNameAsync("david");
            if (david == null)
            {
                david = new User
                {
                    UserName = "david",
                    Email = "david@demo.local",
                    FirstName = "David",
                    LastName = "Dahl",
                    IsActive = false,   // inactive account example
                    IsPrivate = false
                };

                var result = await userManager.CreateAsync(david, "David123!");
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Prevent duplicating seed data
            if (await db.Cvs.AnyAsync() || await db.Messages.AnyAsync())
                return;

            // --- CVs (FK to users) ---
            var adminCv = new Cv { UserId = admin.Id, ViewCount = 0 };

            var leoCv = new Cv
            {
                UserId = leo.Id,
                ViewCount = 0,
                Title = "Systemvetenskap Student",
                Summary = "I'm a frontend web developer dedicated to turning ideas into creative solutions. I specialize in creating seamless and intuitive user experiences.\r\n\t\t\t" +
                "My approach focuses on creating scalable, high-performing solutions tailored to both user needs and business objectives. By prioritizing performance, accessibility, " +
                "and responsiveness, I strive to deliver experiences that not only engage users but also drive tangible results.\r\n\t\t"
            };

            // Same-ish CV template for all demo users (you can tweak later)
            Cv MakeCv(User u)
            {
                return new Cv
                {
                    UserId = u.Id,
                    ViewCount = 0,
                    Title = "Exemepel på titel",
                    Summary =
                        "I'm a frontend web developer dedicated to turning ideas into creative solutions. " +
                        "I specialize in creating seamless and intuitive user experiences. " +
                        "My approach focuses on performance, accessibility, and responsiveness."
                };
            }

            var aliceCv = MakeCv(alice);
            var bobCv = MakeCv(bob);
            var claraCv = MakeCv(clara);
            var davidCv = MakeCv(david);


            db.Cvs.AddRange(adminCv, leoCv, aliceCv, bobCv, claraCv, davidCv);

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
            edu1.Cvs.Add(aliceCv);
            edu1.Cvs.Add(bobCv);
            edu2.Cvs.Add(claraCv);
            edu2.Cvs.Add(davidCv);
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
            exp2.Cvs.Add(aliceCv);
            exp2.Cvs.Add(bobCv);
            exp1.Cvs.Add(claraCv);
            exp1.Cvs.Add(davidCv);
            exp2.Cvs.Add(leoCv);

            // --- SKILLS ---
            var skill1 = new Skill { Name = "C#", ImageUrl = "/cv/images/Logo_C_sharp.svg" };
            var skill2 = new Skill { Name = "ASP.NET Core", ImageUrl = "/cv/images/NET_Core_Logo.svg" };
            var skill3 = new Skill { Name = "JavaScript", ImageUrl = "/cv/images/Unofficial_JavaScript_logo_2.svg" };
            var skill4 = new Skill { Name = "React", ImageUrl = "/cv/images/React-icon.svg" };

            skill1.Cvs.Add(adminCv);
            skill2.Cvs.Add(adminCv);

            skill3.Cvs.Add(leoCv);
            skill4.Cvs.Add(leoCv);

            skill3.Cvs.Add(aliceCv);
            skill4.Cvs.Add(aliceCv);

            skill2.Cvs.Add(bobCv);
            skill4.Cvs.Add(bobCv);

            skill1.Cvs.Add(claraCv);
            skill2.Cvs.Add(claraCv);

            skill2.Cvs.Add(davidCv);
            skill3.Cvs.Add(davidCv);

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


            // Use a single base time to keep ordering stable
            var t0 = DateTime.UtcNow.AddMinutes(-30);

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
                },
                new Message
                {
                    FromUserId = admin.Id,
                    ToUserId = anna.Id,
                    Body = "Hello!",
                    SentAt = DateTime.UtcNow.AddMinutes(2),
                    IsRead = false
                },
                // ===== Conversation: DAVID <-> ADMIN (requested) =====
                new Message
                {
                    FromUserId = david.Id,
                    ToUserId = admin.Id,
                    Body = "Hey admin, I think my profile is marked inactive by mistake. Can you check?",
                    SentAt = t0.AddMinutes(0),
                    IsRead = true
                },
                new Message
                {
                    FromUserId = admin.Id,
                    ToUserId = david.Id,
                    Body = "Hi David! I can see your account is inactive in the demo seed. Want me to activate it?",
                    SentAt = t0.AddMinutes(2),
                    IsRead = false
                },
                new Message
                {
                    FromUserId = david.Id,
                    ToUserId = admin.Id,
                    Body = "Yes please — also, does being private affect messaging?",
                    SentAt = t0.AddMinutes(4),
                    IsRead = false
                },
                new Message
                {
                    FromUserId = admin.Id,
                    ToUserId = david.Id,
                    Body = "Private affects profile visibility, not direct messages (in our demo rules).",
                    SentAt = t0.AddMinutes(6),
                    IsRead = false
                },

                // ===== Conversation: ALICE <-> ADMIN =====
                new Message
                {
                    FromUserId = admin.Id,
                    ToUserId = alice.Id,
                    Body = "Welcome Alice! Try sending a message back to test the inbox.",
                    SentAt = t0.AddMinutes(10),
                    IsRead = true
                },
                new Message
                {
                    FromUserId = alice.Id,
                    ToUserId = admin.Id,
                    Body = "Thanks! This UI is super clean 👌",
                    SentAt = t0.AddMinutes(12),
                    IsRead = false
                },

                // ===== Conversation: BOB <-> ADMIN =====
                new Message
                {
                    FromUserId = bob.Id,
                    ToUserId = admin.Id,
                    Body = "Hi! My profile is private — can you still see my CV?",
                    SentAt = t0.AddMinutes(14),
                    IsRead = false
                },
                new Message
                {
                    FromUserId = admin.Id,
                    ToUserId = bob.Id,
                    Body = "As admin I can, but other users might be blocked depending on your privacy logic.",
                    SentAt = t0.AddMinutes(15),
                    IsRead = false
                },

                // ===== Conversation: CLARA <-> ADMIN =====
                new Message
                {
                    FromUserId = admin.Id,
                    ToUserId = clara.Id,
                    Body = "Hi Clara! Reminder: update your summary and add a project.",
                    SentAt = t0.AddMinutes(18),
                    IsRead = false
                }
                );

            await db.SaveChangesAsync();
        }
    }

}
