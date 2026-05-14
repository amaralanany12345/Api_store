using AutoMapper;
using StoreWebApi.DTO;
using StoreWebApi.Models;
namespace StoreWebApi.Helper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<User,UserDto>()
                .ForMember(dst=>dst.UserName,opt=>opt.MapFrom(a=>a.UserName))
                .ForMember(dst=>dst.Email,opt=>opt.MapFrom(a=>a.Email))
                .ForMember(dst=>dst.CreatedAt,opt=>opt.MapFrom(a=>a.CreatedAt))
                .ForMember(dst=>dst.Balance,opt=>opt.MapFrom(a=>a.Balance))
                .ForMember(dst=>dst.Role, opt=>opt.MapFrom(a=>a.Role));
            CreateMap<Item, ItemDto>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(a => a.Name))
                .ForMember(dst => dst.Price, opt => opt.MapFrom(a => a.Price))
                .ForMember(dst => dst.StockQuantity, opt => opt.MapFrom(a => a.StockQuantity))
                .ForMember(dst => dst.CategoryName, opt => opt.MapFrom(a => a.Category.Name));
            CreateMap<Category, CategoryDto>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(a => a.Name))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(a => a.Description));
            CreateMap<RefreshToken, RefreshTokenDto>()
                .ForMember(dst => dst.RefreshToken, opt => opt.MapFrom(a => a.Token));
            CreateMap<Order, OrderDto>()
                .ForMember(dst => dst.Status, opt => opt.MapFrom(a => a.Status))
                .ForMember(dst => dst.CreatedAt, opt => opt.MapFrom(a => a.CreatedAt))
                .ForMember(dst => dst.UpdatedAt, opt => opt.MapFrom(a => a.UpdatedAt))
                .ForMember(dst => dst.TotalAmount, opt => opt.MapFrom(a => a.TotalAmount));
            CreateMap<Receipt, ReceiptDto>()
                .ForMember(dst => dst.CreateAt, opt => opt.MapFrom(a => a.CreatedAt))
                .ForMember(dst => dst.TotalAmount, opt => opt.MapFrom(a => a.TotalAmount));

        }
    }
}
