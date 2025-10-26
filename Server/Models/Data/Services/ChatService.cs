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
            var messages = await _context.ChatMessages
                .Where(m => m.ConferenceRoomId == roomId)
                .Include(m => m.User)
                .OrderByDescending(m => m.DateTime)
                .Take(count)                       
                .ToListAsync();

            
            return messages.OrderBy(m => m.DateTime).ToList();
        }

        public async Task<List<ChatMessage>> GetMessagesSinceAsync(ConferenceRoomId roomId, DateTime since)
        {
            return await _context.ChatMessages
                .Where(m => m.ConferenceRoomId == roomId && m.DateTime < since)
                .Include(m => m.User)
                .OrderBy(m => m.DateTime)
                .ToListAsync();
        }
    }
}
