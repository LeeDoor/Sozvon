namespace Server.Models
{
    public class ConferenceRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link {  get; set; }
        public bool IsActive { get; set; }
        public List<User>? Users { get; set; }
    }
}
