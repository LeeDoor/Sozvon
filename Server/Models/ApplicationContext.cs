using Microsoft.EntityFrameworkCore;
namespace Server.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<ConferenceRoom> ConferenceRooms => Set<ConferenceRoom>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();


        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
