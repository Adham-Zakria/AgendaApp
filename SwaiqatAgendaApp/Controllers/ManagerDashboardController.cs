using BusinessLogicLayer.Services;
using BusinessLogicLayer.ServicesAbstraction;
using ClosedXML.Excel;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwaiqatAgendaApp.Helper;

namespace SwaiqatAgendaApp.Controllers
{
    //[Authorize]
    //[SessionAuthorize]
    [SessionAuthorize(requireAdmin: true)]
    public class ManagerDashboardController : Controller
    {
        private readonly ITransactionsService _transactionsService;
        private readonly IUsersService _usersService;
        private readonly IBranchService _branchService;
        private readonly IDailyBalanceService _dailyBalanceService;
        private readonly IAdditionalInfoService _additionalInfoService;
        
        public ManagerDashboardController(
            ITransactionsService transactionsService,
            IUsersService usersService,
            IBranchService branchService,
            IDailyBalanceService dailyBalanceService,
            IAdditionalInfoService additionalInfoService)
        {
            _transactionsService = transactionsService;
            _usersService = usersService;
            _branchService = branchService;
            _dailyBalanceService = dailyBalanceService;
            _additionalInfoService = additionalInfoService;
        }

        [HttpGet]
        public IActionResult Index(DateTime? fromDate, DateTime? toDate, int? branchId, int? userId)
        {
            var transactions = _transactionsService.GetAll();

            // filteraton by dates, users and branches 
            if (fromDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                //transactions = transactions.Where(t => t.TransactionDate <= toDate.Value);
                transactions = transactions.Where(t => t.TransactionDate < toDate.Value.AddDays(1));

            if (branchId.HasValue && branchId > 0)
                transactions = transactions.Where(t => t.BranchId == branchId);

            if (userId.HasValue && userId > 0)
                transactions = transactions.Where(t => t.UserId == userId);

            #region ADDITIONAL INFO
            var extraInfo = _additionalInfoService.GetAllInfo();

            //if (branchId.HasValue && branchId > 0 && fromDate.HasValue)
            //{
            //    //var date = (DateOnly)fromDate.Value.Date;
            //    var date = DateOnly.FromDateTime(fromDate.Value);
            //    extraInfo = _additionalInfoService.GetInfoForDay(branchId.Value, date);
            //}
            if (branchId.HasValue && branchId > 0)
            {
                extraInfo = extraInfo.Where(i => i.BranchId == branchId.Value);
            }

            if (fromDate.HasValue)
            {
                var from = DateOnly.FromDateTime(fromDate.Value);
                extraInfo = extraInfo.Where(i => i.TransactionDate >= from);
            }

            if (toDate.HasValue)
            {
                var to = DateOnly.FromDateTime(toDate.Value);
                extraInfo = extraInfo.Where(i => i.TransactionDate <= to);
            }
            #endregion


            ViewBag.Branches = _branchService.GetAllBranches();
            ViewBag.Users = _usersService.GetAllUsers();
            ViewBag.AdditionalInfo = extraInfo.ToList();

            return View(transactions.ToList());
        }

        //[HttpGet]
        //public IActionResult ExportToExcel()
        //{
        //    var transactions = _transactionsService.GetAll().ToList();

        //    using (var workbook = new XLWorkbook())
        //    {
        //        var ws = workbook.Worksheets.Add("Transactions");
        //        ws.Cell(1, 1).Value = "التاريخ";
        //        ws.Cell(1, 2).Value = "الفرع";
        //        ws.Cell(1, 3).Value = "الموظف";
        //        ws.Cell(1, 4).Value = "النوع";
        //        //ws.Cell(1, 5).Value = "الفئة";
        //        ws.Cell(1, 5).Value = "البيان";
        //        ws.Cell(1, 6).Value = "الوصف";
        //        ws.Cell(1, 7).Value = "المبلغ";

        //        int row = 2;
        //        foreach (var t in transactions)
        //        {
        //            ws.Cell(row, 1).Value = t.TransactionDate;
        //            ws.Cell(row, 2).Value = t.Branch?.BranchName;
        //            ws.Cell(row, 3).Value = t.User?.UserName;
        //            ws.Cell(row, 4).Value = t.Type;
        //            ws.Cell(row, 5).Value = t.Category;
        //            ws.Cell(row, 6).Value = t.Description;
        //            ws.Cell(row, 7).Value = t.Amount;
        //            row++;
        //        }

        //        using (var stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            var content = stream.ToArray();
        //            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Transactions.xlsx");
        //        }
        //    }
        //}

        //[HttpGet]
        //public IActionResult ExportToExcel(DateTime? fromDate, DateTime? toDate, int? branchId, int? userId)
        //{
        //    var transactions = _transactionsService.GetAll();

        //    // apply filters
        //    if (fromDate.HasValue)
        //        transactions = transactions.Where(t => t.TransactionDate.Date >= fromDate.Value.Date);

        //    if (toDate.HasValue)
        //        //transactions = transactions.Where(t => t.TransactionDate.Date <= toDate.Value.Date);
        //        transactions = transactions.Where(t => t.TransactionDate < toDate.Value.AddDays(1));


        //    if (branchId.HasValue && branchId.Value > 0)
        //        transactions = transactions.Where(t => t.BranchId == branchId.Value);

        //    if (userId.HasValue && userId.Value > 0)
        //        transactions = transactions.Where(t => t.UserId == userId.Value);

        //    var list = transactions.ToList();

        //    using (var workbook = new XLWorkbook())
        //    {
        //        var ws = workbook.Worksheets.Add("Transactions");
        //        ws.Cell(1, 1).Value = "التاريخ";
        //        ws.Cell(1, 2).Value = "الفرع";
        //        ws.Cell(1, 3).Value = "الموظف";
        //        ws.Cell(1, 4).Value = "النوع";
        //        ws.Cell(1, 5).Value = "البيان";
        //        ws.Cell(1, 6).Value = "الوصف";
        //        ws.Cell(1, 7).Value = "المبلغ";

        //        int row = 2;
        //        foreach (var t in list)
        //        {
        //            ws.Cell(row, 1).Value = t.TransactionDate.ToString("yyyy-MM-dd HH:mm");
        //            ws.Cell(row, 2).Value = t.Branch?.BranchName;
        //            ws.Cell(row, 3).Value = t.User?.UserName;
        //            ws.Cell(row, 4).Value = t.Type;
        //            ws.Cell(row, 5).Value = t.Category;
        //            ws.Cell(row, 6).Value = t.Description;
        //            ws.Cell(row, 7).Value = t.Amount;
        //            row++;
        //        }

        //        using (var stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            var content = stream.ToArray();
        //            return File(content,
        //                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                "Transactions.xlsx");
        //        }
        //    }
        //}

        [HttpGet]
        public IActionResult ExportToExcel(DateTime? fromDate, DateTime? toDate, int? branchId, int? userId)
        {
            var transactions = _transactionsService.GetAll();

            // apply filters
            if (fromDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate < toDate.Value.AddDays(1));

            if (branchId.HasValue && branchId.Value > 0)
                transactions = transactions.Where(t => t.BranchId == branchId.Value);

            if (userId.HasValue && userId.Value > 0)
                transactions = transactions.Where(t => t.UserId == userId.Value);

            // استبعاد أنواع معينة من الجدول
            var excludedTypes = new[] { "دفعة بنك", "دفعة جملة ماركت", "فيزا بنك", "جاري مالك" };
            var list = transactions
                .Where(t => !excludedTypes.Contains(t.Type))
                .OrderBy(t => t.TransactionDate)
                .ToList();

            #region Additional info filters
            var extraInfo = _additionalInfoService.GetAllInfo();

            if (branchId.HasValue && branchId > 0)
                extraInfo = extraInfo.Where(i => i.BranchId == branchId.Value);

            if (fromDate.HasValue)
            {
                var from = DateOnly.FromDateTime(fromDate.Value);
                extraInfo = extraInfo.Where(i => i.TransactionDate >= from);
            }

            if (toDate.HasValue)
            {
                var to = DateOnly.FromDateTime(toDate.Value);
                extraInfo = extraInfo.Where(i => i.TransactionDate <= to);
            }

            var extraList = extraInfo.OrderBy(i => i.TransactionDate).ToList();
            #endregion

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("اليومية");

                // العناوين
                ws.Cell(1, 1).Value = "التاريخ";
                ws.Cell(1, 2).Value = "الفرع";
                ws.Cell(1, 3).Value = "الإيرادات";
                ws.Cell(1, 4).Value = "المصروفات";
                ws.Cell(1, 5).Value = "النوع";
                ws.Cell(1, 6).Value = "الوصف";
                ws.Cell(1, 7).Value = "البيان";

                int row = 2;
                foreach (var t in list)
                {
                    ws.Cell(row, 1).Value = t.TransactionDate.ToString("yyyy-MM-dd HH:mm");
                    ws.Cell(row, 2).Value = t.Branch?.BranchName;
                    ws.Cell(row, 3).Value = t.Type == "إيراد" ? t.Amount : "-";
                    ws.Cell(row, 4).Value = t.Type == "مصروف" ? t.Amount : "-";
                    ws.Cell(row, 5).Value = t.Type;
                    ws.Cell(row, 6).Value = t.Description;
                    ws.Cell(row, 7).Value = t.Category ?? "-";
                    row++;
                }

                #region Additional info section
                row += 2; // مسافة بين الجدولين

                ws.Cell(row, 1).Value = "التاريخ";
                ws.Cell(row, 2).Value = "الوظيفة";
                ws.Cell(row, 3).Value = "الاسم";
                ws.Cell(row, 4).Value = "الكود";
                ws.Cell(row, 5).Value = "القيمة";
                ws.Cell(row, 6).Value = "ملاحظات";

                ws.Range(row, 1, row, 6).Style.Font.Bold = true;
                row++;

                foreach (var info in extraList)
                {
                    ws.Cell(row, 1).Value = info.TransactionDate.ToString();
                    ws.Cell(row, 2).Value = info.Position;
                    ws.Cell(row, 3).Value = info.FullName;
                    ws.Cell(row, 4).Value = info.Code;
                    ws.Cell(row, 5).Value = info.Value;
                    ws.Cell(row, 6).Value = info.Notes;
                    row++;
                }
                #endregion

                // تنسيقات بسيطة
                ws.Range("A1:E1").Style.Font.Bold = true;
                ws.Range("A1:E1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "اليومية.xlsx");
                }
            }
        }


