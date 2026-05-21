using All_Baby_Essentials.Areas.Admin.ViewModels;
using All_Baby_Essentials.Data;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace All_Baby_Essentials.Areas.Customer.Controllers
{
        [Area("Customer")]
    public class CategoryController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoryController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // GET: CategoryController
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            var categoriesVM = _mapper.Map<List<CategoryVM>>(categories);
            return View(categoriesVM);
        }


        // GET: CategoryController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null)
                return NotFound();

            var categoryVM = _mapper.Map<CategoryVM>(category);

            return View(categoryVM);
        }
    }
}
