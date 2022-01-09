using System.Linq.Expressions;
using RegistryApp.Dtos;
using RegistryApp.Filters;
using RegistryApp.Models;
using RegistryApp.Vms;

namespace RegistryApp.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductVM> CreateProduct(ProductDto productDto);
        Task<bool> DeleteProduct(int id);
        Task<List<ProductVM>> GetAllProducts(GenericQueryOption<ProductFilter> option);
        Task<ProductVM?> GetProductById(int id);

        Task<bool> UpdateProduct(int id, ProductDto r);
        Task<bool> AddCategoryToProduct(int productId, int categoryId);
        Task<bool> RemoveCategoryFromProduct(int productId, int categoryId);
    }
}