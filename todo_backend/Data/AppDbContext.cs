using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using todo_backend.Models;

namespace todo_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<TimelineActivity> TimelineActivities { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<ActivityMembers> ActivityMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //klucz zlozony user - friendship
            modelBuilder.Entity<Friendship>()
                .HasKey(f => new { f.UserId, f.FriendId });

            
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User)
                .WithMany(u => u.Friendships) 
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany()
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.NoAction);



            //unikalnosc kategorii w obrębie użytkownika
            // gdy usuwamy użytkownika to usuwają się jego wszystkie kategorie oraz aktywności
            modelBuilder.Entity<Category>()
                .HasIndex(c => new { c.UserId, c.Name })
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TimelineActivity>()
                .HasOne(a => a.User)
                .WithMany(u => u.TimelineActivities)
                .HasForeignKey(a => a.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TimelineActivity>()
                .HasOne(a => a.Category)
                .WithMany(c => c.TimelineActivities)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            //klucz do aktywnosci z znajomymi
            modelBuilder.Entity<ActivityMembers>()
                .HasKey(am => new { am.ActivityId, am.UserId });


            // Zaproszenia do aktywnosci usuwane razem z aktywnością
            //modelBuilder.Entity<ActivityMembers>()
            //.HasOne(am => am.Activity)
            //.WithMany(a => a.ActivityMembers)
            //.HasForeignKey(am => am.ActivityId)
            //.OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
