using All_Baby_Essentials.Areas.Admin.ViewModels;
using All_Baby_Essentials.Data;
using All_Baby_Essentials.Models;
using All_Baby_Essentials.Services;
using All_Baby_Essentials.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace All_Baby_Essentials.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IImageService _imageService;

        public ProductsController(ApplicationDbContext context, IMapper mapper, INotificationService notificationService, IImageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
            _imageService = imageService;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index(string? search, int? colorId, string? stockFilter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.ProductColors)
                .ThenInclude(pc => pc.Color)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            // 1. Search by name
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            // 2. Filter by Color
            if (colorId.HasValue && colorId.Value > 0)
            {
                query = query.Where(p => p.ProductColors.Any(pc => pc.ColorId == colorId));
            }

            // 3. Filter by Stock Quantity
            if (!string.IsNullOrEmpty(stockFilter))
            {
                switch (stockFilter)
                {
                    case "out":
                        query = query.Where(p => p.StockQuantity == 0);
                        break;
                    case "low":
                        query = query.Where(p => p.StockQuantity > 0 && p.StockQuantity < 10);
                        break;
                    case "instock":
                        query = query.Where(p => p.StockQuantity >= 10);
                        break;
                }
            }

            var products = await query.ToListAsync();

            var productsVM = _mapper.Map<List<ProductDetailsVM>>(products);

            // Populate ViewBags
            ViewBag.Search = search;
            ViewBag.ColorId = colorId;
            ViewBag.StockFilter = stockFilter;

            var colors = await _context.Colors.ToListAsync();
            ViewBag.ColorsList = colors.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == colorId
            }).ToList();

            return View(productsVM);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.ProductColors)
                .ThenInclude(pc => pc.Color)
                .Include(p => p.Reviews)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null)
                return NotFound();

            var productVM = _mapper.Map<ProductDetailsVM>(product);

            return View(productVM);
        }

        // GET: Admin/Products/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var productFormVM = new ProductFormVM();
            await LoadCategories(productFormVM);
            return View("Upsert", productFormVM);
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormVM productVM)
        {
            await LoadCategories(productVM);

            if (!ModelState.IsValid)
                return View("Upsert", productVM);

            var product = _mapper.Map<Product>(productVM);

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                if (productVM.ImageFiles != null && productVM.ImageFiles.Any())
                {
                    bool isFirstImage = true;

                    for (int i = 0; i < productVM.ImageFiles.Count; i++)
                    {
                        var file = productVM.ImageFiles[i];

                        if (file != null && file.Length > 0)
                        {
                            var fileName = await _imageService.UploadImage(file);

                            var productImage = new ProductImage
                            {
                                ProductId = product.Id,
                                ImageUrl = fileName,
                                IsMain = isFirstImage,
                                DisplayOrder = i + 1
                            };

                            _context.ProductImages.Add(productImage);
                            isFirstImage = false;
                        }
                    }

                    await _context.SaveChangesAsync();
                    _notificationService.Add("Product Added", $"'{product.Name}' was added.", "fas fa-tag", "success");
                    TempData["SuccessMessage"] = "Product created successfully.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Upsert", productVM);
            }
        }

        // GET: Admin/Products/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null)
                return NotFound();

            var productVM = _mapper.Map<ProductFormVM>(product);

            await LoadCategories(productVM);

            return View("Upsert", productVM);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductFormVM productVM)
        {
            if (id != productVM.Id)
                return NotFound();

            await LoadCategories(productVM);

            if (!ModelState.IsValid)
                return View("Upsert", productVM);

            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null)
                return NotFound();

            _mapper.Map(productVM, product);
            product.UpdatedAt = DateTime.Now;

            try
            {
                if (productVM.ImageFiles != null && productVM.ImageFiles.Any())
                {
                    int displayOrder = product.Images.Any()
                        ? product.Images.Max(i => i.DisplayOrder) + 1
                        : 1;

                    bool hasMainImage = product.Images.Any(i => i.IsMain);

                    foreach (var file in productVM.ImageFiles)
                    {
                        if (file != null && file.Length > 0)
                        {
                            var fileName = await _imageService.UploadImage(file);

                            var productImage = new ProductImage
                            {
                                ProductId = product.Id,
                                ImageUrl = fileName,
                                IsMain = !hasMainImage, 
                                DisplayOrder = displayOrder++
                            };

                            _context.ProductImages.Add(productImage);
                            hasMainImage = true;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                _notificationService.Add("Product Updated", $"'{product.Name}' was updated successfully.", "fas fa-edit", "info");
                TempData["SuccessMessage"] = "Product updated  successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                    return NotFound();

                throw;
            }
        }

        // POST: Admin/Products/Delete (Soft Delete)
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            product.IsDeleted = true;
            product.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            _notificationService.Add("Product Deleted", $"'{product.Name}' was deleted.", "fas fa-trash", "danger");
            return Ok();
        }

        private async Task LoadCategories(ProductFormVM productFormVM)
        {
            productFormVM.Categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.Id == id);
        }

        // Remote Validation
        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckName(ProductFormVM productFormVM)
        {
            var normalizedName = productFormVM.Name.Trim().ToLower();

            var isExists = _context.Categories.Any(c => c.Name.ToLower() == normalizedName && c.Id != productFormVM.Id && !c.IsDeleted);

            return Json(isExists ? "This product name already exists." : true);
        }
    }
}
