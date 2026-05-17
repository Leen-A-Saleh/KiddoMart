namespace All_Baby_Essentials.Areas.Admin.ViewModels
{
    public class ProductReviewVM
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public bool IsApproved { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
