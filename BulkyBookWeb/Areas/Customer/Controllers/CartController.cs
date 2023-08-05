using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IEmailSender emailSender;

        public ShoppingCartVM shoppingCartVM { get; set; }

        public CartController(IUnitOfWork _uow, IHttpContextAccessor _httpContextAccessor, IEmailSender _emailSender)
        {
            uow = _uow;
            httpContextAccessor = _httpContextAccessor;
            emailSender = _emailSender;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var applicationUserId = claim!.Value;

            var ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = uow.ShoppingCart.GetAllForUserId(applicationUserId, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                
                ShoppingCartVM.CartTotal += (cart.Price * cart.Count);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId) 
        { 
            var cart = uow.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            if (cart != null) {
                uow.ShoppingCart.IncrementCount(cart, 1);
                uow.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = uow.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            if (cart != null)
            {
                uow.ShoppingCart.DecrementCount(cart, 1);
                uow.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = uow.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            if (cart != null)
            {
                uow.ShoppingCart.Remove(cart);
                uow.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var applicationUserId = claim!.Value;

            var ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = uow.ShoppingCart.GetAllForUserId(applicationUserId, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = uow.ApplicationUser.GetFirstOrDefault(
                    u => u.Id == applicationUserId);
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);

                ShoppingCartVM.CartTotal += (cart.Price * cart.Count);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public IActionResult PlaceOrder(ShoppingCartVM sCartVM)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var applicationUserId = claim!.Value;

			sCartVM.ListCart = uow.ShoppingCart.GetAllForUserId(applicationUserId, includeProperties: "Product");

            sCartVM.OrderHeader.OrderDate = DateTime.Now;
			sCartVM.OrderHeader.ApplicationUserId = applicationUserId;
    
            foreach (var cart in sCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
				sCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            SetPaymentAndOrderStatus(sCartVM, applicationUserId);

            SaveOrderHeaderAndDetail(sCartVM);

            ApplicationUser applicationUser = uow.ApplicationUser.GetFirstOrDefault(u => u.Id == applicationUserId);

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                return StripePageConfiguration(sCartVM);
            }
            else
            {
                return RedirectToAction("OrderConfirmation", "Cart", new { id = sCartVM.OrderHeader.Id });
            }
		} 

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = uow.OrderHeader.GetFirstOrDefault(oh => oh.Id == id, includeProperties: "ApplicationUser");

            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    uow.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    uow.Save();
                }
            }
            /*
            try
            {
                emailSender.SendEmailAsync(
                   orderHeader.ApplicationUser.Email,
                   "New Order - Book Store",
                   "<h2>Your order has been processed</h2>"
                ).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw e;
            }*/

            var shoppingCart = uow.ShoppingCart.GetAllForUserId(orderHeader.ApplicationUserId).ToList();

            uow.ShoppingCart.RemoveRange(shoppingCart);
            uow.Save();

            return View(id);
        }

        private double GetPriceBasedOnQuantity(int quantity, double price, double price50, double price100)
        {
            if (quantity <= 50) return price;
            if (quantity <= 100) return price50;
            return price100;
        }

        private void SetPaymentAndOrderStatus(ShoppingCartVM sCartVM, string applicationUserId)
        {
            ApplicationUser applicationUser = uow.ApplicationUser.GetFirstOrDefault(u => u.Id == applicationUserId);

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                sCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                sCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else 
            {
                sCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                sCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
        }

        private void SaveOrderHeaderAndDetail(ShoppingCartVM sCartVM)
        {
            uow.OrderHeader.Add(sCartVM.OrderHeader);
            uow.Save();

            List<OrderDetail> details = new();
            foreach (var cart in sCartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = sCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };

                details.Add(orderDetail);
            }

            uow.OrderDetail.AddRange(details);
            uow.Save();
        }

        private IActionResult StripePageConfiguration(ShoppingCartVM sCartVM)
        {
            // ******** CONFIGURACION DE STRIPE ********
            // Asegurarse de que la clase SessionCreateOptions este siendo usada del namespace
            // Stripe.Checkout y no de Stripe.BillingPortal
            var domain = $"{httpContextAccessor?.HttpContext?.Request.Scheme}://{httpContextAccessor?.HttpContext?.Request.Host}";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{domain}/Customer/Cart/OrderConfirmation?id={sCartVM.OrderHeader.Id}",
                CancelUrl = $"{domain}/Customer/Cart/Index",
            };

            foreach (var item in sCartVM.ListCart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        // Esta cantidad tiene que ser en centavos. A nuestro precio lo multiplicamos por 100
                        UnitAmount = (long)(item.Price * 100), // Ejm: 20.00 ==> 2000
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                        },
                    },
                    Quantity = item.Count
                };

                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            uow.OrderHeader.UpdateStripePaymentId(sCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            uow.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}
