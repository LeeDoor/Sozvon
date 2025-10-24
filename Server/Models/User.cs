using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Name {  get; set; }

        [Required]
        public string Password { get; set; }
        public ConferenceRoom? ConferenceRoom { get; set; }
    }
}
