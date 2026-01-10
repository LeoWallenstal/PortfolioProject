using DataLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Data
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Cv> Cvs { get; set; }
        public DbSet<CvVisit> CvVisits { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure your entity mappings here if needed

            modelBuilder.Entity<CvVisit>()
                .HasIndex(v => new { v.CvId, v.VisitorId })
                .IsUnique();

            modelBuilder.Entity<CvVisit>()
                .HasOne(v => v.Cv)
                .WithMany()
                .HasForeignKey(v => v.CvId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CvVisit>()
                .HasOne(v => v.Visitor)
                .WithMany()
                .HasForeignKey(v => v.VisitorId)
                .OnDelete(DeleteBehavior.NoAction);
        

        modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);

            modelBuilder.Entity<Message>()
                .HasOne<User>(m => m.FromUser)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.FromUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Message>()
                .HasOne<User>(m => m.ToUser)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ToUserId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.UserA)
                .WithMany()
                .HasForeignKey(c => c.UserAId);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.UserB)
                .WithMany()
                .HasForeignKey(c => c.UserBId);

            modelBuilder.Entity<Conversation>()
                .HasIndex(c => new { c.UserAId, c.UserBId })
                .IsUnique();


            modelBuilder.Entity<Project>()
                .HasOne<User>(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Cv>()
            //    .HasOne<User>(c => c.User)
            //    .WithMany()
            //    .HasForeignKey(c => c.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Cv>()
            //    .HasMany(c => c.Skills)
            //    .WithMany(s => s.Cvs)
            //    .UsingEntity(j => j.ToTable("CvSkills")); // optional: specify join table name

            //modelBuilder.Entity<Cv>()
            //    .HasMany(c => c.Educations)
            //    .WithMany(e => e.Cvs)
            //    .UsingEntity(j => j.ToTable("CvEducations"));

            //modelBuilder.Entity<Cv>()
            //    .HasMany(c => c.Experiences)
            //    .WithMany(e => e.Cvs)
            //    .UsingEntity(j => j.ToTable("CvExperiences"));


            //Appliceras på alla EF Core-frågor mot User, filtrerar bort inaktiva användare automatiskt
            //Vill vi ha denna??
            /*modelBuilder.Entity<User>()
                .HasQueryFilter(u => u.IsActive);*/
        }
    }
}