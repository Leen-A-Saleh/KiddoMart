using All_Baby_Essentials.Areas.Admin.ViewModels;
using All_Baby_Essentials.Data;
using All_Baby_Essentials.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using All_Baby_Essentials.Areas.Admin.ViewModels;
using All_Baby_Essentials.Data;
using All_Baby_Essentials.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace All_Baby_Essentials.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            var mostOrdered = await _context.OrderItems
                .Include(oi => oi.Product)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductName = g.First().Product.Name,
                    Count = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            var ordersPerDay = await _context.Orders
                .Where(o => o.OrderDate >= thirtyDaysAgo)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalUsers = _userManager.Users.Count(),
                TotalProducts = _context.Products.Count(p => !p.IsDeleted),
                TotalCategories = _context.Categories.Count(c => !c.IsDeleted),
                TotalOrders = _context.Orders.Count(),
                MostOrderedProduct = mostOrdered?.ProductName ?? "No orders yet",
                MostOrderedProductCount = mostOrdered?.Count ?? 0,
                RecentOrders = _context.Orders.Include(o => o.User).OrderByDescending(o => o.OrderDate).Take(5).ToList(),
                OrdersPerDay = ordersPerDay.ToDictionary(k => k.Date.ToString("MMM dd"), v => v.Count)
            };

            return View(viewModel);
        }
    }
}
