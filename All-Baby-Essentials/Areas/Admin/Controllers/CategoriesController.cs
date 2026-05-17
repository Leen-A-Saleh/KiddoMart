using All_Baby_Essentials.Services;
using All_Baby_Essentials.Areas.Admin.ViewModels;
using All_Baby_Essentials.Data;
using All_Baby_Essentials.Models;
using All_Baby_Essentials.Services;
using All_Baby_Essentials.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace All_Baby_Essentials.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IImageService _imageService;

        public CategoriesController(ApplicationDbContext context, IMapper mapper, INotificationService notificationService, IImageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
            _imageService = imageService;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            var categoriesVM = _mapper.Map<List<CategoryVM>>(categories);
            return View(categoriesVM);
        }

        // GET: Admin/Categories/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null)
                return NotFound();

            category.Products = category.Products.Where(p => !p.IsDeleted).ToList();

            var categoryVM = _mapper.Map<CategoryVM>(category);

            return View(categoryVM);
        }

        // GET: Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View("Upsert");
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryVM categoryVM, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return View("Upsert", categoryVM);

            if (image != null && image.Length > 0)
            {
                string fileName = await _imageService.UploadImage(image);
                categoryVM.ImageUrl = fileName;
            }

            var category = _mapper.Map<Category>(categoryVM);
            category.ImageUrl = categoryVM.ImageUrl;

            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();
                _notificationService.Add("Category Added", $"'{category.Name}' was added.", "fas fa-tag", "success"); 
                TempData["SuccessMessage"] = "Product created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Upsert", categoryVM);
            }
        }



        // GET: Categories/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var categoryVM = _mapper.Map<CategoryVM>(category);
            return View("Upsert", categoryVM);
        }

        // POST: Categories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryVM categoryVM, IFormFile? imageFile)
        {
            if (id != categoryVM.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View("Upsert", categoryVM);

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(category.ImageUrl))
                    _imageService.DeleteImage(category.ImageUrl);

                categoryVM.ImageUrl = await _imageService.UploadImage(imageFile);
            }
            else
            {
                categoryVM.ImageUrl = category.ImageUrl;
            }

            _mapper.Map(categoryVM, category);

            try
            {
                await _context.SaveChangesAsync();
                _notificationService.Add("Category Updated", $"'{category.Name}' was updated successfully.","fas fa-edit","info");
                TempData["SuccessMessage"] = "Product updated  successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(categoryVM.Id))
                    return NotFound();
                throw;
            }
        }

        // POST: Admin/Categories/Delete (Soft Delete)
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.IsDeleted = true;
            category.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();
            _notificationService.Add("Category Deleted", $"'{category.Name}' was deleted.", "fas fa-trash", "danger"); 
            return Ok();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }


        // Remote Validation
        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckName(CategoryVM categoryVM)
        {
            var normalizedName = categoryVM.Name.Trim().ToLower();

            var isExists = _context.Categories.Any(c => c.Name.ToLower() == normalizedName && c.Id != categoryVM.Id && !c.IsDeleted);

            return Json(isExists ? "This category name already exists." : true);
        }
    }
}
