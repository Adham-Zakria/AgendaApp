using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Contracts
{
    public interface IUsersRepository : IGenericRepository<User>
    {
        User GetByUsername(string username);
        IEnumerable<User> GetByBranch(int branchId);
    }
}
