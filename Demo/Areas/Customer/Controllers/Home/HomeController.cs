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

        public ViewResult Index()
        {
            return View();
        }
    }
}