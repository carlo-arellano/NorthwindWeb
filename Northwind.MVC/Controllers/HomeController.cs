using Microsoft.AspNetCore.Mvc;
using Northwind.DataContext;
using Northwind.MVC.Models;
using System.Diagnostics;

namespace Northwind.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly NorthwindMvcContext _db;

        public HomeController(
            ILogger<HomeController> logger,
            NorthwindMvcContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeIndexViewModel model = new
            (
                VisitorCount: Random.Shared.Next(1, 1001),
                Categories: _db.Categories.ToList(), Products: _db.Products.ToList()
            );
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
