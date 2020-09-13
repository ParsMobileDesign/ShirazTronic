using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShirazTronic.Data;
using ShirazTronic.Extensions;
using ShirazTronic.Models;
using ShirazTronic.Models.ViewModels;

namespace ShirazTronic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Utility.ManagerUser)]
    public class SubCategoryController : Controller
    {
        ApplicationDbContext db;
        [TempData]
        public string tempMessage { get; set; }
        public SubCategoryController(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var subCategories = db.SubCategory.Include(s => s.Category).ToList();
            return View(subCategories);
        }
        public async Task<IActionResult> Create()
        {
            var subCategoryAndCategory = new VmSubcategoryAndCategory()
            {
                Categories = await db.Category.ToListAsync(),
                Specifications = await db.Specification.ToListAsync(),
                SubCategory = new SubCategory(),
                // subCategoryList = await db.SubCategory.OrderBy(p => p.Title).Select(p => p.Title).Distinct().ToListAsync()
            };

            return View("Edit", subCategoryAndCategory);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(List<IFormFile> Picture, VmSubcategoryAndCategory model)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<SubCategory> isExistSubCat = null;
                if (model.SubCategory.Id == 0) // run when object is new 
                    isExistSubCat = db.SubCategory.Include(s => s.Category).Where(s => s.Title == model.SubCategory.Title && s.Category.Id == model.SubCategory.CategoryId);
                if (isExistSubCat != null && isExistSubCat.Count() > 0)
                {
                    if (isExistSubCat.Count() > 0)
                        tempMessage = "Subcategory already exists";
                }
                else
                {
                    using (var ms = new MemoryStream())
                    {
                        if (Picture.Count > 0)
                        {
                            Picture[0].CopyTo(ms);
                            model.SubCategory.Picture = ms.ToArray();
                        }

                    }
                    if (model.SubCategory.Id == 0)
                        db.SubCategory.Add(model.SubCategory);
                    else
                        db.SubCategory.Update(model.SubCategory);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            VmSubcategoryAndCategory tempModel = new VmSubcategoryAndCategory()
            {
                Categories = db.Category.ToList(),
                SubCategory = model.SubCategory,
                statusMessage = tempMessage,
                //subCategoryList = db.SubCategory.OrderBy(p => p.Title).Select(s => s.Title).ToList()
            };
            return View("Edit", tempModel);
        }

        public IActionResult Delete(int id)
        {
            var subCat = db.SubCategory.SingleOrDefault(s => s.Id == id);
            if (subCat == null)
                return NotFound();
            db.SubCategory.Remove(subCat);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Edit(int id)
        {
            var subCat = db.SubCategory.SingleOrDefault(x => x.Id == id);
            if (subCat == null)
                return NotFound();

            var subCategoryAndCategory = new VmSubcategoryAndCategory()
            {
                Categories = db.Category.ToList(),
                SubCategory = subCat,
                Specifications = db.Specification.ToList(),
                SubCatSpecifications = db.SubCatSpecification.ToList()
                //subCategoryList = await db.SubCategory.OrderBy(p => p.Title).Select(p => p.Title).Distinct().ToListAsync()
            };
            return View(subCategoryAndCategory);
        }
        public IActionResult AddSubCatSpec(int SubCategoryId, int SpecificationId)
        {
            if (SubCategoryId > 0 && SpecificationId > 0)
            {
                var temp = db.SubCatSpecification.FirstOrDefault(x => x.SubCategoryId == SubCategoryId && x.SpecificationId == SpecificationId);
                if (temp == null)
                {
                    var subcatspec = new SubCatSpecification(SubCategoryId, SpecificationId);
                    db.SubCatSpecification.Add(subcatspec);
                    db.SaveChanges();
                }
            }
            return serializeList(SubCategoryId);
        }
        public IActionResult DelSubCatSpec(int Id, int SubCatId)
        {
            if (Id > 0)
            {
                var temp = db.SubCatSpecification.FirstOrDefault(x => x.Id == Id);
                if (temp != null)
                {
                    db.SubCatSpecification.Remove(temp);
                    db.SaveChanges();
                }
            }

            return serializeList(SubCatId);
        }
        private ContentResult serializeList(int iSubCatId)
        {
            var subCatSpec = db.SubCatSpecification.Include(e => e.Specification).Where(x => x.SubCategoryId == iSubCatId).ToList();
            string json = "[";
            string sep = "";
            foreach (SubCatSpecification item in subCatSpec)
            {
                json += sep + JsonConvert.SerializeObject(new { id = item.Id, SubCatId = item.SubCategoryId, SpecId = item.Specification.Id, SpecTitle = item.Specification.Title });
                sep = ",";
            }
            json += "]";
            return Content(json);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(int id, VmSubcategoryAndCategory model, List<IFormFile> Picture)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var subCatinDB = db.SubCategory.SingleOrDefault(s => s.Id == id);
        //        using (var ms = new MemoryStream())
        //        {
        //            if (Picture.Count > 0)
        //            {
        //                Picture[0].CopyTo(ms);
        //                subCatinDB.Picture = ms.ToArray();
        //            }

        //        }
        //        subCatinDB.Title = model.SubCategory.Title;
        //        db.SaveChanges();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    VmSubcategoryAndCategory tempModel = new VmSubcategoryAndCategory()
        //    {
        //        Categories = db.Category.ToList(),
        //        SubCategory = model.SubCategory,
        //        statusMessage = "Subcategory already exists",
        //        //subCategoryList = db.SubCategory.OrderBy(p => p.Title).Select(s => s.Title).ToList()
        //    };
        //    return View(tempModel);
        //}

        [ActionName("GetSubCategory")]
        public IActionResult GetSubCategory(int id)
        {
            List<SubCategory> subCats = new List<SubCategory>();
            subCats = (from subCategory in db.SubCategory
                       where subCategory.CategoryId == id
                       select subCategory).ToList();
            return Json(new SelectList(subCats, "Id", "Title"));


        }
    }
}
