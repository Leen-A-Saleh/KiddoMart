using All_Baby_Essentials.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using All_Baby_Essentials.Interfaces;

namespace All_Baby_Essentials.Services
{
    public class NotificationService : INotificationService
    {
        private static readonly List<NotificationVM> notifications = new();
        private static int Counter = 1;

        public void Add(string title, string message, string icon = "fas fa-bell", string color = "info")
        {
            notifications.Insert(0, new NotificationVM
            {
                Id = Counter++,
                Title = title,
                Message = message,
                Icon = icon,
                Color = color,
                IsRead = false,
                CreatedAt = DateTime.Now
            });
        }

        public List<NotificationVM> GetAll() => notifications.Take(50).ToList();

        public int GetUnreadCount() => notifications.Count(n => !n.IsRead);

        public void MarkAllRead() => notifications.ForEach(n => n.IsRead = true);

        public void Delete(int id) => notifications.RemoveAll(n => n.Id == id);
    }
}


