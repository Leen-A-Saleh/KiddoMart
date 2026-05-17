using All_Baby_Essentials.Areas.Customer.Helpers;
using All_Baby_Essentials.Areas.Customer.ViewModels;
using All_Baby_Essentials.Data;
using All_Baby_Essentials.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Stripe.Checkout;
using System.Security.Claims;

namespace All_Baby_Essentials.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =========================
        // CHECKOUT PAGE
        // =========================
        public IActionResult Index()
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var vm = new CheckoutViewModel
            {
                CartItems = cartItems
            };

            return View(vm);
        }

        // =========================
        // STRIPE PAYMENT
        // =========================
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid)
            {
                model.CartItems = cartItems;
                return View("Index", model);
            }

            // Save checkout details for Success page
            HttpContext.Session.SetObjectAsJson("CheckoutData", model);

            if (model.PaymentMethod == "COD")
            {
                return RedirectToAction("Success");
            }

            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + "/Customer/Checkout/Success",
                CancelUrl = domain + "/Customer/Checkout/Cancel",

                PaymentMethodTypes = new List<string>
                {
                    "card"
                },

                Mode = "payment",

                CustomerEmail = model.Email,

                LineItems = cartItems.Select(item => new SessionLineItemOptions
                {
                    Quantity = item.Quantity,

                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",

                        UnitAmount = (long)(item.Price * 100),

                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName
                        }
                    }

                }).ToList()
            };

            var service = new SessionService();

            Session stripeSession = await service.CreateAsync(options);

            return Redirect(stripeSession.Url);
        }

        // =========================
        // SUCCESS
        // =========================
        public async Task<IActionResult> Success()
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
            var checkoutData = HttpContext.Session.GetObjectFromJson<CheckoutViewModel>("CheckoutData");

            if (cartItems.Any())
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId)) return RedirectToAction("Index", "Home");

                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var isCod = checkoutData?.PaymentMethod == "COD";
                    
                    var order = new Order
                    {
                        UserId = userId,
                        FullName = !string.IsNullOrEmpty(checkoutData?.FullName) ? checkoutData.FullName : (!string.IsNullOrEmpty(user.FullName) ? user.FullName : user.UserName ?? "Customer"),
                        PhoneNumber = !string.IsNullOrEmpty(checkoutData?.PhoneNumber) ? checkoutData.PhoneNumber : (user.PhoneNumber ?? ""),
                        ShippingAddress = checkoutData?.ShippingAddress ?? "Address not provided",
                        City = checkoutData?.City ?? "Unknown",
                        Country = checkoutData?.Country ?? "Unknown",
                        TotalAmount = cartItems.Sum(i => i.Price * i.Quantity),
                        Status = OrderStatus.Processing,
                        PaymentStatus = isCod ? OrderPaymentStatus.Pending : OrderPaymentStatus.Approved,
                        PaymentMethod = isCod ? OrderPaymentMethod.CashOnDelivery : OrderPaymentMethod.Visa,
                        OrderDate = DateTime.Now
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    foreach (var item in cartItems)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.Price
                        };
                        _context.OrderItems.Add(orderItem);

                        // Decrease stock
                        var product = await _context.Products.FindAsync(item.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity -= item.Quantity;
                            if (product.StockQuantity < 0) product.StockQuantity = 0;
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("CheckoutData");
            }

            return View();
        }

        // =========================
        // CANCEL
        // =========================
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
