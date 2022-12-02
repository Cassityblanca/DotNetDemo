using Demo.Areas.Customer.ViewModels;
using Demo.Interfaces;
using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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

        public IActionResult Details(int menuItemId)
        {
            //TEMPORARY Shopping Cart - not saved to DB
            ShoppingCart cartObj = new()
            {
                Count = 1,
                MenuItemId = menuItemId,
                MenuItem = _unitOfWork.MenuItem.Get(m => m.Id == menuItemId, includes: "Category,FoodType"),
            };

            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;
            //checking to see if I already have this item in the shopping
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(
           u => u.ApplicationUserId == claim.Value && u.MenuItemId == shoppingCart.MenuItemId);

            if (cartFromDb == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Commit();
            }

            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
                _unitOfWork.Commit();
            }

            return RedirectToAction(nameof(Index));
        }


    }
}