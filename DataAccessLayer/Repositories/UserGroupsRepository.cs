using DataAccessLayer.Contracts;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UserGroupsRepository : GenericRepository<UserGroup>, IUserGroupsRepository
    {
        public UserGroupsRepository(SwaiqatAgendaAppContext context) : base(context) { }
    }
}
