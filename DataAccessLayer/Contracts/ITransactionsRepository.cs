using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Contracts
{
    public interface ITransactionsRepository : IGenericRepository<Transaction>
    {
        IEnumerable<Transaction> GetByDateRange(DateTime from, DateTime to);
        IEnumerable<Transaction> GetByBranch(int branchId);
        IEnumerable<Transaction> GetAllTransactions();
    }
}
