using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServicesAbstraction
{
    public interface IUsersService
    {
        User ValidateLogin(string username, string password);
        User Authenticate(int userCode, string password);
        IEnumerable<User> GetAllUsers();
        void AddUser(User user);
        void UpdateUser(User user);
    }
}
