using System.ComponentModel.DataAnnotations;

namespace All_Baby_Essentials.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        [StringLength(100)]
        public string? SessionId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
