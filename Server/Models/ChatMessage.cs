using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class ChatMessage
    {
        public ChatMessageId Id { get; set; }
        public string Content {  get; set; }
        public DateTime DateTime { get; set; }

        public UserId UserId { get; set; }
        public User? User { get; set; }

        public ConferenceRoomId? ConferenceRoomId { get; set; }
        [ForeignKey(nameof(ConferenceRoomId))]
        public ConferenceRoom? ConferenceRoom { get; set; }
    }
}