        //[HttpGet]
        //public IActionResult ExportToExcel(DateTime? fromDate, DateTime? toDate, int? branchId, int? userId)
        //{
        //    var transactions = _transactionsService.GetAll();

        //    // apply filters
        //    if (fromDate.HasValue)
        //        transactions = transactions.Where(t => t.TransactionDate.Date >= fromDate.Value.Date);

        //    if (toDate.HasValue)
        //        transactions = transactions.Where(t => t.TransactionDate < toDate.Value.AddDays(1));

        //    if (branchId.HasValue && branchId.Value > 0)
        //        transactions = transactions.Where(t => t.BranchId == branchId.Value);

        //    if (userId.HasValue && userId.Value > 0)
        //        transactions = transactions.Where(t => t.UserId == userId.Value);

        //    // بدون استبعاد أي نوع
        //    var list = transactions
        //        .OrderBy(t => t.TransactionDate)
        //        .ToList();

        //    using (var workbook = new XLWorkbook())
        //    {
        //        var ws = workbook.Worksheets.Add("اليومية");

        //        // ====== العناوين ======
        //        ws.Cell(1, 1).Value = "الإيرادات";
        //        ws.Cell(1, 2).Value = "المصروفات";
        //        ws.Cell(1, 3).Value = "دفعة بنك";
        //        ws.Cell(1, 4).Value = "دفعة جملة ماركت";
        //        ws.Cell(1, 5).Value = "فيزا بنك";
        //        ws.Cell(1, 6).Value = "جاري مالك";
        //        ws.Cell(1, 7).Value = "النوع";
        //        ws.Cell(1, 8).Value = "الوصف";
        //        ws.Cell(1, 9).Value = "البيان";
        //        ws.Cell(1, 10).Value = "التاريخ";

