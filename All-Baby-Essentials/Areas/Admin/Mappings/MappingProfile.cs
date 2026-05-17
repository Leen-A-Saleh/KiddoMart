using All_Baby_Essentials.Areas.Admin.ViewModels;
using All_Baby_Essentials.Models;
using AutoMapper;

namespace All_Baby_Essentials.Areas.Admin.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryVM>();

            CreateMap<CategoryVM, Category>()
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(_ => DateTime.Now));

            CreateMap<Product, CatProductVM>();

            CreateMap<CatProductVM, Product>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.CartItems, opt => opt.Ignore())
                .ForMember(dest => dest.WishlistItems, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(_ => DateTime.Now));

            CreateMap<Product, ProductFormVM>();

            CreateMap<ProductFormVM, Product>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.CartItems, opt => opt.Ignore())
                .ForMember(dest => dest.WishlistItems, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(_ => DateTime.Now));

            CreateMap<Product, ProductDetailsVM>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<ProductColor, ProductColorVM>()
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color.Name))
                .ForMember(dest => dest.ColorHexCode, opt => opt.MapFrom(src => src.Color.HexCode));

            CreateMap<ProductImage, ProductImageVM>();

            CreateMap<ProductReview, ProductReviewVM>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<ApplicationUser, ApplicationUserVM>()
                .ForMember(dest => dest.Role, opt => opt.Ignore()); // Roles are handled separately
        }
    }
}
