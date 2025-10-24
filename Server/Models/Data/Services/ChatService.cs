using Microsoft.EntityFrameworkCore;

namespace Server.Models.Data.Services
{
    public class ChatService
    {
        private readonly ApplicationContext _context;

        public ChatService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<ChatMessage> SendMessageAsync(ConferenceRoomId roomId, UserId userId, string content)
        {
            var message = new ChatMessage
            {
                ConferenceRoomId = roomId,
                UserId = userId,
                Content = content,
                DateTime = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<ChatMessage>> GetRoomMessagesAsync(ConferenceRoomId roomId, int count = 100)
        {
            return await _context.ChatMessages
                .Where(m => m.ConferenceRoomId == roomId)
                .Include(m => m.User)
                .OrderByDescending(m => m.DateTime)
                .Take(count)
                .OrderBy(m => m.DateTime) 
                .ToListAsync();
        }
    }
}
