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

        public async Task<bool> DeleteRoomAsync(ConferenceRoomId roomId)
        {
            var room = await _context.ConferenceRooms.FindAsync(roomId);

            if (room != null)
            {
                _context.ConferenceRooms.Remove(room);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
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

        public async Task RemoveUserFromRoomAsync(UserId userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null && user.ConferenceRoomId.HasValue)
            {
                user.ConferenceRoomId = null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> RemoveUserFromRoomAsync(ConferenceRoomId roomId, UserId userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null && user.ConferenceRoomId == roomId)
            {
                user.ConferenceRoomId = null;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task RemoveAllUsersFromRoomAsync(ConferenceRoomId roomId)
        {
            var usersInRoom = await _context.Users
                .Where(u => u.ConferenceRoomId == roomId)
                .ToListAsync();

            foreach (var user in usersInRoom)
            {
                user.ConferenceRoomId = null;
            }

            await _context.SaveChangesAsync();
        }




        public async Task<ConferenceRoom?> GetRoomByIdAsync(ConferenceRoomId roomId)
        {
            return await _context.ConferenceRooms
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task<ConferenceRoom?> GetRoomByIdWithUsersAsync(ConferenceRoomId roomId)
        {
            return await _context.ConferenceRooms
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task<ConferenceRoom?> GetRoomByLinkAsync(string link)
        {
            return await _context.ConferenceRooms
                .FirstOrDefaultAsync(r => r.Link == link && r.IsActive);
        }

        public async Task<ConferenceRoom?> GetRoomByLinkWithUsersAsync(string link)
        {
            return await _context.ConferenceRooms
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Link == link && r.IsActive);
        }

        public async Task<ConferenceRoom?> GetRoomByUserIdAsync(UserId userId)
        {
            var user = await _context.Users
                .Include(u => u.ConferenceRoom)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.ConferenceRoom;
        }

        private static string GenerateRoomLink()
        {
            return Guid.NewGuid().ToString("N")[..5];
        }
    }
}
