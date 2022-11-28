using Microsoft.AspNetCore.Mvc;
using mvc_andy.Data;

namespace mvc_andy.Controllers.Backend;

public class HomeController : BaseController
{

    public HomeController(MvcAndyContext context) : base(context)
    {
    }

    public IActionResult Index()
    {
        return View();
    }

    
    public IActionResult Info()
    {
        return View();
    }
    
}