        //        ws.Range("A1:J1").Style.Font.Bold = true;
        //        ws.Range("A1:J1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //        int row = 2;

        //        // ====== البيانات ======
        //        foreach (var t in list)
        //        {
        //            ws.Cell(row, 1).Value = t.Type == "إيراد" ? t.Amount : "";
        //            ws.Cell(row, 2).Value = t.Type == "مصروف" ? t.Amount : "";
        //            ws.Cell(row, 3).Value = t.Type == "دفعة بنك" ? t.Amount : "";
        //            ws.Cell(row, 4).Value = t.Type == "دفعة جملة ماركت" ? t.Amount : "";
        //            ws.Cell(row, 5).Value = t.Type == "فيزا بنك" ? t.Amount : "";
        //            ws.Cell(row, 6).Value = t.Type == "جاري مالك" ? t.Amount : "";
        //            ws.Cell(row, 7).Value = t.Type;
        //            ws.Cell(row, 8).Value = t.Description;
        //            ws.Cell(row, 9).Value = t.Category ?? "-";
        //            ws.Cell(row, 10).Value = t.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss");

        //            row++;
        //        }

        //        // ضبط الأعمدة
        //        ws.Columns().AdjustToContents();

        //        using (var stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            var content = stream.ToArray();
        //            return File(content,
        //                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                "اليومية.xlsx");
        //        }
        //    }
        //}





