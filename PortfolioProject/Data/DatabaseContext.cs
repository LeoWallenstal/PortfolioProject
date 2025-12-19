using Microsoft.EntityFrameworkCore;
using PortfolioProject.Models;

namespace PortfolioProject.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        // Define your DbSets here
        // public DbSet<YourEntity> YourEntities { get; set; }
        //public DbSet<Project> Projects { get; set; }
        //public DbSet<Cv> Cvs { get; set; }
        //public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cv> Cvs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure your entity mappings here if needed
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    FirstName = "Leo",
                    LastName = "Wallenstål",
                    UserName = "admin",
                    PasswordHash = "password", // In a real application, passwords should be hashed and salted
                    Email = "admin@portfolio.se"
                },
                new User
                {
                    FirstName = "Anna",
                    LastName = "Andersson",
                    UserName = "user",
                    PasswordHash = "password", // In a real application, passwords should be hashed and salted
                    Email = "user@portfolio.se"
                }
            );

            modelBuilder.Entity<Cv>().HasData(
                new Cv
                {
                    UserId = "5acbd350-28b7-4cb6-94bf-42dd86c53af2" // Assuming this is the ID for Anna Andersson
                },
                new Cv
                {
                    UserId = "5211872b-c0d1-4904-94ef-b7ee174a9d32" // Assuming this is the ID for Leo Wallenstål
                }
            );

        }
    }
}
