using MersadBerberArt.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Web;

namespace MersadBerberArt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.ShowCookiePreferenceNotice = true;
            if (Request.Cookies["CookiePreference"] != null)
            {
                string cookieValue = Request.Cookies["CookiePreference"];
                if (cookieValue == "Accepted")
                {

                }
                else if (cookieValue == "Rejected")
                {

                }

                ViewBag.ShowCookiePreferenceNotice = false;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult CookiePolicy()
        {
            return View();
        }

        public IActionResult AcceptCookies()
        {
            var options = new CookieOptions { Expires = DateTime.Now.AddDays(5) };
            Response.Cookies.Append("CookiePreference", "Accepted", options);

            return Json("Cookies accepted successfully.");
        }

        public IActionResult RejectCookies()
        {
            var options = new CookieOptions { Expires = DateTime.Now.AddDays(5) };
            Response.Cookies.Append("CookiePreference", "Rejected", options);

            return Json("Cookies rejected successfully.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
