namespace All_Baby_Essentials.Areas.Customer.ViewModels
{
    public class ProductImageVM
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}
