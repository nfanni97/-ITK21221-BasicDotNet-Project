using RegistryApp.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using RegistryApp.Vms;
using RegistryApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RegistryApp.Services.Implementations
{

    public class CategoryService : ICategoryService
    {
        private readonly RegistryContext _context;
        private readonly IMapper _mapper;

        public CategoryService(RegistryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CategoryVM>> GetAll()
        {
            return await _context.Categories.ProjectTo<CategoryVM>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}