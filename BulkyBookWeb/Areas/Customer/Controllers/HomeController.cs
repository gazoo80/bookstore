
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork uow;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork _uow)
        {
            _logger = logger;
            uow = _uow;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = uow.Product.GetAll(includeProperties: "Category,CoverType");

            return View(productList);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart sCart = new()
            {
                Product = uow.Product.GetFirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType"),
                ProductId = id,
                Count = 1
            };

            return View(sCart);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize] 
        public IActionResult Details(ShoppingCart sCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            sCart.ApplicationUserId = claim!.Value;

            ShoppingCart cartFromDb = uow.ShoppingCart.GetFirstOrDefault(
                sc => sc.ApplicationUserId == claim.Value && sc.ProductId == sCart.ProductId
            );

            if (cartFromDb == null)
            {
                sCart.Id = 0;
                uow.ShoppingCart.Add(sCart);
            }
            else 
            {    
                cartFromDb.Count = sCart.Count;
                uow.ShoppingCart.Update(cartFromDb);
            }

            uow.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}