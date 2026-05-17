using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace All_Baby_Essentials.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        [StringLength(100)]
        public string? SessionId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;
        [NotMapped]
        public decimal SubTotal => Quantity * (Product?.Price ?? 0);
    }
}
