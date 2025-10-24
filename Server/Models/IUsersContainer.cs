namespace Server.Models
{
    public interface IUsersContainer
    {
        public int CreateUser(User user);
        public bool ValidateUser(UserCredential userCredential);
        public int DeleteUser(int userId);
    }
}
