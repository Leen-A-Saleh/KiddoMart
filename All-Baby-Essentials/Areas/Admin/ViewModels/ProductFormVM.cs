using All_Baby_Essentials.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace All_Baby_Essentials.Areas.Admin.ViewModels
{
    public class ProductFormVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Name must be between 3 and 100 characters.")]
        [Remote(action: "CheckName", controller: "Categories", areaName: "Admin", AdditionalFields = nameof(Id),
        ErrorMessage = "This category name already exists.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000,
            ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 99999.99,
            ErrorMessage = "Price must be between 0.01 and 99,999.99.")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100.")]
        public decimal? DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, int.MaxValue,
            ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }

        public List<ProductImageVM> ExistingImages { get; set; } = new();
        public IEnumerable<SelectListItem> Categories { get; set; }  = new List<SelectListItem>();
    }
}
