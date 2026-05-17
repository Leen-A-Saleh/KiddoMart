using All_Baby_Essentials.Areas.Customer.ViewModels;
using All_Baby_Essentials.Data;
using All_Baby_Essentials.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace All_Baby_Essentials.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public HomeController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            // Latest Products
            var latestProducts = await _context.Products
                .Include(p => p.Images)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();

            // Popular Products
            var popularProducts = await _context.Products
                .Include(p => p.Images)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.StockQuantity)
                .Take(8)
                .ToListAsync();

            // Testimonials
            var testimonials = await _context.Testimonials
                .Include(t => t.User)
                .Where(t => !t.IsDeleted && t.IsApproved)
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .ToListAsync();

            var vm = new HomeVM
            {
                LatestProducts = _mapper.Map<List<ProductDetailsVM>>(latestProducts),
                PopularProducts = _mapper.Map<List<ProductDetailsVM>>(popularProducts),
                Testimonials = testimonials
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTestimonial(string content, int rating)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Challenge();
            }

            if (string.IsNullOrWhiteSpace(content) || rating < 1 || rating > 5)
            {
                TempData["Error"] = "Invalid testimonial data. Content is required and rating must be between 1 and 5.";
                return RedirectToAction("Index");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var testimonial = new Testimonial
            {
                UserId = userId,
                Content = content,
                Rating = rating,
                IsApproved = false, // Must be approved by admin
                CreatedAt = DateTime.Now
            };

            _context.Testimonials.Add(testimonial);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thank you for your testimonial! It will be visible on the site once approved by an administrator.";
            return RedirectToAction("Index");
        }
    }
}
