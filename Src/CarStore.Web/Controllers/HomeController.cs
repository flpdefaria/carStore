using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CarStore.Web.Models;

namespace CarStore.Web.Controllers;

public class HomeController : Controller
{
    // Serves the single SPA shell page; Vue Router owns every other client-side route.
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
