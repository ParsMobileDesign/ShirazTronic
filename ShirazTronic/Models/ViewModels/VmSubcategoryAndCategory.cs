using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShirazTronic.Models.ViewModels
{
    public class VmSubcategoryAndCategory
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Specification> Specifications { get; set; }
        public IEnumerable<SubCatSpecification> SubCatSpecifications { get; set; }

        //public List<string> subCategoryList{ get; set; }
        public SubCategory SubCategory { get; set; }
        public IFormFile Picture{ get; set; }
        public string statusMessage { get; set; }
    }
}
