using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Web.Models;

namespace BookStore.Web.Controllers;

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
