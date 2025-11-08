using BusinessLogicLayer.Services;
using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace SwaiqatAgendaApp.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionsService _transactionService;
        private readonly IBranchService _branchService;

        public TransactionsController(ITransactionsService transactionService, IBranchService branchService)
        {
            _transactionService = transactionService;
            _branchService = branchService;
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
