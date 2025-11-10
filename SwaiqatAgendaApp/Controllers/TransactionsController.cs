using BusinessLogicLayer.Services;
using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;

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
    }
}
