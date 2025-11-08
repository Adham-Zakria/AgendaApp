using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Contracts;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchesRepository _branchRepository;

        public BranchService(IBranchesRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public IEnumerable<Branch> GetAllBranches()
        {
            return _branchRepository.GetAll().OrderBy(b => b.BranchName);
        }

        public Branch GetById(int id)
        {
            return _branchRepository.GetById(id);
        }
    }
}
