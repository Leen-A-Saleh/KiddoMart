using System.ComponentModel.DataAnnotations;

namespace All_Baby_Essentials.Areas.Customer.ViewModels
{
    public class CheckoutViewModel
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PaymentMethod { get; set; } = "Card";

        // Cart
        public List<CartItemViewModel> CartItems { get; set; }
            = new List<CartItemViewModel>();

        // Total
        public decimal TotalPrice =>
            CartItems.Sum(x => x.Price * x.Quantity);
    }
}
