using System.ComponentModel.DataAnnotations;
namespace All_Baby_Essentials.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100,MinimumLength = 3,ErrorMessage ="Name can't be less than 3 greater than 100")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 500 characters.")]
    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt {  get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<Product> Products { get; set; } = new List<Product>(); 


}
