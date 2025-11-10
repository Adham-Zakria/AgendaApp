using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwaiqatAgendaApp.Helper;

namespace SwaiqatAgendaApp.Controllers
{
    //[Authorize]
    //[SessionAuthorize]
    [SessionAuthorize(requireAdmin: true)]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IBranchService _branchService;

        public UsersController(IUsersService usersService, IBranchService branchService)
        {
            _usersService = usersService;
            _branchService = branchService;
        }

        [HttpGet]
        public IActionResult Add()
        {
            // branches loading
            ViewBag.Branches = _branchService.GetAllBranches();
            return View();
        }

        //[HttpPost]
        //public IActionResult Add(User model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.Branches = _branchService.GetAllBranches();
        //        return View(model);
        //    }

        //    _usersService.AddUser(model);
        //    TempData["SuccessMessage"] = "تم إضافة الموظف بنجاح.";
        //    return RedirectToAction("Add");
        //}
        [HttpPost]
        public IActionResult Add(User model)
        {
            try
            {
                // branches loading
                ViewBag.Branches = _branchService.GetAllBranches();

                if (!ModelState.IsValid) 
                    return View(model);

                // checking for existing user code
                var existingUser = _usersService.GetAllUsers()
                    .FirstOrDefault(u => u.UserId == model.UserId);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", " كود الموظف موجود بالفعل. يرجى اختيار كود آخر.");
                    return View(model);
                }

                _usersService.AddUser(model);

                TempData["SuccessMessage"] = " تم إضافة الموظف بنجاح.";
                return RedirectToAction("Add");
            }
            catch (Exception ex)
            {
                // in case of any error
                ModelState.AddModelError("", "حدث خطأ أثناء إضافة الموظف: " + ex.Message);
                return View(model);
            }
        }

    }
}
