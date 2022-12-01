
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using mvc_andy.Data;
using mvc_andy.Services.Backend;

namespace mvc_andy.Controllers.Backend;

[Area("Backend")]
public class BaseController : Controller
{

    protected readonly MvcAndyContext _context;

    public BaseController(MvcAndyContext context)
    {
        _context = context;
    }


    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        bool needLogin = this._checkLogin(filterContext.HttpContext);
        if (needLogin)
        {
            filterContext.Result = Redirect("/Manage/Auth/Login");
        }
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
    }

    public bool _checkLogin(HttpContext context)
    {
        if (context == null)
        {
            return false;
        }
        int? uid = context.Session.GetInt32("UserInfo");
        if (uid == null || uid <= 0)
        {
            return true;
        }
        var currentUser = UserService.CreateObject().setDbContext(_context).getSessionUser(uid);
        if (currentUser == null || currentUser.Id != uid)
        {
            // context.Response.Redirect("/Manage/Auth/Login");
            return true;
        }
        ViewData["UserInfo"] = currentUser;
        return false;
    }

}