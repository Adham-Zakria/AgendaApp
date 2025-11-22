using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Contracts;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class DailyBalanceService : IDailyBalanceService
    {
        private readonly IGenericRepository<DailyBalance> _balanceRepository;
        private readonly ITransactionsRepository _transactionsRepository;

        public DailyBalanceService(
            IGenericRepository<DailyBalance> balanceRepository,
            ITransactionsRepository transactionsRepository)
        {
            _balanceRepository = balanceRepository;
            _transactionsRepository = transactionsRepository;
        }

        public DailyBalance GetByDate(int branchId, DateTime date)
        {
            // check BalanceDate.HasValue to avoid null reference exception
            return _balanceRepository.GetAll()
                .FirstOrDefault(b =>
                    b.BranchId == branchId &&
                    b.BalanceDate.HasValue &&
                    b.BalanceDate.Value.Date == date.Date);
        }

        public void CreateOrUpdateBalance(int branchId, DateTime date)
        {
            // if already has closing balance, do nothing
            var existing = GetByDate(branchId, date);
            if (existing != null && existing.ClosingBalance.HasValue)  // zero is a value 
                return;
            //if (existing != null && existing.ClosingBalance.HasValue && existing.ClosingBalance.Value != 0)
            //    return;

            // calc opening balance from previous day
            var prevDay = date.AddDays(-1);
            var prevBalance = GetByDate(branchId, prevDay);
            decimal opening = prevBalance?.ClosingBalance ?? 0;

            // today's transactions
            var transactions = _transactionsRepository
                .GetByDateRange(date.Date, date.Date)
                .Where(t => t.BranchId == branchId)
                .ToList();

            //var income = transactions
            //    .Where(t => t.Type == "إيراد")
            //    .Sum(t => t.Amount);

            //var expense = transactions
            //    .Where(t => t.Type == "مصروف")
            //    .Sum(t => t.Amount);

            //var closing = opening + income - expense;

            var income = transactions
                .Where(t => t.Type == "إيراد")
                .Sum(t => t.Amount);

            var expense = transactions
                .Where(t => t.Type == "مصروف")
                .Sum(t => t.Amount);

            // new options  
            var bankPayment = transactions
                .Where(t => t.Type == "دفعة بنك")
                .Sum(t => t.Amount);

            var wholesalePayment = transactions
                .Where(t => t.Type == "دفعة جملة ماركت")
                .Sum(t => t.Amount);

            var visaPayment = transactions
                .Where(t => t.Type == "فيزا بنك")
                .Sum(t => t.Amount);

            var ownerPayment = transactions
                .Where(t => t.Type == "جاري مالك")
                .Sum(t => t.Amount);

            // final closing balance
            var closing = ((income - expense) + opening)
                             - (bankPayment + wholesalePayment + visaPayment + ownerPayment);

            if (existing == null)
            {
                _balanceRepository.Add(new DailyBalance
                {
                    BranchId = branchId,
                    BalanceDate = date,
                    OpeningBalance = opening,
                    ClosingBalance = closing,
                    CreatedAt = DateTime.Now
                });
            }
            else
            {
                existing.OpeningBalance = opening;
                existing.ClosingBalance = closing;

                existing.CreatedAt = DateTime.Now; // update the created at to became the actual closing time 

                _balanceRepository.Update(existing);
            }

            // update next day opening balance
            UpdateNextDayOpening(branchId, date.AddDays(1), closing);
        }


        private void UpdateNextDayOpening(int branchId, DateTime nextDay, decimal? closingBalance)
        {
            var next = GetByDate(branchId, nextDay);

            if (next == null)
            {
                _balanceRepository.Add(new DailyBalance
                {
                    BranchId = branchId,
                    BalanceDate = nextDay,
                    OpeningBalance = closingBalance,
                    //ClosingBalance = 0,  // = null instead of 0
                    ClosingBalance = null,
                    CreatedAt = DateTime.Now
                });
            }
            else
            {
                next.OpeningBalance = closingBalance;
                _balanceRepository.Update(next);
            }
        }

        public IEnumerable<DailyBalance> GetAllBalances()
        {
            return _balanceRepository.GetAll()
                //.Include(b => b.Branch)
                .OrderByDescending(b => b.BalanceDate)
                .ToList();
        }

        public DailyBalance GetById(int id)
        {
            return _balanceRepository.GetAll()
                //.Include(b => b.Branch)
                .FirstOrDefault(b => b.BalanceId == id)
                ?? throw new NotFoundException("التقفيل غير موجود");
        }

        public void UpdateBalance(DailyBalance model, string userName)
        {
            var existing = _balanceRepository.GetAll().FirstOrDefault(b => b.BalanceId == model.BalanceId);
            if (existing == null) throw new NotFoundException("التقفيل غير موجود");

            existing.OpeningBalance = model.OpeningBalance;
            existing.ClosingBalance = model.ClosingBalance;
            existing.BalanceDate = model.BalanceDate;

            existing.LastModifiedBy = userName;
            existing.LastModifiedAt = DateTime.Now;

            _balanceRepository.Update(existing);
        }

        public DateTime? GetLastUnclosedDay(int branchId)
        {
            // نجيب آخر يوم الرصيد بتاعه مش متقفل

            //var unclosed = _balanceRepository
            //    .GetAll()
            //    .Where(b => b.BranchId == branchId && (b.ClosingBalance == null))
            //    .OrderBy(b => b.BalanceDate)
            //    .FirstOrDefault();

            //return unclosed?.BalanceDate;
            var allDays = _transactionsRepository.GetAll()
                .Where(t => t.BranchId == branchId)
                .Select(t => t.TransactionDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            var closedDays = _balanceRepository.GetAll()
                .Where(b => b.BranchId == branchId)
                .Select(b => b.BalanceDate.Value.Date)
                .ToList();

            var unclosed = allDays
                .FirstOrDefault(d => !closedDays.Contains(d));

            return unclosed == default ? (DateTime?)null : unclosed;
        }

        public bool ToggleReview(int id)
        {
            var balance = _balanceRepository.GetById(id);
            if (balance == null)
                throw new Exception("Balance not found");

            balance.IsReviewed = !balance.IsReviewed;
            _balanceRepository.Update(balance);

            return balance.IsReviewed;
        }


    }



    //public class DailyBalanceService : IDailyBalanceService
    //{
    //    private readonly IGenericRepository<DailyBalance> _balanceRepository;
    //    private readonly ITransactionsRepository _transactionsRepository;

    //    public DailyBalanceService(
    //        IGenericRepository<DailyBalance> balanceRepository,
    //        ITransactionsRepository transactionsRepository)
    //    {
    //        _balanceRepository = balanceRepository;
    //        _transactionsRepository = transactionsRepository;
    //    }

    //    public DailyBalance GetByDate(int branchId, DateTime date)
    //    {
    //        return _balanceRepository.GetAll()
    //            .FirstOrDefault(b => b.BranchId == branchId && b.BalanceDate.Value.Date == date.Date);
    //    }


    //    public void CreateOrUpdateBalance(int branchId, DateTime date)
    //    {
    //        // ✅ الحصول على الرصيد الافتتاحي من اليوم السابق (إن وجد)
    //        var prevDay = date.AddDays(-1);
    //        var prevBalance = GetByDate(branchId, prevDay);
    //        decimal opening = prevBalance?.ClosingBalance ?? 0;

    //        // ✅ حساب الإيرادات والمصروفات
    //        var transactions = _transactionsRepository
    //            .GetByDateRange(date, date)
    //            .Where(t => t.BranchId == branchId)
    //            .ToList();

    //        var income = transactions
    //            .Where(t => t.Type == "إيراد")
    //            .Sum(t => t.Amount);

    //        var expense = transactions
    //            .Where(t => t.Type == "مصروف")
    //            .Sum(t => t.Amount);

    //        var closing = opening + income - expense;

    //        // ✅ التحقق إذا كان اليوم موجود بالفعل أو لا
    //        var existing = GetByDate(branchId, date);

    //        if (existing == null)
    //        {
    //            _balanceRepository.Add(new DailyBalance
    //            {
    //                BranchId = branchId,
    //                BalanceDate = date,
    //                OpeningBalance = opening,
    //                ClosingBalance = closing,
    //                CreatedAt = DateTime.Now
    //            });
    //        }
    //        else
    //        {
    //            existing.OpeningBalance = opening;
    //            existing.ClosingBalance = closing;
    //            _balanceRepository.Update(existing);
    //        }

    //        // ✅ تأمين الترحيل التلقائي لليوم التالي (اختياري)
    //        UpdateNextDayOpening(branchId, date.AddDays(1), closing);
    //    }


    //    private void UpdateNextDayOpening(int branchId, DateTime nextDay, decimal? closingBalance)
    //    {
    //        var next = GetByDate(branchId, nextDay);
    //        if (next == null)
    //        {
    //            _balanceRepository.Add(new DailyBalance
    //            {
    //                BranchId = branchId,
    //                BalanceDate = nextDay,
    //                OpeningBalance = closingBalance,
    //                ClosingBalance = closingBalance, // مؤقتًا نفس القيمة
    //                CreatedAt = DateTime.Now
    //            });
    //        }
    //        else
    //        {
    //            next.OpeningBalance = closingBalance;
    //            _balanceRepository.Update(next);
    //        }
    //    }
    //}

    //public class DailyBalanceService : IDailyBalanceService
    //{
    //    private readonly IGenericRepository<DailyBalance> _balanceRepository;
    //    private readonly ITransactionsRepository _transactionsRepository;

    //    public DailyBalanceService(
    //        IGenericRepository<DailyBalance> balanceRepository,
    //        ITransactionsRepository transactionsRepository)
    //    {
    //        _balanceRepository = balanceRepository;
    //        _transactionsRepository = transactionsRepository;
    //    }

    //    public DailyBalance GetByDate(int branchId, DateTime date)
    //    {
    //        return _balanceRepository.GetAll()
    //            .FirstOrDefault(b => b.BranchId == branchId && b.BalanceDate?.Date == date.Date)
    //            ?? throw new NotFoundException("This date can't be found");
    //    }

    //    public void UpdateClosingBalance(int branchId, DateTime date)
    //    {
    //        var balance = GetByDate(branchId, date);
    //        if (balance == null) return;

    //        var transactions = _transactionsRepository.GetByDateRange(date, date)
    //            .Where(t => t.BranchId == branchId);

    //        var income = transactions.Where(t => t.Type == "إيراد").Sum(t => t.Amount);
    //        var expense = transactions.Where(t => t.Type == "مصروف").Sum(t => t.Amount);

    //        balance.ClosingBalance = balance.OpeningBalance + income - expense;

    //        _balanceRepository.Update(balance);
    //    }

    //    public void CreateOrUpdateBalance(int branchId, DateTime date, decimal opening, decimal closing)
    //    {
    //        var existing = GetByDate(branchId, date);
    //        if (existing == null)
    //        {
    //            _balanceRepository.Add(new DailyBalance
    //            {
    //                BranchId = branchId,
    //                BalanceDate = date,
    //                OpeningBalance = opening,
    //                ClosingBalance = closing
    //            });
    //        }
    //        else
    //        {
    //            existing.OpeningBalance = opening;
    //            existing.ClosingBalance = closing;
    //            _balanceRepository.Update(existing);
    //        }
    //    }
    //}

}
