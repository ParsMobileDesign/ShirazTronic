using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShirazTronic.Data;
using ShirazTronic.Models;
using ShirazTronic.Models.ViewModels;

namespace ShirazTronic.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        ApplicationDbContext db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _db)
        {
            db = _db;
            _logger = logger;
        }


        public IActionResult Index()
        {
            var count = 0;
            var user = U.getUserId(this);
            if (!string.IsNullOrEmpty(user))
               count= db.ShoppingCart.Where(e => e.AppUserId == user).ToList().Count;
            HttpContext.Session.SetInt32(U.ShoppingCartSession, count);
            var customerModel = new VmCustomerHomePage
            {
                Manufacturers = db.Manufacturer.ToList(),
                CompanyInfos = db.CompanyInfo.ToList()
            };
            return View(customerModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult getAsynci()
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
