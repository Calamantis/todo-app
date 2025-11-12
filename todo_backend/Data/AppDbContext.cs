using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using todo_backend.Models;
using Activity = todo_backend.Models.Activity;

namespace todo_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ActivityMembers> ActivityMembers { get; set; }

        public DbSet<BlockedUsers>  BlockedUsers { get; set; }

        public DbSet<Statistics> Statistics { get; set; }

        public DbSet<Notification> Notification { get; set; }


        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityInstance> ActivityInstances { get; set; }
        public DbSet<ActivityRecurrenceRule> ActivityRecurrenceRules { get; set; }

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

            modelBuilder.Entity<Activity>()
              .HasOne(a => a.Owner)
              .WithMany(u => u.Activities)
              .HasForeignKey(a => a.OwnerId)
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Category)
                .WithMany(c => c.TimelineActivities)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Activity>()
            .HasMany(a => a.RecurrenceRules)
            .WithOne(r => r.Activity)
            .HasForeignKey(r => r.ActivityId)
            .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<ActivityRecurrenceRule>()
                .HasMany(r => r.Instances)
                .WithOne(i => i.RecurrenceRule)
                .HasForeignKey(i => i.RecurrenceRuleId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<Activity>()
                .HasMany(a => a.Instances)
                .WithOne(i => i.Activity)
                .HasForeignKey(i => i.ActivityId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<ActivityInstance>()
                .HasOne(i => i.RecurrenceRule)
                .WithMany()
                .HasForeignKey(i => i.RecurrenceRuleId)
                .OnDelete(DeleteBehavior.NoAction); 



            //klucz do aktywnosci z znajomymi
            //modelBuilder.Entity<ActivityMembers>()
            //    .HasKey(am => new { am.ActivityId, am.UserId });


            // Zaproszenia do aktywnosci
            //modelBuilder.Entity<ActivityMembers>()
            //.HasOne(am => am.Activity)
            //.WithMany(a => a.ActivityMembers)
            //.HasForeignKey(am => am.ActivityId)
            //.OnDelete(DeleteBehavior.NoAction);




















            //Blokowanie
            modelBuilder.Entity<BlockedUsers>()
                .HasKey(b => new { b.UserId, b.BlockedUserId });

            // Relacja User -> BlockedUsers (użytkownik blokuje innych)
            modelBuilder.Entity<BlockedUsers>()
                .HasOne(b => b.User)
                .WithMany(u => u.BlockedUsers)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Relacja do zablokowanego użytkownika (opcjonalna nawigacja)
            modelBuilder.Entity<BlockedUsers>()
                .HasOne(b => b.BlockedUser)
                .WithMany()
                .HasForeignKey(b => b.BlockedUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Statystyka
            modelBuilder.Entity<Statistics>()
                .HasOne(s => s.User)
                .WithMany(u => u.Statistics) // musisz dodać ICollection<Statistics> Statistics w User
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction); // jeśli chcesz usuwać statystyki przy usuwaniu użytkownika

            // Unikalne kody dołączania do aktywości dla nie-znajomych
            modelBuilder.Entity<Models.Activity>()
                .HasIndex(a => a.JoinCode)
                .IsUnique();

            //Role jako string
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            //Unikalny email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Alert i reminders
            modelBuilder.Entity<Notification>()
                .HasOne(r => r.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
