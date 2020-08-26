using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShirazTronic.Models.ViewModels
{
    public class VmSpecAndSpecValue
    {
        public IEnumerable<Specification> Specifications { get; set; }
        public SpecificationValue SpecValue { get; set; }
    }
}
