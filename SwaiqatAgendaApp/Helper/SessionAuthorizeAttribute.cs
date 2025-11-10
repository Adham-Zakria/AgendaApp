using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SwaiqatAgendaApp.Helper
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    var session = context.HttpContext.Session;
        //    var userId = session.GetInt32("UserId");

        //    // if no user in session, redirect to login
        //    if (userId == null)
        //    {
        //        context.Result = new RedirectToActionResult("Login", "Auth", null);
        //        return;
        //    }

        //    base.OnActionExecuting(context);
        //}

        private readonly bool _requireAdmin;

        // parameter to specify if admin access is required
        public SessionAuthorizeAttribute(bool requireAdmin = false)
        {
            _requireAdmin = requireAdmin;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetInt32("UserId");
            var groupId = session.GetInt32("GroupId");
            var groupName = session.GetString("GroupName");


            // if no user in session, redirect to login
            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // Manager-only access check
            //if (_requireAdmin && (groupId == null || groupId != 1))
            if(_requireAdmin && !((groupId == 1) || (groupName == "Manager")))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
