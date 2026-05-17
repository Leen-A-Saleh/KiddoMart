using All_Baby_Essentials.Areas.Admin.ViewModels;
using All_Baby_Essentials.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace All_Baby_Essentials.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ApplicationUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            var userVMs = _mapper.Map<List<ApplicationUserVM>>(users);

            foreach (var vm in userVMs)
            {
                var user = users.First(u => u.Id == vm.Id);
                var roles = await _userManager.GetRolesAsync(user);
                vm.Role = roles.FirstOrDefault() ?? "No Role";
            }

            return View(userVMs);
        }
    }
}
