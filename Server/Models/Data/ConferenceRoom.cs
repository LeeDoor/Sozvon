using System.ComponentModel.DataAnnotations;

namespace Server.Models.Data
{

    public class ConferenceRoom
    {
        public ConferenceRoomId Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Link {  get; set; }
        public bool IsActive { get; set; }
        public List<User>? Users { get; set; }
        public List<ChatMessage>? ChatMessages { get; set; }
    }
}
