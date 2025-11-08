using DataAccessLayer.Contracts;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class TransactionsRepository : GenericRepository<Transaction>, ITransactionsRepository
    {
        public TransactionsRepository(SwaiqatAgendaAppContext context) : base(context) { }

        public IEnumerable<Transaction> GetByDateRange(DateTime from, DateTime to)
        {
            //return _context.Transactions
            //    .Where(t => t.TransactionDate >= from && t.TransactionDate <= to)
            //    .ToList();
            return _context.Transactions
                .Where(t => t.TransactionDate >= from.Date &&
                            t.TransactionDate < to.Date.AddDays(1))
                .ToList();
        }

        public IEnumerable<Transaction> GetByBranch(int branchId)
        {
            return _context.Transactions
                .Where(t => t.BranchId == branchId)
                .ToList();
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return _context.Transactions
                .Include(t => t.Branch)
                .Include(t => t.User)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
        }
    }
}
