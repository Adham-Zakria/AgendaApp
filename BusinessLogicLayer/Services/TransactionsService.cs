using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Contracts;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly ITransactionsRepository _transactionsRepository;

        public TransactionsService(ITransactionsRepository transactionsRepository)
        {
            _transactionsRepository = transactionsRepository;
        }

        public IEnumerable<Transaction> GetTransactions(DateTime? from = null, DateTime? to = null, int? branchId = null)
        {
            IEnumerable<Transaction> result = _transactionsRepository.GetAll();

            if (branchId.HasValue)
                result = _transactionsRepository.GetByBranch(branchId.Value);

            if (from.HasValue && to.HasValue)
                result = _transactionsRepository.GetByDateRange(from.Value, to.Value);

            return result;
        }

        public IEnumerable<Transaction> GetByDate(DateTime date)
        {
            return _transactionsRepository.GetAll()
                .Where(t => t.TransactionDate.Date == date.Date)
                .ToList();
        }

        public IEnumerable<Transaction> GetAll()
        {
            return _transactionsRepository.GetAllTransactions();
        }

        public void AddTransaction(Transaction transaction)
        {
            transaction.TransactionDate = DateTime.Now;
            _transactionsRepository.Add(transaction);
        }

        //public void UpdateTransaction(Transaction transaction)
        //{
        //    _transactionsRepository.Update(transaction);
        //}
        public void UpdateTransaction(Transaction model)
        {
            var existing = _transactionsRepository.GetById(model.TransactionId);

            if (existing != null)
            {
                existing.Amount = model.Amount;
                existing.Description = model.Description;
                existing.Category = model.Category;
                existing.Type = model.Type;

                // use new date
                //if (model.TransactionDate != default(DateTime))
                //    existing.TransactionDate = model.TransactionDate;

                // doesn't change UserId and BranchId 
                _transactionsRepository.Update(existing);
            }
        }


        public void DeleteTransaction(int id)
        {
            _transactionsRepository.Delete(id);
        }
    }
}
