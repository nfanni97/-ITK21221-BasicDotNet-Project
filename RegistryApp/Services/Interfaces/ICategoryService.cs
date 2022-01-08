using RegistryApp.Dtos;
using RegistryApp.Vms;

namespace RegistryApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryVM>> GetAllCategories();
        Task<CategoryVM?> GetCategoryById(int id);
        Task<CategoryVM> CreateCategory(CategoryDto categoryDto);

        Task<bool> DeleteCategory(int id);
        Task<bool> UpdateCategory(int id, CategoryDto categoryDto);
    }
}