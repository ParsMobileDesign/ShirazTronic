using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShirazTronic.Models.ViewModels
{
    public class VmCustomerProducts
    {
        public Category Category { get; set; }
        public SubCategory SubCategory { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
