using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShirazTronic.Data;

namespace ShirazTronic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Utility.ManagerUser)]
    public class UserController : Controller
    {
        ApplicationDbContext db;
        public UserController(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var users = db.AppUser.Where(user => user.Id != claim.Value).ToList();
            return View(users);
        }
        public IActionResult Lock(string id)
        {
            if (id == null)
                return NotFound();
            var user = db.AppUser.FirstOrDefault(user => user.Id == id);
            if (user == null)
                return NotFound();
            user.LockoutEnd = DateTime.Now.AddYears(1000);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult UnLock(string id)
        {
            if (id == null)
                return NotFound();
            var user = db.AppUser.FirstOrDefault(user => user.Id == id);
            if (user == null)
                return NotFound();
            user.LockoutEnd = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