        [HttpGet]
        public IActionResult Edit(int id)
        {
            var transaction = _transactionsService.GetAll().FirstOrDefault(t => t.TransactionId == id);
            if (transaction == null)
                return NotFound();

            ViewBag.Branches = _branchService.GetAllBranches();
            ViewBag.Users = _usersService.GetAllUsers();

            return PartialView("_EditTransactionPartial", transaction);
        }

        [HttpPost]
        public IActionResult Edit(Transaction model)
        {
            if (!ModelState.IsValid)
                return BadRequest("البيانات غير صحيحة.");

            var username = HttpContext.Session.GetString("UserName") ?? "Unknown";

            _transactionsService.UpdateTransaction(model, username); // adding also in service

            return Json(new { success = true, modifiedBy = username });
        }

        [HttpGet]
        public IActionResult BranchClosings()
        {
            //var balances = _dailyBalanceService.GetAllBalances()
            //    .OrderByDescending(b => b.BalanceDate)
            //    .ToList();

            //var branches = _branchService.GetAllBranches();

            //ViewBag.Branches = branches;
            //return View(balances);

            var balances = _dailyBalanceService.GetAllBalances()
                .OrderByDescending(b => b.BalanceDate)
                .ToList();

            var branches = _branchService.GetAllBranches();

            // نجيب كل المعاملات عشان نحسب الإيرادات والمصروفات
            var transactions = _transactionsService.GetAll();

            // نحسب الإيراد والمصروف لكل فرع وتاريخ
            //var incomeExpenses = transactions
            //    .GroupBy(t => new { t.BranchId, Date = t.TransactionDate.Date })
            //    .Select(g => new
            //    {
            //        g.Key.BranchId,
            //        g.Key.Date,
            //        Income = g.Where(x => x.Type == "إيراد").Sum(x => x.Amount),
            //        Expense = g.Where(x => x.Type == "مصروف").Sum(x => x.Amount)
            //    })
            //    .ToList();

            // نحسب الإيراد، المصروف، والدفعات الإضافية لكل فرع وتاريخ
            var incomeExpenses = transactions
                .GroupBy(t => new { t.BranchId, Date = t.TransactionDate.Date })
                .Select(g => new
                {
                    g.Key.BranchId,
                    g.Key.Date,
                    Income = g.Where(x => x.Type == "إيراد").Sum(x => x.Amount),
                    Expense = g.Where(x => x.Type == "مصروف").Sum(x => x.Amount),
                    BankPayment = g.Where(x => x.Type == "دفعة بنك").Sum(x => x.Amount),
                    WholesalePayment = g.Where(x => x.Type == "دفعة جملة ماركت").Sum(x => x.Amount),
                    VisaPayment = g.Where(x => x.Type == "فيزا بنك").Sum(x => x.Amount),
                    OwnerPayment = g.Where(x => x.Type == "جاري مالك").Sum(x => x.Amount)
                })
                .ToList();

            ViewBag.IncomeExpenses = incomeExpenses;
            ViewBag.Branches = branches;
            return View(balances);
        }

