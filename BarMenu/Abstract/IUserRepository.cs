﻿using BarMenu.Entities.AppEntities;

namespace BarMenu.Abstract
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        List<User> GetAllUsers();
        User UpdateUser(int id, User user);
        User GetUserById(int id);
        User DeleteUser(int id);
        Task<User> GetUserByName(string name);
    }
}
