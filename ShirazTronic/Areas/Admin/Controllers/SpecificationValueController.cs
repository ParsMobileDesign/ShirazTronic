using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShirazTronic.Data;
using ShirazTronic.Extensions;
using ShirazTronic.Models;
using ShirazTronic.Models.ViewModels;

namespace ShirazTronic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SpecificationValueController : Controller
    {
        ApplicationDbContext db;
        [TempData]
        public string tempMessage { get; set; }
        public SpecificationValueController(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var specValues = db.SpecificationValue.Include(s => s.Specification).ToList();
            return View(specValues);
        }
        public async Task<IActionResult> Create()
        {
            var SpecAndSpecValues = new VmSpecAndSpecValue()
            {
                Specifications = await db.Specification.ToListAsync(),
                SpecValue = new SpecificationValue()
            };

            return View("Edit", SpecAndSpecValues);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(VmSpecAndSpecValue model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.SpecificationValue.Add(model.SpecValue);
        //        db.SaveChanges();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    VmSpecAndSpecValue tempModel = new VmSpecAndSpecValue()
        //    {
        //        Specifications = db.Specification.ToList(),
        //        SpecValue = model.SpecValue,
        //    };
        //    return View("Edit",tempModel);
        //}


        public IActionResult Edit(int id)
        {
            var specVal = db.SpecificationValue.SingleOrDefault(x => x.Id == id);
            if (specVal == null)
                return NotFound();

            var SpecAndSpecValue = new VmSpecAndSpecValue()
            {
                Specifications = db.Specification.ToList(),
                SpecValue = specVal,
            };

            return View(SpecAndSpecValue);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, VmSpecAndSpecValue model)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                    db.SpecificationValue.Add(model.SpecValue);
                else 
                    db.SpecificationValue.Update(model.SpecValue);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            var tempModel = new VmSpecAndSpecValue()
            {
                Specifications = db.Specification.ToList(),
                SpecValue = model.SpecValue,
            };
            return View(tempModel);
        }
        public IActionResult Delete(int id)
        {
            var specVal = db.SpecificationValue.SingleOrDefault(s => s.Id == id);
            if (specVal == null)
                return NotFound();
            try
            {
                db.SpecificationValue.Remove(specVal);
                db.SaveChanges();
            }
            catch(Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
           
            return RedirectToAction(nameof(Index));

        }
        [ActionName("GetSpecVal")]
        public IActionResult GetSpecificationValue(int id)
        {
            List<SpecificationValue> specVals = new List<SpecificationValue>();
            specVals = (from specVal in db.SpecificationValue
                        where specVal.SpecificationId == id
                        select specVal).ToList();
            return Json(new SelectList(specVals, "Id", "Value"));


        }
    }
}
