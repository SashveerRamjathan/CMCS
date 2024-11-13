using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["HideSidebar"] = true;
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["HideSidebar"] = true;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