        [HttpGet]
        public IActionResult BranchClosingsFiltered(int? branchId, DateTime? fromDate, DateTime? toDate)
        {
            var balances = _dailyBalanceService.GetAllBalances();

            if (branchId.HasValue)
                balances = balances.Where(b => b.BranchId == branchId);

            if (fromDate.HasValue)
                balances = balances.Where(b => b.BalanceDate >= fromDate);

            if (toDate.HasValue)
                balances = balances.Where(b => b.BalanceDate <= toDate);

            //
            //var incomeExpenses = _transactionsService.GetAll()
            //    .GroupBy(t => new { t.BranchId, Date = t.TransactionDate.Date })
            //    .Select(g => new
            //    {
            //        BranchId = g.Key.BranchId,
            //        Date = g.Key.Date,
            //        Income = g.Where(t => t.Type == "إيراد").Sum(t => t.Amount),
            //        Expense = g.Where(t => t.Type == "مصروف").Sum(t => t.Amount)
            //    })
            //    .ToList();
            var incomeExpenses = _transactionsService.GetAll()
                .GroupBy(t => new { t.BranchId, Date = t.TransactionDate.Date })
                .Select(g => new
                {
                    g.Key.BranchId,
                    g.Key.Date,
                    Income = g.Where(x => x.Type == "إيراد").Sum(x => x.Amount),
                    Expense = g.Where(x => x.Type == "مصروف").Sum(x => x.Amount),
                    BankPayment = g.Where(x => x.Type == "دفعة بنك").Sum(x => x.Amount),
                    WholesalePayment = g.Where(x => x.Type == "دفعة جملة ماركت").Sum(x => x.Amount),
                    VisaPayment = g.Where(x => x.Type == "فيزا بنك").Sum(x => x.Amount),
                    OwnerPayment = g.Where(x => x.Type == "جاري مالك").Sum(x => x.Amount)
                })
                .ToList();

            ViewBag.IncomeExpenses = incomeExpenses;

            ViewBag.Branches = _branchService.GetAllBranches();
            //return View(balances.OrderByDescending(b => b.BalanceDate).ToList());
            return View("BranchClosings", balances.OrderByDescending(b => b.BalanceDate).ToList());
        }

        [HttpGet]
        public IActionResult EditBalance(int id)
        {
            var balance = _dailyBalanceService.GetById(id);
            if (balance == null) return NotFound();

            return PartialView("_EditBalancePartial", balance);
        }

        [HttpPost]
        public IActionResult EditBalance(DailyBalance model)
        {
            try
            {
                _dailyBalanceService.UpdateBalance(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ToggleReview(int id)
        {
            try
            {
                var isReviewed = _dailyBalanceService.ToggleReview(id);
                return Json(new { success = true, isReviewed });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        public IActionResult Print(DateTime? fromDate, DateTime? toDate, int? branchId, int? userId)
        {
            #region transactions

            var data = _transactionsService.GetTransactions(fromDate, toDate, branchId)
                  .OrderByDescending(t => t.TransactionDate)
                  .ThenByDescending(t => t.TransactionId)
                  .ToList();

            #endregion

            #region Branch

            string branchName = "—";
            //var branch = _branchService.GetById(branchId ?? 0);
            if (branchId.HasValue && branchId.Value > 0)
            {
                var branch = _branchService.GetById(branchId.Value);
                branchName = branch?.BranchName ?? "—";
            }
            ViewBag.BranchName = branchName;

            #endregion

            #region Opening Balance 

            decimal openingBalanceValue = 0;
            //var openingBalance = _dailyBalanceService.GetByDate(branchId ?? 0, (DateTime)fromDate!);
            if (branchId.HasValue && fromDate.HasValue)
            {
                var openingBalance = _dailyBalanceService.GetByDate(branchId.Value, fromDate.Value);
                openingBalanceValue = openingBalance?.OpeningBalance ?? 0;
            }
            ViewBag.OpeningBalance = openingBalanceValue;

            #endregion

            #region Additional Info 

            var fromDay = fromDate.HasValue
                          ? DateOnly.FromDateTime(fromDate.Value)
                          : DateOnly.FromDateTime(DateTime.Today);

            var toDay = toDate.HasValue 
                        ? DateOnly.FromDateTime(toDate.Value) 
                        : DateOnly.FromDateTime(DateTime.Today);


            //var additional = (branchId.HasValue && branchId.Value > 0)
            //    ? _additionalInfoService.GetInfoForDay(branchId.Value, fromDay)
            //    : new List<AdditionalInfo>();

            var additional = (branchId.HasValue && branchId.Value > 0)
                ? _additionalInfoService.GetAllInfo()
                                  .Where(b => b.BranchId == branchId.Value)
                                  .Where(d => d.TransactionDate >= fromDay && d.TransactionDate <= toDay)
                                  .ToList()
                : new List<AdditionalInfo>();

            ViewBag.AdditionalInfo = additional ?? new List<AdditionalInfo>();

            #endregion

            ViewBag.FromDateText = fromDate?.ToString("dd/MM/yyyy");
            ViewBag.ToDateText = toDate?.ToString("dd/MM/yyyy");


            return View("PrintView", data);
        }



    }
}
