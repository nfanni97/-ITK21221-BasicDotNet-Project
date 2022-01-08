using RegistryApp.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using RegistryApp.Vms;
using RegistryApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RegistryApp.Dtos;

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

        public async Task<List<CategoryVM>> GetAllCategories()
        {
            return await _context.Categories.ProjectTo<CategoryVM>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<CategoryVM?> GetCategoryById(int id)
        {
            return await _context.Categories
                .Where(x => x.Id == id)
                .ProjectTo<CategoryVM>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<CategoryVM> CreateCategory(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryVM>(category);
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCategory(int id, CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            category.Id = id;
            _context.Entry(category).State = EntityState.Modified;
            var n = await _context.SaveChangesAsync();
            return n == 1;
        }
    }
}