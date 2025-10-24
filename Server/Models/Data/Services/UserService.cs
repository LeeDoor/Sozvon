namespace Server.Models.Data.Services
{
    public class UserService
    {
        private readonly ApplicationContext _context;

        public UserService(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<User> CreateUserAsync(string login, string password, string name)
        {
            var user = new User { Login = login, Password = password, Name = name };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}
