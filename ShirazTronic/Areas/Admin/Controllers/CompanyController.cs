using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShirazTronic.Data;
using ShirazTronic.Models;

namespace ShirazTronic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        ApplicationDbContext db;
        public CompanyController(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            return View(db.CompanyInfo.ToList());
        }
        public IActionResult Create()
        {
            var Com = new CompanyInfos();
            return View("Edit", Com);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(CompanyInfos com)
        {
            if (ModelState.IsValid)
            {
                if (com.Id == 0)
                    db.CompanyInfo.Add(com);
                else
                    db.CompanyInfo.Update(com);
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(byte? Id)
        {
            if (Id == null)
                return NotFound();
            var com = db.CompanyInfo.Find(Id);
            if (com == null)
                return NotFound();
            return View(com);

        }


    }
}
