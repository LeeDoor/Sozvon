using Microsoft.EntityFrameworkCore;

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
        public async Task<User?> GetUserByIdAsync(UserId id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<User?> GetUserByLoginAsync(string login)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }
        public async Task<bool> ValidateUserAsync(string login, string password)
        {
            return await _context.Users
                .AnyAsync(u => u.Login == login && u.Password == password);
        }
    }
}
