namespace All_Baby_Essentials.Areas.Customer.ViewModels
{
    public class PaginatedResultVM<T>
    {
        public List<T> Items { get; set; } = new();

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}
