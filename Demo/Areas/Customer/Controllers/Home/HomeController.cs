using Demo.Areas.Customer.ViewModels;
using Demo.Interfaces;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Demo.Areas.Customer.Controllers.Home
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)//Dependency Injection
        {
            _unitOfWork = unitOfWork;

        }



        public MenuItemVM MenuItemObj { get; set; }
        public ViewResult Index()
        {
            MenuItemObj = new MenuItemVM
            {
                MenuItemList = _unitOfWork.MenuItem.List(null, null, "Category,FoodType"),
                CategoryList = _unitOfWork.Category.List(null, c => c.DisplayOrder, null)
            };

            return View(MenuItemObj);
        }

    }
}