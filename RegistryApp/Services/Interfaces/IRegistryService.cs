using RegistryApp.Filters;
using RegistryApp.Vms;

namespace RegistryApp.Services.Interfaces
{
    public interface IRegistryService
    {
        Task<List<RegistryItemVM>> GetRegistryOfUser(string userName, GenericQueryOption<RegistryFilter> option);
        Task<List<RegistryItemVM>> GetProductsBoughtByUser(string userName);
        Task<bool> AddItemToRegistry(string userName, int productId);
        Task<bool> RemoveItemFromRegistry(string userName, int productId);
        Task<bool> BuyItem(string buyerName, string recipientName, int productId);
        Task<bool> UnBuyItem(string buyerName, string recipientName, int productId);

    }
}