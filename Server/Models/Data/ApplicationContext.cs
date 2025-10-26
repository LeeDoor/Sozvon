using Microsoft.EntityFrameworkCore;
namespace Server.Models.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<ConferenceRoom> ConferenceRooms => Set<ConferenceRoom>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

        private static ApplicationContext instance;
        public static ApplicationContext Instance => instance ?? (instance = new ApplicationContext());
        private ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasData(
            //    new User { Id = 1, Login = "Boris", Name = "Boris", Password = "1234" },
            //    new User { Id = 2, Login = "Leonid", Name = "Leonid", Password = "1234" },
            //    new User { Id = 3, Login = "Egor", Name = "Egor", Password = "1234" },
            //    new User { Id = 4, Login = "Seva", Name = "Seva", Password = "1234" }
            //    );
            modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
            modelBuilder.Entity<User>()
                .HasOne(u => u.ConferenceRoom)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.ConferenceRoomId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<ChatMessage>()
                .HasOne(ch => ch.ConferenceRoom)
                .WithMany(c => c.ChatMessages)
                .HasForeignKey(ch => ch.ConferenceRoomId);
            modelBuilder.Entity<ChatMessage>()
                .HasOne(ch => ch.User)
                .WithMany()
                .HasForeignKey(ch => ch.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
