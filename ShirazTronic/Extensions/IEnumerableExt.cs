using Microsoft.AspNetCore.Mvc.Rendering;
using ShirazTronic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShirazTronic.Extensions
{
    public static class IEnumerableExt
    {
        public static IEnumerable<SelectListItem> GetSelectListItems<T>(this IEnumerable<T> items, int selectedValue)
        {
            IEnumerable<SelectListItem> s = from item in items
                                            select new SelectListItem
                                            {
                                                Text = item.GetPropertyValue("Title"),
                                                Value = item.GetPropertyValue("Id"),
                                                Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                                            };
            return s;
        }
        //public static bool HasSpecification<T>(this ICollection<T> items, string[] Values)
        //{
        //    if (items.GetType() is ICollection<ProductSpecification>)
        //    {
        //        foreach (var item in items)
        //        {
        //            int temp = (int)item.GetType().GetProperty("SpecificationValueId").GetValue(item, null);
        //            if (Values.Contains(temp.ToString()))
        //                return true;
        //        }
        //    }
        //    return false;
        //}

    }
}
