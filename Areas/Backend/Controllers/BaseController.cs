
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using mvc_andy.Data;

namespace mvc_andy.Controllers.Backend;

[Area("Backend")]
public class BaseController : Controller
{

    protected bool loginRequired = true;
    private readonly MvcAndyContext _context;

    public BaseController(MvcAndyContext context)
    {
        _context = context;
    }


    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        this._checkLogin(filterContext.HttpContext);
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
    }

    public void _checkLogin(HttpContext context)
    {
        if (context == null)
        {
            return;
        }
        int? uid = context.Session.GetInt32("UserInfo");
        if (uid == null || uid <= 0)
        {
            return;
        }
        /*
        var help = UserHelp.CreateObject();
        help.setDbContext(_context);
        currentUser = help.getSessionUser(uid);
        if (currentUser.Id != uid)
        {
            context.Response.Redirect("/Auth/Login");
        }
        ViewData["UserInfo"] = currentUser;
        */
    }

}