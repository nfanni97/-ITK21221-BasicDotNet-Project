using RegistryApp.Vms;

namespace RegistryApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryVM>> GetAll();
    }
}