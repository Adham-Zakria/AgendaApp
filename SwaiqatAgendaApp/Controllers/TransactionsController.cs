using BusinessLogicLayer.Services;
using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using SwaiqatAgendaApp.Models;

namespace SwaiqatAgendaApp.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionsService _transactionService;
        private readonly IBranchService _branchService;
        private readonly IDailyBalanceService _dailyBalanceService;

        public TransactionsController(ITransactionsService transactionService, 
            IBranchService branchService,
            IDailyBalanceService dailyBalanceService)
        {
            _transactionService = transactionService;
            _branchService = branchService;
            _dailyBalanceService = dailyBalanceService;
        }

        //[HttpGet]
        //public IActionResult Index()
        //{
        //    //  dropdown for loading all branches
        //    var branches = _branchService.GetAllBranches();
        //    ViewBag.Branches = branches;

        //    // today's transactions
        //    var transactions = _transactionService.GetByDate(DateTime.Today);
        //    return View(transactions);
        //}
        [HttpGet]
        public IActionResult Index()
        {
            // get all branches
            var branches = _branchService.GetAllBranches();
            ViewBag.Branches = branches;

            // Get the user's branch
            var userBranchId = HttpContext.Session.GetInt32("BranchId");

            //
            var branch = _branchService.GetById((int)userBranchId!);
            ViewBag.BranchName = branch?.BranchName;

            //if (userBranchId == null)
            //    return RedirectToAction("Login", "Account");

            // get today's transactions for this user's branch
            var transactions = _transactionService
                .GetByDate(DateTime.Today)
                .Where(t => t.BranchId == userBranchId.Value)
                .ToList();
            //
            var today = DateTime.Today;
            //var dailyBalance = _dailyBalanceService.GetByDate((int)userBranchId, today);
            //decimal openingBalance = dailyBalance?.OpeningBalance ?? 0;

            //ViewBag.OpeningBalance = openingBalance;
            var dailyBalance = _dailyBalanceService.GetByDate((int)userBranchId, today);
            if (dailyBalance == null)
            {
                var yesterday = today.AddDays(-1);
                var yesterdayBalance = _dailyBalanceService.GetByDate((int)userBranchId, yesterday);
                ViewBag.OpeningBalance = yesterdayBalance?.ClosingBalance ?? 0;
            }
            else
            {
                ViewBag.OpeningBalance = dailyBalance.OpeningBalance;
            }


            return View(transactions);
        }




        [HttpPost]
        public IActionResult AddTransaction(Transaction model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = _branchService.GetAllBranches();
                var transactions = _transactionService.GetByDate(DateTime.Today);
                return View("Index", transactions);
            }

            model.TransactionDate = DateTime.Now;
            //model.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
            model.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            //
            model.BranchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            _transactionService.AddTransaction(model);

            return RedirectToAction("Index");
        }
        //[HttpGet]
        //public IActionResult Index()
        //{
        //    var branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
        //    var branch = _branchService.GetById(branchId);
        //    ViewBag.BranchName = branch?.BranchName;

        //    var today = DateTime.Today;
        //    var daily = _dailyBalanceService.GetByDate(branchId, today);
        //    ViewBag.OpeningBalance = daily?.OpeningBalance ?? 0;

        //    return View();
        //}

        //[HttpPost]
        //public IActionResult AddTransaction(List<Transaction> transactions, decimal OpeningBalance)
        //{
        //    var branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
        //    var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        //    var today = DateTime.Today;

        //    // فلترة البيانات الفارغة
        //    var validTransactions = transactions
        //        .Where(t => !string.IsNullOrEmpty(t.Type) && t.Amount > 0)
        //        .ToList();

        //    // حفظ كل معاملة
        //    foreach (var t in validTransactions)
        //    {
        //        t.BranchId = branchId;
        //        t.UserId = userId;
        //        t.TransactionDate = DateTime.Now;
        //        _transactionService.AddTransaction(t);
        //    }

        //    // حساب الإجماليات
        //    var income = validTransactions.Where(t => t.Type == "إيراد").Sum(t => t.Amount);
        //    var expense = validTransactions.Where(t => t.Type == "مصروف").Sum(t => t.Amount);
        //    var closing = OpeningBalance + income - expense;

        //    // تحديث أو إنشاء DailyBalance
        //    _dailyBalanceService.CreateOrUpdateBalance(branchId, today);

        //    TempData["Message"] = "تم حفظ معاملات اليوم بنجاح";
        //    return RedirectToAction("Index");
        //}

    }
}
