using All_Baby_Essentials.Models;

namespace All_Baby_Essentials.Areas.Customer.ViewModels
{
    public class ProductDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public int StockQuantity { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<ProductImageVM> Images { get; set; } = new();

        public List<ProductReviewVM> Reviews { get; set; } = new();
        public List<ProductColorVM> ProductColors { get; set; } = new();
    }
}
