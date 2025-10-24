using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Content {  get; set; }
        public DateTime DateTime { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int? ConferenceRoomId { get; set; }
        [ForeignKey(nameof(ConferenceRoomId))]
        public ConferenceRoom? ConferenceRoom { get; set; }
    }
}
