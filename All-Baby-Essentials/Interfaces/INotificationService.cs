using All_Baby_Essentials.Areas.Admin.ViewModels;
using System.Collections.Generic;

namespace All_Baby_Essentials.Interfaces
{
    public interface INotificationService
    {
        void Add(string title, string message, string icon = "fas fa-bell", string color = "info");
        List<NotificationVM> GetAll();
        int GetUnreadCount();
        void MarkAllRead();
        void Delete(int id);
    }
}
