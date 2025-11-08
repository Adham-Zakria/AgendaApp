using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServicesAbstraction
{
    public interface IDailyBalanceService
    {
        //DailyBalance GetByDate(int branchId, DateTime date);
        //void UpdateClosingBalance(int branchId, DateTime date);
        //void CreateOrUpdateBalance(int branchId, DateTime date, decimal opening, decimal closing);
        DailyBalance GetByDate(int branchId, DateTime date);
        void CreateOrUpdateBalance(int branchId, DateTime date);
        IEnumerable<DailyBalance> GetAllBalances();
        DailyBalance GetById(int id);
        void UpdateBalance(DailyBalance model);
    }
}
