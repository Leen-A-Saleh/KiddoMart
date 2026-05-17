namespace All_Baby_Essentials.Areas.Admin.ViewModels
{
    public class ProductColorVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; } = string.Empty;
        public string ColorHexCode { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
