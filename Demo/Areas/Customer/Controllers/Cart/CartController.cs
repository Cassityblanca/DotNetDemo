using Demo.Areas.Customer.ViewModels;
using Demo.Interfaces;
using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
//using Stripe.Checkout;
using System.Security.Claims;


namespace Demo.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]


    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "MenuItem"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.MenuItem.Price = (float)GetPriceBasedOnQuantity(cart.Count, cart.MenuItem.Price);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.MenuItem.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        private static double GetPriceBasedOnQuantity(double quantity, double price)
        {
            if (quantity <= 5)
            {
                return price;
            }
            else
            {
                if (quantity <= 11)
                {
                    return price * .9;
                }
                return price * .8;
            }
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Commit();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Delete(cart);
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
            }
            _unitOfWork.Commit();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Delete(cart);
            _unitOfWork.Commit();
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "MenuItem"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(
                u => u.Id == claim.Value);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.FullName;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.MenuItem.Price = (float)GetPriceBasedOnQuantity(cart.Count, cart.MenuItem.Price);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.MenuItem.Price * cart.Count;

            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]  //used to map the "different" name of the action.
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost()  //Note we do not need to bring in ShoppingCartVM because we have BIND PROPERTY above, which binds all the form values in memory between the form's get and post events.
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //The Order is actually the shopping cart items, so we might as well use it

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "MenuItem");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = (float)GetPriceBasedOnQuantity(cart.Count, cart.MenuItem.Price);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.MenuItem.Price * cart.Count;

            }

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == claim.Value);

            ShoppingCartVM.OrderHeader.PaymentStatus = "PaymentStatusPending";
            ShoppingCartVM.OrderHeader.OrderStatus = "OrderStatusPending";

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Commit();

            //stripe settings 
            //var domain = "https://localhost:5001/";
            //var options = new SessionCreateOptions
            //{
            //    PaymentMethodTypes = new List<string>
            //        {
            //          "card",
            //        },
            //    LineItems = new List<SessionLineItemOptions>(),
            //    Mode = "payment",
            //    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
            //    CancelUrl = domain + $"customer/cart/index",
            //};

            //foreach (var item in ShoppingCartVM.ListCart)
            //{

            //    var sessionLineItem = new SessionLineItemOptions
            //    {
            //        PriceData = new SessionLineItemPriceDataOptions
            //        {
            //            UnitAmount = (long)(item.MenuItem.Price * 100),//20.00 -> 2000
            //            Currency = "usd",

            //            ProductData = new SessionLineItemPriceDataProductDataOptions
            //            {
            //                Name = item.MenuItem.Name

            //            },

            //        },
            //        Quantity = item.Count,



            //    };

            //    options.LineItems.Add(sessionLineItem);

            //}

            //var service = new Stripe.Checkout.SessionService();
            //Session session = service.Create(options);
            //_unitOfWork.ShoppingCart.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            //_unitOfWork.Commit();
            //Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);



        }


        //public IActionResult OrderConfirmation(int id)
        //{
        //    OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includes: "ApplicationUser");

        //    var service = new SessionService();
        //    Session session = service.Get(orderHeader.SessionId);
        //    //check the stripe status
        //    if (session.PaymentStatus.ToLower() == "paid")
        //    {
        //        _unitOfWork.OrderHeader.UpdateStatus(id, "OrderStatusApproved", "PaymentStatusApproved");
        //        _unitOfWork.Commit();
        //    }

        //    _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - CBTD", "<p>New Order Created</p>");
        //    //remove shopping cart
        //    List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId ==
        //    orderHeader.ApplicationUserId).ToList();
        //    _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
        //    _unitOfWork.Commit();
        //    return View(id);
        //}
    }
}
