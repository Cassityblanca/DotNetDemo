using Demo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo.Areas.Admin.ViewModels
{
    public class MenuItemVM
    {
        public MenuItem? MenuItem { get; set; }

        //SelectListItem: uses ASP helper text
        //  A drop-down menu would have only shown 1, 2, 3, 4, 5 ect
        //  We can use the library to make this a key value pair, so we can change how it appears in dropdowns
        //      This is a part of the ASP rendering library
        //  You would EXPECT it to be IEnumerable<Category>
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        public IEnumerable<SelectListItem>? FoodTypeList { get; set; }
    }
}
