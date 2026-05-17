using Microsoft.AspNetCore.Mvc;
using All_Baby_Essentials.Areas.Customer.Helpers;
using All_Baby_Essentials.Areas.Customer.ViewModels;
using All_Baby_Essentials.Data;
using Microsoft.EntityFrameworkCore;

namespace All_Baby_Essentials.Areas.Customer.Controllers
{
        [Area("Customer")]
    public class CartController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CartController
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }


        public async Task<IActionResult> AddToCart(int productId)
        {
            await AddItemToCartInternal(productId);
            return RedirectToAction("Index", "Cart", new { area = "Customer" });
        }

        public IActionResult Remove(int productId)
        {
            UpdateCart(cart => {
                var item = cart.FirstOrDefault(x => x.ProductId == productId);
                if (item != null) cart.Remove(item);
            });
            return RedirectToAction("Index", "Cart", new { area = "Customer" });
        }

        public IActionResult IncreaseQuantity(int productId)
        {
            UpdateCart(cart => {
                var item = cart.FirstOrDefault(x => x.ProductId == productId);
                if (item != null) item.Quantity++;
            });
            return RedirectToAction("Index");
        }

        public IActionResult DecreaseQuantity(int productId)
        {
            UpdateCart(cart => {
                var item = cart.FirstOrDefault(x => x.ProductId == productId);
                if (item != null) {
                    item.Quantity--;
                    if (item.Quantity <= 0) cart.Remove(item);
                }
            });
            return RedirectToAction("Index");
        }

        // --- AJAX ENDPOINTS FOR OFFCANVAS ---

        [HttpGet]
        public IActionResult GetCartOffcanvas()
        {
            var cart = GetCart();
            var viewModel = new CartViewModel { 
                Items = cart, 
                TotalPrice = cart.Sum(x => x.Price * x.Quantity) 
            };
            return PartialView("_CartOffcanvasPartial", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCartAjax(int productId)
        {
            var success = await AddItemToCartInternal(productId);
            if (!success) return NotFound(new { success = false, message = "Product not found" });

            return Json(new { success = true, count = GetCartCount() });
        }

        [HttpPost]
        public IActionResult RemoveAjax(int productId)
        {
            UpdateCart(cart => {
                var item = cart.FirstOrDefault(x => x.ProductId == productId);
                if (item != null) cart.Remove(item);
            });
            return Json(new { success = true, count = GetCartCount() });
        }

        [HttpPost]
        public IActionResult UpdateQuantityAjax(int productId, int quantity)
        {
            UpdateCart(cart => {
                var item = cart.FirstOrDefault(x => x.ProductId == productId);
                if (item != null) {
                    if (quantity <= 0) cart.Remove(item);
                    else item.Quantity = quantity;
                }
            });
            return Json(new { success = true, count = GetCartCount() });
        }

        // ==========================================
        // PRIVATE HELPER METHODS (DRY)
        // ==========================================

        private List<CartItemViewModel> GetCart()
        {
            return HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") 
                   ?? new List<CartItemViewModel>();
        }

        private void SaveCart(List<CartItemViewModel> cart)
        {
            HttpContext.Session.SetObjectAsJson("Cart", cart);
        }

        private void UpdateCart(Action<List<CartItemViewModel>> action)
        {
            var cart = GetCart();
            action(cart);
            SaveCart(cart);
        }

        private int GetCartCount() => GetCart().Sum(c => c.Quantity);

        private async Task<bool> AddItemToCartInternal(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return false;

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                decimal price = product.DiscountPercentage > 0 
                    ? product.Price * (1 - (product.DiscountPercentage.Value / 100)) 
                    : product.Price;

                cart.Add(new CartItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = price,
                    Quantity = 1,
                    ImageUrl = product.Images.FirstOrDefault()?.ImageUrl ?? ""
                });
            }

            SaveCart(cart);
            return true;
        }

    }
}
