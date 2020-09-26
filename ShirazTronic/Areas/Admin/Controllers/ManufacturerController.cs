using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShirazTronic.Data;
using ShirazTronic.Models;

namespace ShirazTronic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManufacturerController : Controller
    {
        ApplicationDbContext db;
        public ManufacturerController(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            return View(db.Manufacturer.ToList());
        }
        public IActionResult Create()
        {
            var Manufacturer = new Manufacturer();
            return View("Edit", Manufacturer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(Manufacturer Manufacturer, List<IFormFile> Picture)
        {
            var files = HttpContext.Request.Form.Files;
            if (ModelState.IsValid)
            {
                if (files.Count > 0)
                    using (var ms = new MemoryStream())
                    {
                        files[0].CopyTo(ms);
                        Manufacturer.Logo = ms.ToArray();
                    }
                if (Manufacturer.Id == 0)
                    db.Manufacturer.Add(Manufacturer);
                else
                    db.Manufacturer.Update(Manufacturer);
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null)
                return NotFound();
            var Manufacturer = db.Manufacturer.Find(Id);
            if (Manufacturer == null)
                return NotFound();
            return View(Manufacturer);

        }
        public IActionResult Delete(int? Id)
        {
            if (Id == null)
                return NotFound();
            var Manufacturer = db.Manufacturer.Find(Id);
            if (Manufacturer == null)
                return NotFound();
            db.Manufacturer.Remove(Manufacturer);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}
