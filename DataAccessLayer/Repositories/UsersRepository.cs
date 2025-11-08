using DataAccessLayer.Contracts;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UsersRepository : GenericRepository<User>, IUsersRepository
    {
        public UsersRepository(SwaiqatAgendaAppContext context) : base(context) { }

        public User GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == username)
                ?? throw new NotFoundException("User Not Found");
        }

        public IEnumerable<User> GetByBranch(int branchId)
        {
            return _context.Users.Where(u => u.BranchId == branchId).ToList();
        }
    }
}
