using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShirazTronic.Models.ViewModels
{
    public class VmCustomerHomePage
    {
        public List<CompanyInfos> CompanyInfos { get; set; }
        public IEnumerable<Manufacturer> Manufacturers { get; set; }
    }
}
