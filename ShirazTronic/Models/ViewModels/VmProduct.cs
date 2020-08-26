using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShirazTronic.Models.ViewModels
{
    public class VmProduct
    {
        public Product Product { get; set; }
        public List<ProductCategory> CatProducts { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public ICollection<ProductSpecification> ProductSpecs { get; set; }
        public IEnumerable<Specification> Specifications { get; set; }
    }
}
