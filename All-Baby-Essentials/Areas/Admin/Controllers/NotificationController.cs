using All_Baby_Essentials.Services;
using All_Baby_Essentials.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace All_Baby_Essentials.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new
            {
                notifications = _notificationService.GetAll(),
                unreadCount = _notificationService.GetUnreadCount()
            });
        }

        [HttpPost]
        public IActionResult MarkAllRead()
        {
            _notificationService.MarkAllRead();
            return Ok();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _notificationService.Delete(id);
            return Ok();
        }
    }
}
