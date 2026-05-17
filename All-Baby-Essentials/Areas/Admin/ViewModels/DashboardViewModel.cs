using All_Baby_Essentials.Models;
using System.Collections.Generic;

namespace All_Baby_Essentials.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public string MostOrderedProduct { get; set; } = "N/A";
        public int MostOrderedProductCount { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public Dictionary<string, int> OrdersPerDay { get; set; } = new();
    }
}
