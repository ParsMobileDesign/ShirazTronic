using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShirazTronic.Models.ViewModels
{
    public class VmShoppingCart
    {
        public IEnumerable<ShoppingCart> ShoppingCartItems { get; set; }
        public MemOrder MemOrder { get; set; }
    }
}
