using BusinessLogicLayer.ServicesAbstraction;
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

            var today = DateTime.Today;

            // نجيب بيانات الرصيد لو موجودة
            var balance = _dailyBalanceService.GetByDate(branchId, today);

            // نجيب ملخص اليوم من المعاملات
            var transactions = _transactionsService.GetByDate(today)
                .Where(t => t.BranchId == branchId)
                .ToList();

            var income = transactions.Where(t => t.Type == "إيراد").Sum(t => t.Amount);
            var expense = transactions.Where(t => t.Type == "مصروف").Sum(t => t.Amount);
            var net = income - expense;

            ViewBag.BranchName = _branchService.GetById(branchId)?.BranchName ?? "";
            ViewBag.Income = income;
            ViewBag.Expense = expense;
            ViewBag.Net = net;
            ViewBag.Balance = balance;

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
                var today = DateTime.Today;
                _dailyBalanceService.CreateOrUpdateBalance(branchId, today);
                TempData["SuccessMessage"] = "تم قفل اليوم بنجاح";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء قفل اليوم: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

    }
}
