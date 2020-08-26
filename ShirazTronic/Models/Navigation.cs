using Microsoft.AspNetCore.Mvc;
using ShirazTronic.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ShirazTronic.Models
{
    public class NavigationViewComponent : ViewComponent
    {
        ApplicationDbContext db;
        public NavigationViewComponent(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IViewComponentResult Invoke()
        {
            List<Category> cats = db.Category.Include(e=>e.SubCategories).ToList();
            return View(cats);
        }
    }
}
