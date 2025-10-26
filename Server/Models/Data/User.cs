using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace Server.Models.Data
{
    public class User
    {
        public UserId Id { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        public DateTime RegistrationTime { get; set; }
        public ConferenceRoomId? ConferenceRoomId { get; set; }
        [ForeignKey(nameof(ConferenceRoomId))]
        public ConferenceRoom? ConferenceRoom { get; set; }

        public User()
        {
            RegistrationTime = DateTime.UtcNow;
        }
    }
}
