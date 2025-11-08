using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Contracts;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public User ValidateLogin(string username, string password)
        {
            var user = _usersRepository.GetByUsername(username);
            if (user == null) return null!;

            // add password hashing later 
            if (user.UserPassword == password)
                return user;

            return null!;
        }

        public User Authenticate(int userCode, string password)
        {
            return _usersRepository.GetAll()
                .FirstOrDefault(u => u.UserId == userCode && u.UserPassword == password)
                ?? throw new NotFoundException("User Not Found");
        }


        public IEnumerable<User> GetAllUsers()
        {
            return _usersRepository.GetAll();
        }

        public void AddUser(User user)
        {
            _usersRepository.Add(user);
        }

        public void UpdateUser(User user)
        {
            _usersRepository.Update(user);
        }
    }
}
