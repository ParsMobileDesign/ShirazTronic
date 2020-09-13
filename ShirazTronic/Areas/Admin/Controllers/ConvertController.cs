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
            int cat = 0, subCat = 0;
            IRange range = workSheet.Range;
            for (int i = 1; i <= range.Rows.Count() && workSheet.Range[i, 1].Text != null; i++)
            {
                try
                {
                    string title = workSheet.Range[i, 1].Text;
                    var inDb = db.Category.Where(e => e.Title == title).FirstOrDefault();
                    if (inDb == null)
                    {
                        Category category = new Category() { Title = workSheet.Range[i, 1].Text };
                        db.Category.Add(category);
                        db.SaveChanges();
                        cat++;
                        int j = 2;
                        while (workSheet.Range[i, j].Text != null)
                        {
                            SubCategory subCategory = new SubCategory
                            {
                                CategoryId = category.Id,
                                Title = workSheet.Range[i, j].Text
                            };
                            db.SubCategory.Add(subCategory);
                            subCat++;
                            j++;
                        }
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    return "Error in Converting Categories and SubCategories..." + e.Message;
                }
            }
            return "Converted Categories : " + cat + "-------- Converted Subcategories : " + subCat;
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
                    var inDb = db.Specification.Where(e => e.Title == title).FirstOrDefault();
                    if (inDb == null)
                    {

                        Specification spec = new Specification() { Title = workSheet.Range[i, 1].Text };
                        db.Specification.Add(spec);
                        db.SaveChanges();
                        specCount++;
                        int j = 2;
                        while (workSheet.Range[i, j].Text != null || !double.IsNaN(workSheet.Range[i, j].Number))
                        {
                            SpecificationValue specValue = new SpecificationValue
                            {
                                SpecificationId = spec.Id,
                                Value = workSheet.Range[i, j].Text != null ? workSheet.Range[i, j].Text : workSheet.Range[i, j].Number.ToString()
                            };
                            db.SpecificationValue.Add(specValue);
                            specValCount++;
                            j++;
                        }
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    return "Error in Converting Specifications and Specification values..." + e.Message;
                }
            }
            return "Converted Specifications : " + specCount + "-------- Converted Specification values : " + specValCount;
        }
        public string ProductInsertion(IWorksheet workSheet)
        {
            // int specCount = 0, specValCount = 0;
            IRange range = workSheet.Range;
            for (int i = 2; i <= range.Rows.Count() && workSheet.Range[i, 1].Text != null; i++)
            {
                try
                {
                    string title = workSheet.Range[i, 1].Text;
                    var inDb = db.Product.Where(e => e.Title == title).FirstOrDefault();
                    if (inDb == null)
                    {
                        Product product = new Product()
                        {
                            Title = workSheet.Range[i, 1].Text,
                            PartNumber = workSheet.Range[i, 1].Text,
                            Brand = workSheet.Range[i, 3].Text,
                            UnitPrice = decimal.Parse(workSheet.Range[i, 7].Number.ToString()),
                            BuyingPrice = decimal.Parse(workSheet.Range[i, 5].Number.ToString()),
                            StockQuantity = int.Parse(workSheet.Range[i, 4].Number.ToString())
                            //ShortDescription =??,
                            //FullDescription = "",
                        };
                        db.Product.Add(product);
                        db.SaveChanges();
                        string[] specs = workSheet.Range[i, 2].Text.Split(",");
                        for (int j = 0; j < specs.Length; j++)
                        {
                            var specValueInDB = db.SpecificationValue.FirstOrDefault(sv => sv.Value.ToUpper() == specs[j].ToUpper());
                            if (specValueInDB != null)
                            {
                                var productSpec = new ProductSpecification(product.Id, specValueInDB.Id);
                                db.ProductSpecification.Add(productSpec);

                            }

                        }
                        string[] productCategories = workSheet.Range[i, 8].Text.Split(",");
                        for (int j = 0; j < productCategories.Length; j++)
                        {
                            var subCatInDB = db.SubCategory.FirstOrDefault(sv => sv.Id == int.Parse(productCategories[j]));
                            if (subCatInDB != null)
                            {
                                var productCategory = new ProductCategory()
                                {
                                    CatId = subCatInDB.CategoryId,
                                    SubCatId = subCatInDB.Id,
                                    ProductId = product.Id
                                };
                                db.ProductCategory.Add(productCategory);

                            }

                        }
                        db.SaveChanges();

                        //Product Images

                        //int k = 9;
                        //Stream picture;
                        //IRange range1 = workSheet.Range[i, k];
                        //while (workSheet.Range[i, k].Value != null)
                        //{

                        //}
                        //string webRootPath = webHostEnvironment.WebRootPath;
                        //var files = HttpContext.Request.Form.Files;
                        //var productImageInDB = db.ProductImage.Find(productImage.Id);
                        //var filenameComplete = productImage.ProductId.ToString() + "_" + productImage.Id.ToString();
                        //productImageInDB.ImageAddr = Utility.SaveFileThenGetFileName(webHostEnvironment, "products", files, filenameComplete);
                    }
                }
                catch (Exception e)
                {
                    return "Error in Converting Specifications and Specification values..." + e.Message;
                }
            }
            return "Converted Specifications : ";// + specCount + "-------- Converted Specification values : " + specValCount;
        }
        public IActionResult ConvertData(ConvertExcel iConvert)
        {
            if (iConvert.MyImage != null)
            {
                Stream sampleFile = iConvert.MyImage.OpenReadStream();
                IWorkbook workbook = this.application.Workbooks.Open(sampleFile);
                IWorksheet worksheet = workbook.Worksheets[iConvert.EntityType];

                switch (iConvert.EntityType)
                {
                    case "Category":
                        iConvert.message = this.CategoryInsertion(worksheet);
                        break;
                    case "Specification":
                        iConvert.message = this.SpecificationInsertion(worksheet);
                        break;
                    case "Product":
                        iConvert.message = this.ProductInsertion(worksheet);
                        break;
                }
            }
            else
                iConvert.message = "File not Found";

            return View("Convert", iConvert);
        }
    }

}
