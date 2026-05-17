namespace All_Baby_Essentials.Areas.Customer.ViewModels
{
    public class ProductFilterVM
    {
        public int? CategoryId { get; set; }

        public string? Search { get; set; }

        public string? SortBy { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }
    }
}
