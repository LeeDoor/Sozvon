using Microsoft.EntityFrameworkCore;

namespace Server.Models.Data.Services
{
    public class ConferenceRoomService
    {
        private readonly ApplicationContext _context;

        public ConferenceRoomService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<ConferenceRoom> CreateRoomAsync(string name)
        {
            var room = new ConferenceRoom
            {
                Name = name,
                Link = GenerateRoomLink(),
                IsActive = true
            };

            _context.ConferenceRooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<ConferenceRoom?> GetRoomWithUsersAsync(ConferenceRoomId roomId)
        {
            return await _context.ConferenceRooms
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task AddUserToRoomAsync(ConferenceRoomId roomId, UserId userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null)
            {
                user.ConferenceRoomId = roomId;
                await _context.SaveChangesAsync();
            }
        }

        private static string GenerateRoomLink()
        {
            return Guid.NewGuid().ToString("N")[..10];
        }
    }
}
