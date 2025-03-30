using BarMenu.Abstract;
using BarMenu.Entities;
using BarMenu.Entities.AppEntities;
using Microsoft.EntityFrameworkCore;

namespace BarMenu.Concrete
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;
        public UserRepository(Context context)
        {
            _context = context;
        }
        public async Task<User> CreateUser(User user)
        {           
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
        }
        public User UpdateUser(int id, User updateUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) {
                throw new Exception("Kullanıcı bulunamadı");
            }
            if (!string.IsNullOrEmpty(updateUser.Password)) { 
                user.Password = updateUser.Password;
            }
            _context.Users.Update(user);
            _context.SaveChanges();
            return user;
        }
        public User DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return user;
        }
        public User GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            return user;
        }
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
        public async Task<User> GetUserByName(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Name == name);
        }

    }
}
