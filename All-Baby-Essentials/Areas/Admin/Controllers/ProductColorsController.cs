using All_Baby_Essentials.Areas.Admin.ViewModels;
using All_Baby_Essentials.Data;
using All_Baby_Essentials.Models;
using All_Baby_Essentials.Interfaces;
using All_Baby_Essentials.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace All_Baby_Essentials.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductColorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IImageService _imageService;

        public ProductColorsController(ApplicationDbContext context, IMapper mapper, INotificationService notificationService, IImageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
            _imageService = imageService;
        }

        // GET: Admin/ProductColors?productId=5
        public async Task<IActionResult> Index(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductColors)
                .ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);

            if (product == null)
                return NotFound();

            var colors = await _context.Colors.ToListAsync();
            ViewBag.ColorsList = colors.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Name} ({c.HexCode})"
            }).ToList();

            ViewBag.Product = product;
            
            var productColorsVM = _mapper.Map<List<ProductColorVM>>(product.ProductColors);
            return View(productColorsVM);
        }

        // POST: Admin/ProductColors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productId, int? colorId, string? newColorName, string? newColorHexCode, int stockQuantity, IFormFile? imageFile)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
            if (product == null)
                return NotFound();

            int finalColorId = 0;

            // Handle New Color creation
            if (!string.IsNullOrWhiteSpace(newColorName) && !string.IsNullOrWhiteSpace(newColorHexCode))
            {
                // Validate HEX format
                newColorHexCode = newColorHexCode.Trim();
                if (!newColorHexCode.StartsWith("#")) newColorHexCode = "#" + newColorHexCode;
                
                if (newColorHexCode.Length != 7 || !System.Text.RegularExpressions.Regex.IsMatch(newColorHexCode, @"^#[0-9A-Fa-f]{6}$"))
                {
                    TempData["ErrorMessage"] = "Invalid HEX color code format. Must be e.g. #FF0000";
                    return RedirectToAction(nameof(Index), new { productId });
                }

                // Check if color already exists
                var existingColor = await _context.Colors.FirstOrDefaultAsync(c => c.Name.ToLower() == newColorName.Trim().ToLower() || c.HexCode.ToLower() == newColorHexCode.ToLower());
                if (existingColor != null)
                {
                    finalColorId = existingColor.Id;
                }
                else
                {
                    var newColor = new Color
                    {
                        Name = newColorName.Trim(),
                        HexCode = newColorHexCode
                    };
                    _context.Colors.Add(newColor);
                    await _context.SaveChangesAsync();
                    finalColorId = newColor.Id;
                }
            }
            else if (colorId.HasValue && colorId.Value > 0)
            {
                finalColorId = colorId.Value;
            }
            else
            {
                TempData["ErrorMessage"] = "Please select an existing color or enter a new one.";
                return RedirectToAction(nameof(Index), new { productId });
            }

            // Check if this product already has this color variant
            var existingVariant = await _context.ProductColors.FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.ColorId == finalColorId);
            if (existingVariant != null)
            {
                TempData["ErrorMessage"] = "This product already has this color variant. Use Edit to modify stock.";
                return RedirectToAction(nameof(Index), new { productId });
            }

            // Handle Image Upload
            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                imageUrl = await _imageService.UploadImage(imageFile);
            }

            var pcVariant = new ProductColor
            {
                ProductId = productId,
                ColorId = finalColorId,
                StockQuantity = stockQuantity,
                ImageUrl = imageUrl
            };

            _context.ProductColors.Add(pcVariant);
            await _context.SaveChangesAsync();

            await RecalculateProductStock(productId);

            _notificationService.Add("Color Variant Added", "Product color variant was created successfully.", "fas fa-palette", "success");
            TempData["SuccessMessage"] = "Color variant added successfully.";

            return RedirectToAction(nameof(Index), new { productId });
        }

        // GET: Admin/ProductColors/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pc = await _context.ProductColors
                .Include(x => x.Product)
                .Include(x => x.Color)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pc == null)
                return NotFound();

            var vm = _mapper.Map<ProductColorVM>(pc);
            return View(vm);
        }

        // POST: Admin/ProductColors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int stockQuantity, IFormFile? imageFile, string? existingImageUrl)
        {
            var pc = await _context.ProductColors.FirstOrDefaultAsync(x => x.Id == id);
            if (pc == null)
                return NotFound();

            pc.StockQuantity = stockQuantity;

            if (imageFile != null && imageFile.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(pc.ImageUrl))
                {
                    _imageService.DeleteImage(pc.ImageUrl);
                }

                pc.ImageUrl = await _imageService.UploadImage(imageFile);
            }
            else
            {
                pc.ImageUrl = existingImageUrl;
            }

            _context.ProductColors.Update(pc);
            await _context.SaveChangesAsync();

            await RecalculateProductStock(pc.ProductId);

            _notificationService.Add("Color Variant Updated", "Color variant stock and image updated successfully.", "fas fa-edit", "info");
            TempData["SuccessMessage"] = "Color variant updated successfully.";

            return RedirectToAction(nameof(Index), new { productId = pc.ProductId });
        }

        // POST: Admin/ProductColors/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var pc = await _context.ProductColors.FirstOrDefaultAsync(x => x.Id == id);
            if (pc == null)
                return NotFound(new { message = "Variant not found." });

            int productId = pc.ProductId;

            // Delete old image if exists
            if (!string.IsNullOrEmpty(pc.ImageUrl))
            {
                _imageService.DeleteImage(pc.ImageUrl);
            }

            _context.ProductColors.Remove(pc);
            await _context.SaveChangesAsync();

            await RecalculateProductStock(productId);

            _notificationService.Add("Color Variant Removed", "Color variant removed successfully.", "fas fa-trash", "danger");
            return Ok();
        }

        private async Task RecalculateProductStock(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product != null)
            {
                // If it has color variants, calculate total stock as sum of color stocks
                if (product.ProductColors.Any())
                {
                    product.StockQuantity = product.ProductColors.Sum(pc => pc.StockQuantity);
                }
                product.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
