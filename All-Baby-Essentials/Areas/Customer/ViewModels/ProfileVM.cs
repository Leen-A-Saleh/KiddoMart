using System.ComponentModel.DataAnnotations;

namespace All_Baby_Essentials.Areas.Customer.ViewModels
{
    public class ProfileVM
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }
    }
}
