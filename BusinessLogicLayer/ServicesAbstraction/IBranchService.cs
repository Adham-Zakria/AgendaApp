using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServicesAbstraction
{
    public interface IBranchService
    {
        IEnumerable<Branch> GetAllBranches();
        Branch GetById(int id);
    }
}
