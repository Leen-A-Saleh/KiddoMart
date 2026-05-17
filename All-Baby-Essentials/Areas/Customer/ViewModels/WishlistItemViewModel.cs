namespace All_Baby_Essentials.Areas.Customer.ViewModels
{
    public class WishlistItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
