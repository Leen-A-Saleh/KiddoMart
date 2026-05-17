using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using All_Baby_Essentials.Data;
using All_Baby_Essentials.Areas.Customer.Helpers;
using All_Baby_Essentials.Areas.Customer.ViewModels;

namespace All_Baby_Essentials.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📌 عرض الـ Wishlist
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        // 📌 إضافة منتج إلى Wishlist
        public async Task<IActionResult> Add(int productId)
        {
            await AddItemToWishlistInternal(productId);
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int productId)
        {
            UpdateWishlist(wishlist => {
                var item = wishlist.FirstOrDefault(x => x.ProductId == productId);
                if (item != null) wishlist.Remove(item);
            });
            return RedirectToAction("Index");
        }

        // --- AJAX ENDPOINTS FOR OFFCANVAS ---

        [HttpGet]
        public IActionResult GetWishlistOffcanvas()
        {
            var wishlist = GetWishlist();
            return PartialView("_WishlistOffcanvasPartial", wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> AddAjax(int productId)
        {
            var success = await AddItemToWishlistInternal(productId);
            if (!success) return NotFound(new { success = false, message = "Product not found" });

            return Json(new { success = true, count = GetWishlistCount() });
        }

        [HttpPost]
        public IActionResult RemoveAjax(int productId)
        {
            UpdateWishlist(wishlist => {
                var item = wishlist.FirstOrDefault(x => x.ProductId == productId);
                if (item != null) wishlist.Remove(item);
            });
            return Json(new { success = true, count = GetWishlistCount() });
        }

        // ==========================================
        // PRIVATE HELPER METHODS (DRY)
        // ==========================================

        private List<WishlistItemViewModel> GetWishlist()
        {
            return HttpContext.Session.GetObjectFromJson<List<WishlistItemViewModel>>("Wishlist") 
                   ?? new List<WishlistItemViewModel>();
        }

        private void SaveWishlist(List<WishlistItemViewModel> wishlist)
        {
            HttpContext.Session.SetObjectAsJson("Wishlist", wishlist);
        }

        private void UpdateWishlist(Action<List<WishlistItemViewModel>> action)
        {
            var wishlist = GetWishlist();
            action(wishlist);
            SaveWishlist(wishlist);
        }

        private int GetWishlistCount() => GetWishlist().Count;

        private async Task<bool> AddItemToWishlistInternal(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);

            if (product == null) return false;

            var wishlist = GetWishlist();
            var exists = wishlist.FirstOrDefault(x => x.ProductId == productId);

            if (exists == null)
            {
                decimal price = product.DiscountPercentage > 0 
                    ? product.Price * (1 - (product.DiscountPercentage.Value / 100)) 
                    : product.Price;

                wishlist.Add(new WishlistItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = price,
                    ImageUrl = product.Images.FirstOrDefault()?.ImageUrl ?? ""
                });
                SaveWishlist(wishlist);
            }

            return true;
        }
    }
}
