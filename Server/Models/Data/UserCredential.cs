using System.ComponentModel.DataAnnotations;

namespace Server.Models.Data
{
    public class UserCredential
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
