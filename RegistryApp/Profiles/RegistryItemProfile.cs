using AutoMapper;
using RegistryApp.Models;
using RegistryApp.Vms;

namespace RegistryApp.Profiles
{
    public class RegistryItemProfile : Profile
    {
        public RegistryItemProfile()
        {
            CreateMap<RegistryItem,RegistryItemVM>()
                .ForMember(
                    dest=>dest.ProductName,
                    opt=>opt.MapFrom(src=>src.Product.Name)
                );
        }
    }
}