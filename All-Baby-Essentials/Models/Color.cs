using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace All_Baby_Essentials.Models
{
    public class Color
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Color name is required.")]
        [StringLength(50, ErrorMessage = "Color name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Color code is required.")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Color code must be a 7-character HEX value (e.g. #FF0000).")]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid HEX color code format.")]
        public string HexCode { get; set; } = string.Empty;

        public ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();
    }
}
