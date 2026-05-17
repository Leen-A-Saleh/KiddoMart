using System.ComponentModel.DataAnnotations;

namespace All_Baby_Essentials.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId {  get; set; }
        public Product Product { get; set; } = null!;

        [Required(ErrorMessage = "Image URL is required.")]
        public string ImageUrl {  get; set; } = string.Empty;
        public bool IsMain {  get; set; } = false;
        public int DisplayOrder {  get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;

    }
}
