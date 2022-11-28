using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvc_andy.Models;

namespace mvc_andy.Controllers.Backend;

public class HomeController : BaseController
{

    public HomeController() : base()
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
