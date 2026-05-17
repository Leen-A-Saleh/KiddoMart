using System.ComponentModel.DataAnnotations;

namespace All_Baby_Essentials.Models
{
    public class Testimonial
    {
        public int Id {  get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        [Required(ErrorMessage = "Testimonial content is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 1000 characters.")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        public bool IsApproved { get; set; } = false;

        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
