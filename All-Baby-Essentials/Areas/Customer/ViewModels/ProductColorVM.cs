namespace All_Baby_Essentials.Areas.Customer.ViewModels
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
        public ColorVM Color { get; set; } = null!;
    }

    public class ColorVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string HexCode { get; set; } = string.Empty;
    }
}
