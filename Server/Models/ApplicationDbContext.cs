using Microsoft.EntityFrameworkCore;
namespace Server.Models
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<ConferenceRoom> ConferenceRooms => Set<ConferenceRoom>();

        

    }
}
