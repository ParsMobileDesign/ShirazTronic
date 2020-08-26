using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShirazTronic.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        public IActionResult Index(int id)
        {
            return View(id);
        }
        public IActionResult SelProduct()
        {
            return View();
        }
    }
}
