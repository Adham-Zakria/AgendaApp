using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServicesAbstraction
{
    public interface ITransactionsService
    {
        IEnumerable<Transaction> GetTransactions(DateTime? from = null, DateTime? to = null, int? branchId = null);
        IEnumerable<Transaction> GetByDate(DateTime date);
        IEnumerable<Transaction> GetAll();
        void AddTransaction(Transaction transaction);
        //void UpdateTransaction(Transaction transaction);
        void UpdateTransaction(Transaction model, string username);
        void DeleteTransaction(int id);
    }
}
