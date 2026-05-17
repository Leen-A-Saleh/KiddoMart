using All_Baby_Essentials.Models;

namespace All_Baby_Essentials.Areas.Customer.ViewModels
{
    public class HomeVM
    {
        public List<ProductDetailsVM> LatestProducts { get; set; } = new();

        public List<ProductDetailsVM> PopularProducts { get; set; } = new();

        public List<Testimonial> Testimonials { get; set; } = new();
    }
}
