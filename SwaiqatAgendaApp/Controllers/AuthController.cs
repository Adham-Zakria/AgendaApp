using BusinessLogicLayer.Services;
using BusinessLogicLayer.ServicesAbstraction;
using Microsoft.AspNetCore.Mvc;

namespace SwaiqatAgendaApp.Controllers
{
    public class AuthController(IUsersService _usersService) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Login(string username, string password)
        //{
        //    var user = _usersService.ValidateLogin(username, password);

        //    if (user == null)
        //    {
        //        ViewBag.Error = "اسم المستخدم أو كلمة المرور غير صحيحة";
        //        return View();
        //    }

        //    // save the user's data in sessions 
        //    HttpContext.Session.SetInt32("UserId", user.UserId);
        //    HttpContext.Session.SetString("UserName", user.UserName);
        //    HttpContext.Session.SetInt32("BranchId", user.BranchId ?? 0);
        //    HttpContext.Session.SetString("GroupName", user.GroupId == 1 ? "Manager" : "Employee");

        //    if (user.GroupId == 1)
        //        return RedirectToAction("Index", "Manager");
        //    else
        //        return RedirectToAction("Index", "Transactions");
        //}

        [HttpPost]
        public IActionResult Login(int UserCode, string UserPassword)
        {
            try
            {
                var user = _usersService.Authenticate(UserCode, UserPassword);

                if (user == null)
                {
                    ViewBag.ErrorMessage = "كود الموظف أو كلمة المرور غير صحيحة.";
                    return View();
                }

                //save the user's data in sessions 
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetInt32("BranchId", user.BranchId ?? 0);
                HttpContext.Session.SetString("GroupName", user.GroupId == 1 ? "Manager" : "Employee");

                if (user.GroupId == 1)
                    return RedirectToAction("Index", "ManagerDashboard");
                else
                    return RedirectToAction("Index", "Transactions");
            }
            catch (Exception)
            {
                // in case NotFoundException
                ViewBag.ErrorMessage = "كود الموظف أو كلمة المرور غير صحيحة.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
