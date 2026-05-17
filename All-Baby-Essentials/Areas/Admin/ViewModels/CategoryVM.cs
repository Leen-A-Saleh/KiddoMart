using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using All_Baby_Essentials.Models;



namespace All_Baby_Essentials.Areas.Admin.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100,MinimumLength = 3,ErrorMessage ="Name can't be less than 3 greater than 100")]
        [Remote( action: "CheckName",controller: "Categories",areaName: "Admin", AdditionalFields = nameof(Id),
        ErrorMessage = "This category name already exists.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 500 characters.")]
        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt {  get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<CatProductVM> Products { get; set; } = new List<CatProductVM>();


    }
}
