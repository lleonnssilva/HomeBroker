using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HomeBroker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace HomeBroker.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;


    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;

    }

    public IActionResult Index()
    {
       
        return View();
    }
 
    public IActionResult Goodbye()
    {

        return View();
    }
    

    [Authorize(Policy = "Admin")]
    public IActionResult Admin()
    {
        var user = User.Identity;
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string message)
    {
        return View(new ErrorViewModel {Message=message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
