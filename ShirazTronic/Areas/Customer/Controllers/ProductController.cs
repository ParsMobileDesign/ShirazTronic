using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShirazTronic.Data;
using ShirazTronic.Models;
using ShirazTronic.Models.ViewModels;

namespace ShirazTronic.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        ApplicationDbContext db;
        public ProductController(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IActionResult Index(int? catId,int? subCatId)
        {
            Category cat=null;
            IEnumerable<ProductCategory> products = null;
            if (catId != null && catId > 0)
                cat = db.Category.Include(e => e.SubCategories).FirstOrDefault(e => e.Id == catId);
            if (subCatId != null && subCatId > 0)
                products = db.ProductCategory.Include(e=>e.Product).Include(e=>e.Product.Images).Include(e=>e.Product.ProductSpecification).Where(e => e.SubCatId == subCatId).ToList();
            var customerProducts = new VmCustomerProducts()
            {
                Category = cat,
                Products=products
            };
            return View(customerProducts);
        }
        public IActionResult SelProduct(int? pId)
        {
            var product=db.Product.Include(e=>e.Images).Include(e=>e.ProductSpecification).ThenInclude(e=>e.SpecificationValue).ThenInclude(e=>e.Specification).FirstOrDefault(e=>e.Id==pId);
            if (product == null)
                return NotFound();
            return View(product);
        }
    }
}
