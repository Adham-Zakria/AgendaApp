using DataAccessLayer.Contracts;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class BranchesRepository : GenericRepository<Branch>, IBranchesRepository
    {
        public BranchesRepository(SwaiqatAgendaAppContext context) : base(context) { }
    }
}
