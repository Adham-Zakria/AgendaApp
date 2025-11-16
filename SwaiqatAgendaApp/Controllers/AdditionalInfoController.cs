using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace SwaiqatAgendaApp.Controllers
{
    public class AdditionalInfoController(IAdditionalInfoService _additionalInfoService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddAdditionalInfo(AdditionalInfo model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Transactions");

            var branchId = HttpContext.Session.GetInt32("BranchId");

            model.BranchId = branchId.Value;
            model.TransactionDate = DateOnly.FromDateTime(DateTime.Now);

            _additionalInfoService.AddInfo(model);

            return RedirectToAction("Index", "Transactions");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var info = _additionalInfoService.GetById(id);
            if (info == null) return NotFound();

            return PartialView("_EditAdditionalInfoPartial", info);
        }

        [HttpPost]
        public IActionResult Edit(AdditionalInfo model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false });

            var username = HttpContext.Session.GetString("UserName") ?? "Unknown";

            _additionalInfoService.UpdateInfo(model, username);

            return Json(new { success = true, modifiedBy = username });
        }


    }
}
