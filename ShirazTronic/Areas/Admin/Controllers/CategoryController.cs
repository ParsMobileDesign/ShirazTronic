using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShirazTronic.Data;
using ShirazTronic.Models;

namespace ShirazTronic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Utility.ManagerUser)]
    public class CategoryController : Controller
    {
        ApplicationDbContext db;
        public CategoryController(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            return View(db.Category.ToList());
        }
        public IActionResult Create()
        {
            var category = new Category();
            return View("Edit", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(Category category, List<IFormFile> Picture)
        {
            if (ModelState.IsValid)
            {

                using (var ms = new MemoryStream())
                {
                    Picture[0].CopyTo(ms);
                    category.Picture = ms.ToArray();
                }
                if (category.Id == 0)
                    db.Category.Add(category);
                else
                    db.Category.Update(category);
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null)
                return NotFound();
            var category = db.Category.Find(Id);
            if (category == null)
                return NotFound();
            return View(category);

        }
        public IActionResult Delete(int? Id)
        {
            if (Id == null)
                return NotFound();
            var category = db.Category.Find(Id);
            if (category == null)
                return NotFound();
            db.Category.Remove(category);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

    }
}
