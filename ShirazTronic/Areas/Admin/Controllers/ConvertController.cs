using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShirazTronic.Data;
using ShirazTronic.Models;
using Syncfusion.XlsIO;

namespace ShirazTronic.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class ConvertController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        ApplicationDbContext db;
        ExcelEngine excelEngine;
        IApplication application;
        public ConvertController(ApplicationDbContext _db, IWebHostEnvironment iWebHostEnvironment)
        {
            db = _db;
            webHostEnvironment = iWebHostEnvironment;

            this.excelEngine = new ExcelEngine();
            this.application = excelEngine.Excel;
            this.application.DefaultVersion = ExcelVersion.Excel2016;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            var con = new ConvertExcel();
            return View("Convert", con);
        }

        public string CategoryInsertion(IWorksheet workSheet)
        {
            int catCount = 0, subCatCount = 0;
            IRange range = workSheet.Range;
            for (int i = 1; i <= range.Rows.Count() && workSheet.Range[i, 1].Text != null; i++)
            {
                try
                {
                    string title = workSheet.Range[i, 1].Text;
                    var inDb = db.Category.Include(e => e.SubCategories).Where(e => e.Title == title).FirstOrDefault();
                    if (inDb == null)
                    {
                        Category category = new Category() { Title = workSheet.Range[i, 1].Text };
                        db.Category.Add(category);
                        inDb = category;
                        db.SaveChanges();
                        catCount++;
                    }
                    int j = 2;
                    while (workSheet.Range[i, j].Text != null)
                    {
                        string text = workSheet.Range[i, j].Text;
                        var subCatInDb = (inDb.SubCategories != null && inDb.SubCategories.Count > 0) ? inDb.SubCategories.FirstOrDefault(e => e.Title == text) : null;
                        if (subCatInDb == null)
                        {
                            SubCategory subCat = new SubCategory()
                            {
                                CategoryId = inDb.Id,
                                Title = text != null ? text : workSheet.Range[i, j].Number.ToString()
                            };
                            db.SubCategory.Add(subCat);
                            subCatCount++;
                        }
                        j++;
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return "Error in Converting Categories and SubCategories..." + e.Message;
                }
            }
            return "Converted Categories : " + catCount + "-- Converted Subcategories : " + subCatCount;
        }
        public string SpecificationInsertion(IWorksheet workSheet)
        {
            int specCount = 0, specValCount = 0;
            IRange range = workSheet.Range;
            for (int i = 1; i <= range.Rows.Count() && workSheet.Range[i, 1].Text != null; i++)
            {
                try
                {
                    string title = workSheet.Range[i, 1].Text;
                    var inDb = db.Specification.Include(e => e.SpecificationValues).Where(e => e.Title == title).FirstOrDefault();
                    if (inDb == null)
                    {
                        Specification spec = new Specification() { Title = workSheet.Range[i, 1].Text };
                        db.Specification.Add(spec);
                        db.SaveChanges();
                        inDb = spec;
                        specCount++;
                    }
                    int j = 2;
                    while (workSheet.Range[i, j].Text != null || !double.IsNaN(workSheet.Range[i, j].Number))
                    {
                        string text = workSheet.Range[i, j].Text != null ? workSheet.Range[i, j].Text : workSheet.Range[i, j].Number.ToString();
                        SpecificationValue specValue = new SpecificationValue();
                        //var valInDb = inDb.SpecificationValues.FirstOrDefault(e => e.Value == workSheet.Range[i, j].Text);
                        var valInDb = (inDb.SpecificationValues != null && inDb.SpecificationValues.Count() > 0) ? inDb.SpecificationValues.FirstOrDefault(e => e.Value == text) : null;
                        if (valInDb == null)
                        {
                            specValue = new SpecificationValue
                            {
                                SpecificationId = inDb.Id,
                                Value = text
                            };
                            db.SpecificationValue.Add(specValue);
                            specValCount++;
                        }
                        j++;
                    }
                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    return "Error in Converting Specifications and Specification values..." + e.Message;
                }
            }
            return "Converted Specifications : " + specCount + "-- Converted Specification values : " + specValCount;
        }
        public string ProductInsertion(IWorksheet workSheet)
        {
            int productInsertCount = 0, productUpdateCount = 0, specCount = 0, catCount = 0;
            IRange range = workSheet.Range;
            for (int i = 2; i <= range.Rows.Count() && workSheet.Range[i, 1].Text != null; i++)
            {
                try
                {
                    string title = workSheet.Range[i, 1].Text;
                    var inDb = db.Product.Include(e => e.ProductSpecification).Include(e => e.Categories).Where(e => e.Title == title).FirstOrDefault();
                    if (inDb == null)
                    {
                        Product product = new Product()
                        {
                            Title = workSheet.Range[i, 1].Text,
                            PartNumber = workSheet.Range[i, 1].Text,
                            Brand = workSheet.Range[i, 3].Text,
                            StockQuantity = int.Parse(workSheet.Range[i, 4].Number.ToString()),
                            BuyingPrice = decimal.Parse(workSheet.Range[i, 5].Number.ToString()),
                            UnitPrice = decimal.Parse(workSheet.Range[i, 7].Number.ToString()),
                            ShortDescription = string.IsNullOrEmpty(workSheet.Range[i, 9].Text) ? "" : workSheet.Range[i, 9].Text,
                            FullDescription = string.IsNullOrEmpty(workSheet.Range[i, 10].Text) ? "" : workSheet.Range[i, 10].Text,
                        };
                        db.Product.Add(product);
                        productInsertCount++;
                        inDb = product;
                    }
                    else
                    {
                        inDb.Brand = workSheet.Range[i, 3].Text;
                        inDb.UnitPrice = decimal.Parse(workSheet.Range[i, 7].Number.ToString());
                        inDb.BuyingPrice = decimal.Parse(workSheet.Range[i, 5].Number.ToString());
                        inDb.StockQuantity = int.Parse(workSheet.Range[i, 4].Number.ToString());
                        inDb.ShortDescription = string.IsNullOrEmpty(workSheet.Range[i, 9].Text) ? "" : workSheet.Range[i, 9].Text;
                        inDb.FullDescription = string.IsNullOrEmpty(workSheet.Range[i, 10].Text) ? "" : workSheet.Range[i, 10].Text;
                        db.Product.Update(inDb);
                        productUpdateCount++;
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                    string[] specs = workSheet.Range[i, 2].Text.Split(",");
                    for (int j = 0; j < specs.Length; j++)
                    {
                        var specValueInDB = db.SpecificationValue.FirstOrDefault(sv => sv.Value.ToUpper() == specs[j].ToUpper());
                        if (specValueInDB != null)
                        {
                            var valInDb = (inDb.ProductSpecification != null && inDb.ProductSpecification.Count() > 0) ? inDb.ProductSpecification.FirstOrDefault(e => e.SpecificationValueId == specValueInDB.Id) : null;
                            if (valInDb == null)
                            {
                                var productSpec = new ProductSpecification(inDb.Id, specValueInDB.Id);
                                db.ProductSpecification.Add(productSpec);
                                specCount++;
                            }
                        }

                    }
                    string[] productCategories = workSheet.Range[i, 8].Text.Split(",");
                    for (int j = 0; j < productCategories.Length; j++)
                    {
                        var subCatInDB = db.SubCategory.FirstOrDefault(sv => sv.Id == int.Parse(productCategories[j]));
                        if (subCatInDB != null)
                        {
                            var catInDb = (inDb.Categories != null && inDb.Categories.Count() > 0) ? inDb.Categories.FirstOrDefault(e => e.SubCatId == subCatInDB.Id) : null;
                            if (catInDb == null)
                            {
                                var productCat = new ProductCategory();
                                productCat.CatId = subCatInDB.CategoryId;
                                productCat.SubCatId = subCatInDB.Id;
                                productCat.ProductId = inDb.Id;
                                db.ProductCategory.Add(productCat);
                                catCount++;
                            }
                        }
                    }
                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    return "Error in Converting Specifications and Specification values..." + e.Message;
                }
            }
            return "Converted Product => Insertion :" + productInsertCount + "--Update :" + productUpdateCount + "--Product Category :" + catCount + "--Product Specification : " + specCount;
        }
        public IActionResult ConvertData(ConvertExcel iConvert)
        {
            if (iConvert.MyImage != null)
            {
                string ss = Request.Form["Category"].ToString();
                Stream sampleFile = iConvert.MyImage.OpenReadStream();
                IWorkbook workbook = this.application.Workbooks.Open(sampleFile);

                if (iConvert.isCategoryChecked)
                    iConvert.message = this.CategoryInsertion(workbook.Worksheets["Category"]);
                if (iConvert.isSpecChecked)
                    iConvert.message +="----------"+ this.SpecificationInsertion(workbook.Worksheets["Specification"]);
                if (iConvert.isProductChecked)
                    iConvert.message += "----------" + this.ProductInsertion(workbook.Worksheets["Product"]);
                //switch (iConvert.EntityType)
                //{
                //    case "Category":
                //        iConvert.message = this.CategoryInsertion(worksheet);
                //        break;
                //    case "Specification":
                //        iConvert.message = this.SpecificationInsertion(worksheet);
                //        break;
                //    case "Product":
                //        iConvert.message = this.ProductInsertion(worksheet);
                //        break;
                //}
            }
            else
                iConvert.message = "File not Found";

            return View("Convert", iConvert);
        }
    }

}
