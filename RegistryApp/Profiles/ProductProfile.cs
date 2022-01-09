using AutoMapper;
using RegistryApp.Dtos;
using RegistryApp.Models;
using RegistryApp.Vms;

namespace RegistryApp.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductDto,Product>()
                .ForMember(
                    dest => dest.CategoryProducts,
                    opt => new List<CategoryProduct>()
            );
            CreateMap<Product,ProductVM>()
                .ForMember(
                    dest => dest.CategoryNames,
                    opt => new List<string>()
            );
        }
    }
}