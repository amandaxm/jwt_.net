using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ultimoteste.Models;

namespace ultimoteste.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext userContext;

        public UserRepository(UserContext userContext)
        {
            this.userContext = userContext;
        }

        public User Create(User user)
        {
            userContext.Users.Add(user);
            user.Id =  userContext.SaveChanges();
            return user;
        }

        public User GetByEmail(string email)
        {
            return userContext.Users.FirstOrDefault(u => u.Email == email);
        }

        public User GetById(int id)
        {
            return userContext.Users.FirstOrDefault(u => u.Id == id);
        }
    }
}
