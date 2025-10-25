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
        public async Task CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User?> GetUserByIdAsync(UserId id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<User?> GetUserByLoginAsync(string login)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }
        public async Task<bool> ValidateUserAsync(UserCredential userCredential)
        {
            return await _context.Users
                .AnyAsync(u => u.Login == userCredential.Login && u.Password == userCredential.Password);
        }
    }
}
