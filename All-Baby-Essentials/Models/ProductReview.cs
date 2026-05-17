using System.ComponentModel.DataAnnotations;

namespace All_Baby_Essentials.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int ProductId {  get; set; }
        public Product Product { get; set; } = null!;

        [Required(ErrorMessage = "Comment is required.")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Comment must be between 5 and 1000 characters.")]
        public string Comment { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
