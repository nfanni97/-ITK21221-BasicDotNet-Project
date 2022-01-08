using AutoMapper;
using RegistryApp.Dtos;
using RegistryApp.Models;
using RegistryApp.Vms;

namespace RegistryApp.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryDto,Category>();
            CreateMap<Category,CategoryVM>();
        }
    }
}