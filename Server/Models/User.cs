using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Login {  get; set; }

        [Required]
        public string Name {  get; set; }

        [Required]
        public string Password { get; set; }

        public int? ConferenceRoomId { get; set; }
        [ForeignKey(nameof(ConferenceRoomId))]
        public ConferenceRoom? ConferenceRoom { get; set; }
    }
}
