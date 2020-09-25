using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueryDesignerCore;
using ShirazTronic.Data;
using ShirazTronic.Models;
using ShirazTronic.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ShirazTronic.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        ApplicationDbContext db;
        IEnumerable<Product> productsToFilter;
        public ProductController(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IActionResult Index(int catId, int subCatId)
        {
            Category cat = null;
            SubCategory subcat = null;
            IEnumerable<Product> products = null;


            if (catId > 0)
                cat = db.Category.Include(e => e.SubCategories).FirstOrDefault(e => e.Id == catId);
            if (subCatId > 0)
            {
                products = db.ProductCategory.Include(e => e.Product).ThenInclude(product => product.Images).Include(product => product.Product.ProductSpecification).Where(e => e.SubCatId == subCatId).Select(p => p.Product).ToList();
                subcat = db.SubCategory.Include(e => e.SubCatSpecifications).ThenInclude(e => e.Specification).ThenInclude(s => s.SpecificationValues).FirstOrDefault(e => e.Id == subCatId);
                HttpContext.Session.SetInt32(U.SubCategoryIdSession, subCatId);
            }

            var customerProducts = new VmCustomerProducts()
            {
                Category = cat,
                SubCategory = subcat,
                Products = products
            };

            return View(customerProducts);
        }

        [Authorize]
        [ActionName("AddToCart")]
        public ActionResult AddToCart(int ProductId)
        {
            var tempShopCart = new ShoppingCart();
            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);
            //if(claim==null)
            tempShopCart.AppUserId = claim.Value;
            tempShopCart.ProductId = ProductId;

            var shopCartInDB = db.ShoppingCart.Where(sc => sc.AppUserId == claim.Value && sc.ProductId == ProductId).FirstOrDefault();
            if (shopCartInDB == null)
                db.ShoppingCart.Add(tempShopCart);
            else
                shopCartInDB.Count += 1;
            db.SaveChanges();

            var shoppingCartItems = db.ShoppingCart.Where(sc => sc.AppUserId == claim.Value).Count();
            HttpContext.Session.SetInt32(U.ShoppingCartSession, shoppingCartItems);

            return PartialView("_ShoppingCartPartial", shoppingCartItems);
        }
        public IActionResult SelProduct(int? pId)
        {
            var product = db.Product.Include(e => e.Images).Include(e => e.ProductSpecification).ThenInclude(e => e.SpecificationValue).ThenInclude(e => e.Specification).FirstOrDefault(e => e.Id == pId);
            if (product == null)
                return NotFound();
            return View(product);

        }
        [HttpGet]
        public async Task<IActionResult> FilterAp(string jsonQuery)
        {
            var objArray = jsonQuery.Split("-");
            int subCatId = (int)HttpContext.Session.GetInt32(U.SubCategoryIdSession);
            productsToFilter = await db.Product.Include(p => p.Images).Include(p => p.ProductSpecification).ThenInclude(ps => ps.SpecificationValue).Where(e => e.Categories.Any(pc => pc.SubCatId == subCatId)).ToListAsync();
            for (int i = 0; i < objArray.Length; i++)
            {
                string[] idArray = objArray[i].Split(",");
                productsToFilter = productsToFilter.Where(p => p.ProductSpecification.Any(ps => idArray.Contains(ps.SpecificationValueId.ToString()))).ToList();
            }
            IEnumerable<Product> temp3 = productsToFilter;
            return PartialView("_ProductPartial", temp3);
        }
        public async Task<IActionResult> resetAp()
        {
            int subCatid = (int)HttpContext.Session.GetInt32(U.SubCategoryIdSession);
            IEnumerable<Product> products = await db.Product.Include(p => p.Images).Include(p => p.ProductSpecification).ThenInclude(ps => ps.SpecificationValue).Where(e => e.Categories.Any(pc => pc.SubCatId == subCatid)).ToListAsync();
            return PartialView("_ProductPartial", products);
        }
    }
}
