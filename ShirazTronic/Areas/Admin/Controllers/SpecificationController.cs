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
    [Authorize(Roles = U.ManagerUser)]
    public class SpecificationController : Controller
    {
        ApplicationDbContext db;
        public SpecificationController(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            return View(db.Specification.ToList());
        }
        public IActionResult Create()
        {
            var Specification = new Specification();
            return View("Edit", Specification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(Specification spec)
        {
            if (ModelState.IsValid)
            {
                if (spec.Id == 0)
                    db.Specification.Add(spec);
                else
                    db.Specification.Update(spec);
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null)
                return NotFound();
            var spec = db.Specification.Find(Id);
            if (spec == null)
                return NotFound();
            return View(spec);

        }
        public IActionResult Delete(int? Id)
        {
            if (Id == null)
                return NotFound();
            var spec = db.Specification.Find(Id);
            if (spec == null)
                return NotFound();
            db.Specification.Remove(spec);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


    }
}
