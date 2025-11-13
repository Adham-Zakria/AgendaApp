using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace SwaiqatAgendaApp.Controllers
{
    public class DailyBalanceController : Controller
    {
        private readonly IDailyBalanceService _dailyBalanceService;
        private readonly IBranchService _branchService;
        private readonly ITransactionsService _transactionsService;

        public DailyBalanceController(
            IDailyBalanceService dailyBalanceService,
            IBranchService branchService,
            ITransactionsService transactionsService)
        {
            _dailyBalanceService = dailyBalanceService;
            _branchService = branchService;
            _transactionsService = transactionsService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // نجيب الفرع الحالي من الـ Session
            var branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
            if (branchId == 0)
                return RedirectToAction("Login", "Auth");

            //var today = DateTime.Today;
            // نحاول نجيب آخر يوم فيه معاملات للفرع
            //var lastTransactionDate = _transactionsService.GetAll()
            //    .Where(t => t.BranchId == branchId)
            //    .OrderByDescending(t => t.TransactionDate)
            //    .Select(t => t.TransactionDate.Date)
            //    .FirstOrDefault();

            //// لو مفيش معاملات خالص نشتغل على اليوم الحالي
            //var workDate = lastTransactionDate == default ? DateTime.Today : lastTransactionDate;
            // نحاول نجيب آخر يوم غير مقفول من DailyBalance
            var unclosedDay = _dailyBalanceService.GetLastUnclosedDay(branchId);

            // نحاول نجيب آخر معاملة لو مفيش يوم غير مقفول
            var lastTransactionDate = _transactionsService.GetAll()
                .Where(t => t.BranchId == branchId)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => t.TransactionDate.Date)
                .FirstOrDefault();

            // نحدد يوم العمل الفعلي
            DateTime workDate;
            if (unclosedDay != null)
            {
                // لو فيه يوم لسه متقفلش، نشتغل عليه
                workDate = unclosedDay.Value;
            }
            else if (lastTransactionDate != default)
            {
                // لو مفيش يوم غير مقفول، نشتغل على آخر يوم معاملات
                workDate = lastTransactionDate;
            }
            else
            {
                // لو مفيش معاملات خالص، نبدأ من اليوم الحالي
                workDate = DateTime.Today;
            }


            // نجيب بيانات الرصيد لو موجودة
            //var balance = _dailyBalanceService.GetByDate(branchId, today);
            // لو اليوم ده متقفلش نعرضه، حتى لو الساعة عدت 12
            var balance = _dailyBalanceService.GetByDate(branchId, workDate);

            // نجيب ملخص اليوم من المعاملات
            //var transactions = _transactionsService.GetByDate(today)
            var transactions = _transactionsService.GetByDate(workDate)
                .Where(t => t.BranchId == branchId)
                .ToList();

            //var income = transactions.Where(t => t.Type == "إيراد").Sum(t => t.Amount);
            //var expense = transactions.Where(t => t.Type == "مصروف").Sum(t => t.Amount);
            //var net = income - expense;

            //ViewBag.BranchName = _branchService.GetById(branchId)?.BranchName ?? "";
            //ViewBag.Income = income;
            //ViewBag.Expense = expense;
            //ViewBag.Net = net;
            //ViewBag.Balance = balance;

            //return View();

            // حساب الإجماليات
            decimal totalIncome = transactions.Where(t => t.Type == "إيراد").Sum(t => t.Amount) ?? 0;
            decimal totalExpense = transactions.Where(t => t.Type == "مصروف").Sum(t => t.Amount) ?? 0;
            decimal bankPayment = transactions.Where(t => t.Type == "دفعة بنك").Sum(t => t.Amount) ?? 0;
            decimal wholesalePayment = transactions.Where(t => t.Type == "دفعة جملة ماركت").Sum(t => t.Amount) ?? 0;
            decimal visaPayment = transactions.Where(t => t.Type == "فيزا بنك").Sum(t => t.Amount) ?? 0;
            decimal ownerPayment = transactions.Where(t => t.Type == "جاري مالك").Sum(t => t.Amount) ?? 0;

            // نجيب الرصيد الافتتاحي من اليوم السابق
            decimal openingBalance;
            if (balance == null)
            {
                //var yesterday = today.AddDays(-1);
                var yesterday = workDate.AddDays(-1);
                var yesterdayBalance = _dailyBalanceService.GetByDate(branchId, yesterday);
                openingBalance = yesterdayBalance?.ClosingBalance ?? 0;
            }
            else
            {
                openingBalance = balance.OpeningBalance ?? 0;
            }

            // حساب الرصيد الحالي والمرحّل
            decimal currentBalance = (totalIncome - totalExpense) + openingBalance;
            decimal closingBalance = currentBalance - (bankPayment + wholesalePayment + visaPayment + ownerPayment);

            // نرسل البيانات للـ View
            ViewBag.BranchName = _branchService.GetById(branchId)?.BranchName ?? "";
            ViewBag.OpeningBalance = openingBalance;
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.BankPayment = bankPayment;
            ViewBag.WholesalePayment = wholesalePayment;
            ViewBag.VisaPayment = visaPayment;
            ViewBag.OwnerPayment = ownerPayment;
            ViewBag.CurrentBalance = currentBalance;
            ViewBag.ClosingBalance = closingBalance;
            ViewBag.WorkDate = workDate;


            return View();
        }

        [HttpPost]
        public IActionResult CloseDay()
        {
            var branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
            if (branchId == 0)
                return RedirectToAction("Login", "Auth");

            try
            {
                //var today = DateTime.Today;
                //_dailyBalanceService.CreateOrUpdateBalance(branchId, today);
                //TempData["SuccessMessage"] = "تم قفل اليوم بنجاح";

                // نحاول نجيب آخر يوم غير مقفول من DailyBalance
                var unclosedDay = _dailyBalanceService.GetLastUnclosedDay(branchId);

                // نجيب آخر يوم فيه معاملات للفرع
                var lastTransactionDate = _transactionsService.GetAll()
                    .Where(t => t.BranchId == branchId)
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(t => t.TransactionDate.Date)
                    .FirstOrDefault();

                // نحدد يوم العمل الفعلي
                DateTime workDate;
                if (unclosedDay != null)
                {
                    // لو فيه يوم لسه متقفلش، نقفله دلوقتي
                    workDate = unclosedDay.Value;
                }
                else if (lastTransactionDate != default)
                {
                    // لو مفيش يوم غير مقفول، نقفل آخر يوم معاملات
                    workDate = lastTransactionDate;
                }
                else
                {
                    // لو مفيش أي معاملات، نقفل اليوم الحالي
                    workDate = DateTime.Today;
                }

                //var workDate = lastTransactionDate == default ? DateTime.Today : lastTransactionDate;

                _dailyBalanceService.CreateOrUpdateBalance(branchId, workDate);

                TempData["SuccessMessage"] = $"تم قفل يوم {workDate:yyyy-MM-dd} بنجاح";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء قفل اليوم: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

    }
}
