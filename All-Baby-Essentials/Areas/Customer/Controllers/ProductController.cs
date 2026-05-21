
using All_Baby_Essentials.Areas.Customer.ViewModels;
using All_Baby_Essentials.Data;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using All_Baby_Essentials.Models;

namespace All_Baby_Essentials.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            if (id <= 0) return NotFound();
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.ProductColors)
                .ThenInclude(pc => pc.Color)
                .Include(p => p.Reviews)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (product == null) return NotFound();
            var productVM = _mapper.Map<ProductDetailsVM>(product);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                productVM.IsInWishlist = await _context.WishlistItems.AnyAsync(w => w.UserId == userId && w.ProductId == id);
            }

            return View(productVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetQuickView(int id)
        {
            if (id <= 0) return NotFound();
            var product = await _context.Products.Include(p => p.Category).Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (product == null) return NotFound();
            var productVM = _mapper.Map<ProductDetailsVM>(product);
            return PartialView("_QuickViewPartial", productVM);
        }


        ///Customer/Product/Index? page = 2 & categoryId = 3 & search = milk & sortBy = price_desc
        public async Task<IActionResult> Index(
            int? categoryId,
            string? search,
            string? sortBy,
            decimal? minPrice,
            decimal? maxPrice,
            int? colorId,
            int page = 1,
            int pageSize = 6)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.ProductColors)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            // Category
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            // Search
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            // Color Filter
            if (colorId.HasValue && colorId.Value > 0)
            {
                query = query.Where(p => p.ProductColors.Any(pc => pc.ColorId == colorId));
            }

            // Price filter 
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            // Sorting
            switch (sortBy)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;

                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;

                default:
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            var totalItems = await query.CountAsync();

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new PaginatedResultVM<ProductDetailsVM>
            {
                Items = _mapper.Map<List<ProductDetailsVM>>(products),
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var wishlistProductIds = await _context.WishlistItems
                    .Where(w => w.UserId == userId)
                    .Select(w => w.ProductId)
                    .ToListAsync();

                foreach (var p in vm.Items)
                    p.IsInWishlist = wishlistProductIds.Contains(p.Id);
            }

            ViewBag.CategoryId = categoryId;
            ViewBag.Search = search;
            ViewBag.SortBy = sortBy;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.ColorId = colorId;

            ViewBag.Categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            ViewBag.Colors = await _context.Colors
                .Where(c => c.ProductColors.Any())
                .ToListAsync();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, string comment, int rating)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Challenge();
            }

            if (string.IsNullOrWhiteSpace(comment) || rating < 1 || rating > 5)
            {
                TempData["ErrorMessage"] = "Invalid review data. Comment is required and rating must be between 1 and 5.";
                return RedirectToAction("ProductDetails", new { id = productId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var review = new ProductReview
            {
                ProductId = productId,
                UserId = userId,
                Comment = comment,
                Rating = rating,
                IsApproved = false, // Must be approved by admin
                CreatedAt = DateTime.Now
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thank you! Your review has been submitted and is pending approval.";
            return RedirectToAction("ProductDetails", new { id = productId });
        }
    }
}
