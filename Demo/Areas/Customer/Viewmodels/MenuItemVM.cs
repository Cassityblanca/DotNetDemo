using Demo.Models;

namespace Demo.Areas.Customer.ViewModels
{
    public class MenuItemVM
    {
        public IEnumerable<Category> CategoryList { get; set; }
        public IEnumerable<MenuItem> MenuItemList { get; set; }
    }
}
