using All_Baby_Essentials.Areas.Customer.ViewModels;
using All_Baby_Essentials.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace All_Baby_Essentials.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var vm = new ProfileVM
            {
                FullName = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth
            };

            return View(vm);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Index(ProfileVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            user.FullName = vm.FullName;
            user.PhoneNumber = vm.PhoneNumber;
            user.DateOfBirth = vm.DateOfBirth;

            await _userManager.UpdateAsync(user);

            ViewBag.Success = "Profile updated successfully";

            return View(vm);
        }
    }
}
