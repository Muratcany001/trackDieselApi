using BarMenu.Entities.AppEntities;

namespace BarMenu.Abstract
{
    public interface IUserRepository
    {
        User CreateUser(User user);
        List<User> GetAllUsers();
        User UpdateUser(int id, User user);
        User GetUserById(int id);
        User DeleteUser(int id);
        User GetUserByName(string name);
    }
}
