using Microsoft.AspNetCore.Mvc;
using mvc_andy.Data;
using mvc_andy.Models.com;

namespace mvc_andy.Controllers.Backend;

[Area("Backend")]
public class AuthController : Controller
{

    protected readonly MvcAndyContext _context;
    public AuthController(MvcAndyContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Login()
    {
        int? uid = HttpContext.Session.GetInt32("UserInfo");
        if (uid != null || uid > 0)
        {
            return RedirectToAction(nameof(Index), "Home", new { area = "Backend" });
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([Bind("UserName,UserPass")] UserModel userModel)
    {

        if (ModelState.IsValid)
        {
            var user = from m in _context.UserModels
                       where m.Deleted.Equals(DeleteType.Enable)
                       where m.UserName.Equals(userModel.UserName)
                       select m;
            var modelTest = user.FirstOrDefault();
            if (modelTest == null)
            {
                ModelState.AddModelError("UserName", "账号不存在");
                return View(userModel);
            }
            // var userLog = new UserLog();
            // userLog.Type = LogType.Login;
            // userLog.UserId = modelTest.Id;
            // userLog.UserName = modelTest.UserName;
            // userLog.Message = "login system ";
            if (HttpContext != null)
            {
                // userLog.RequestUa = HttpContext.Request.Headers["User-Agent"];
                // if (HttpContext.Connection != null && HttpContext.Connection.RemoteIpAddress != null)
                // {
                //     userLog.Ip = HttpContext.Connection.RemoteIpAddress.ToString();
                // }

            }
            if (!BCrypt.Net.BCrypt.Verify(userModel.UserPass, modelTest.UserPass))
            {
                // userLog.Message += " =failed.密码错误";
                // UserLogHelp.Load().publish(userLog);
                ModelState.AddModelError("UserName", "账号或密码错误");
                return View(userModel);
            }
            if (modelTest.Status != StatusType.Enable)
            {
                // userLog.Message += " =failed.账号被禁用";
                // UserLogHelp.Load().publish(userLog);
                ModelState.AddModelError("UserName", "账号被禁用");
                return View(userModel);
            }
            //密码处理.
            modelTest.UpdateAt = DateTime.Now;
            _context.Update(modelTest);
            await _context.SaveChangesAsync();
            // userLog.Message += " success";
            // UserLogHelp.Load().publish(userLog);
            HttpContext.Session.SetInt32("UserInfo", modelTest.Id);
            return RedirectToAction(nameof(Index), "Home", new { area = "Backend" });
        }
        return View(userModel);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("UserInfo");
        return View();
    }

}
