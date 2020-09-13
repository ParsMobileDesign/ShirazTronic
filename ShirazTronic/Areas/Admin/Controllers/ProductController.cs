using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShirazTronic.Data;
using ShirazTronic.Models;
using ShirazTronic.Models.ViewModels;
namespace ShirazTronic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Utility.ManagerUser)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        [BindProperty]
        public VmProduct productVm { get; set; }
        public VmProductCategories catProductVm;
        public Product Product;

        [BindProperty]
        public ProductImage productImage { get; set; }
        public ProductController(ApplicationDbContext _db, IWebHostEnvironment iWebHostEnvironment)
        {
            db = _db;
            webHostEnvironment = iWebHostEnvironment;

            productVm = new VmProduct()
            {
                Product = new Product(),
                CatProducts = new List<ProductCategory>(),
                ProductImages = new List<ProductImage>(),
                Specifications = db.Specification.ToList(),
                
            };
            productImage = new ProductImage();
        }
        // GET: ProductController
        public IActionResult Index()
        {
            var products = db.Product.ToList();
            return View(products);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            return View("Edit", productVm);
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save()
        {
            if (!ModelState.IsValid)
                return View(productVm);
            try
            {
                if (productVm.Product.Id == 0)
                    db.Product.Add(productVm.Product);
                else
                    db.Product.Update(productVm.Product);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCategoryProduct(VmProductCategories iCatProduct)
        {
            if (!ModelState.IsValid)
                return View("EditProductCat", iCatProduct);
            try
            {
                if (iCatProduct.CatProduct.Id == 0)
                    db.ProductCategory.Add(iCatProduct.CatProduct);
                else
                    db.ProductCategory.Update(iCatProduct.CatProduct);
                db.SaveChanges();
                return RedirectToAction(nameof(Edit), new { id = iCatProduct.CatProduct.ProductId });
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveProductImage(int ProductImageIdentifi)
        {
            if (!ModelState.IsValid)
                return View("EditProductImage", productImage);
            try
            {
                if (productImage.Id == 0)
                    db.ProductImage.Add(productImage);
                else
                    db.ProductImage.Update(productImage);
                db.SaveChanges();
                //Saving Image 
                string webRootPath = webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                var productImageInDB = db.ProductImage.Find(productImage.Id);
                var filenameComplete = productImage.ProductId.ToString() + "_" + productImage.Id.ToString();
                productImageInDB.ImageAddr = Utility.SaveFileThenGetFileName(webHostEnvironment, "products", files, filenameComplete);
                db.SaveChanges();

                return RedirectToAction(nameof(Edit), new { id = productImage.ProductId });
            }
            catch (Exception e)
            {
                return View();
            }
        }
        public IActionResult newProductCategory(int id)
        {
            var tempProduct = db.Product.SingleOrDefault(e => e.Id == id);
            if (tempProduct == null)
                return NotFound();
            var catProVm = new VmProductCategories
            {
                CatProduct = new ProductCategory(tempProduct),
                Categories = db.Category
            };
            return View("EditProductCat", catProVm);
        }
        public IActionResult newProductImage(int id)
        {
            var tempProduct = db.Product.SingleOrDefault(e => e.Id == id);
            if (tempProduct == null)
                return NotFound();
            var proImage = new ProductImage(tempProduct);
            return View("EditProductImage", proImage);
        }
        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            if (id > 0)
            {
                var product = db.Product.SingleOrDefault(x => x.Id == id);
                if (product == null)
                    return NotFound();
                productVm.Product = this.Product = product;
                productVm.CatProducts = db.ProductCategory.Include(e => e.Category).Include(e => e.SubCategory).Where(e => e.ProductId == product.Id).ToList();
                productVm.ProductImages = db.ProductImage.Where(e => e.ProductId == product.Id).ToList();
                productVm.ProductSpecs = db.ProductSpecification.Include(e => e.SpecificationValue).OrderBy(e => e.SpecificationValue.Specification.DisplayOrder).Where(e => e.ProductId == product.Id).ToList();
                productVm.Specifications = db.Specification.ToList();
                return View(productVm);
            }
            else
                return View();
        }



        // GET: ProductController/Delete/5
        public ActionResult DeleteProductImage(int id, int ProductId)
        {
            var ProductImage = db.ProductImage.SingleOrDefault(e => e.Id == id);
            if (ProductImage != null)
            {
                db.ProductImage.Remove(ProductImage);
                db.SaveChanges();
                string webroot = webHostEnvironment.WebRootPath;
                System.IO.File.Delete(webroot + ProductImage.ImageAddr);
            }
            return RedirectToAction(nameof(Edit), new { id = ProductId });
        }
        public ActionResult DeleteProductCategory(int id, int ProductId)
        {
            var ProductCategory = db.ProductCategory.SingleOrDefault(e => e.Id == id);
            if (ProductCategory != null)
            {
                db.ProductCategory.Remove(ProductCategory);
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Edit), new { id = ProductId });
        }

        [ActionName("AddProductSpec")]
        public ActionResult addProductSpecification(int productId, int specValId)
        {
            var ProductCategory = db.ProductSpecification.SingleOrDefault(e => e.ProductId == productId && e.SpecificationValueId == specValId);
            if (ProductCategory == null)
            {
                var productSpec = new ProductSpecification(productId, specValId);
                db.ProductSpecification.Add(productSpec);
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Edit), new { id = productId });
        }

        public ActionResult DeleteProductSpec(int id)
        {
            var ProductSpec = db.ProductSpecification.SingleOrDefault(e => e.Id == id);
            if (ProductSpec != null)
            {
                db.ProductSpecification.Remove(ProductSpec);
                db.SaveChanges();
            }
            else
                return NotFound();
            return RedirectToAction(nameof(Edit), new { id = ProductSpec.ProductId });
        }
        // POST: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            if (id > 0)
            {
                var product = db.Product.Find(id);
                if (product != null)
                {
                    try
                    {
                        db.Product.Remove(product);
                        db.SaveChanges();
                    }
                    catch(Exception e)
                    {
                    }

                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
