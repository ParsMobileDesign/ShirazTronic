using Microsoft.AspNetCore.Mvc;
using ShirazTronic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShirazTronic.Models
{
    public class TopBarViewComponent : ViewComponent
    {
        ApplicationDbContext db;
        public TopBarViewComponent(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IViewComponentResult Invoke()
        {
            var companyInfo = db.CompanyInfo.Count() > 0 ? db.CompanyInfo.ToList() : null;
            return View(companyInfo);
        }
    }

}
