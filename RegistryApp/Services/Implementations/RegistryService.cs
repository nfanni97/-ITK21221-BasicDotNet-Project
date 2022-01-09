using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RegistryApp.Filters;
using RegistryApp.Models;
using RegistryApp.Models.Authentication;
using RegistryApp.Services.Interfaces;
using RegistryApp.Vms;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace RegistryApp.Services.Implementations
{
    public class RegistryService : IRegistryService
    {
        private readonly ILogger<RegistryService> _logger;
        private readonly RegistryContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegistryService(ILogger<RegistryService> logger, RegistryContext context, IMapper mapper, UserManager<ApplicationUser> usermanager)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _userManager = usermanager;
        }

        public async Task<bool> AddItemToRegistry(string userName, int productId)
        {
            //check if user exists
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return false;
            }
            //check if already present
            var registryItem = await _context.RegistryItems.Where(ri => ri.RecipientName.Equals(userName) && ri.ProductId == productId).SingleOrDefaultAsync();
            if (registryItem != null)
            {
                return false;
            }
            _context.RegistryItems.Add(new RegistryItem
            {
                Recipient = user,
                ProductId = productId,
                RecipientName = user.UserName
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BuyItem(string buyerName, string recipientName, int productId)
        {
            //check if both users exist
            var buyer = await _userManager.FindByNameAsync(buyerName);
            var recipient = await _userManager.FindByNameAsync(recipientName);
            if (buyer == null || recipient == null)
            {
                return false;
            }
            //check if product is on list
            var item = await _context.RegistryItems.Where(ri => ri.ProductId == productId && ri.RecipientName == recipientName).SingleOrDefaultAsync();
            if (item == null)
            {
                return false;
            }
            //check if not already bought
            if (item!.Buyer != null || item!.Buyer == buyer)
            {
                return false;
            }
            item.Buyer = buyer;
            item.BuyerName = buyerName;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RegistryItemVM>> GetProductsBoughtByUser(string userName)
        {
            //check if user exists
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new List<RegistryItemVM>();
            }
            return await _context.RegistryItems.ProjectTo<RegistryItemVM>(_mapper.ConfigurationProvider).Where(rivm => rivm.BuyerName.Equals(userName)).ToListAsync();
        }

        public async Task<List<RegistryItemVM>> GetRegistryOfUser(string userName, GenericQueryOption<RegistryFilter> option)
        {
            //check if user exists
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new List<RegistryItemVM>();
            }
            var q = _context.RegistryItems.Where(ri => ri.RecipientName.Equals(userName)) as IQueryable<RegistryItem>;
            //product filtering
            if (!String.IsNullOrEmpty(option.Filter?.NameTerm))
            {
                _logger.LogDebug("nameterm: " + option.Filter.NameTerm);
                q = q.Where(x => x.Product.Name.Contains(option.Filter.NameTerm));
            }
            if (option.Filter?.MinPriceHuf != null)
            {
                q = q.Where(x => x.Product.PriceHuf >= option.Filter.MinPriceHuf);
            }
            if (option.Filter?.MaxPriceHuf != null)
            {
                q = q.Where(x => x.Product.PriceHuf <= option.Filter.MaxPriceHuf);
            }
            if (!String.IsNullOrEmpty(option.Filter?.InDescription))
            {
                q = q.Where(x => x.Product.Description.Contains(option.Filter.InDescription));
            }
            //sort
            q = option.SortOrder == SortOrder.Ascending ? q.OrderBy(x => x.Product.Name) : q.OrderByDescending(x => x.Product.Name);
            //bought-filtering
            if (option.Filter?.Bought != null && option.Filter.Bought != null)
            {
                q = q.Where(ri => (bool)option.Filter.Bought ? !String.IsNullOrEmpty(ri.BuyerName) : String.IsNullOrEmpty(ri.BuyerName));
            }
            //paging
            return await q
                .Skip((option.Page - 1) * option.PageSize)
                .Take(option.PageSize)
                .ProjectTo<RegistryItemVM>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<bool> RemoveItemFromRegistry(string userName, int productId)
        {
            //check if user exists
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return false;
            }
            //check if item in registry
            var item = await _context.RegistryItems.Where(ri => ri.ProductId == productId && ri.RecipientName.Equals(userName)).SingleOrDefaultAsync();
            if(item == null) {
                return false;
            }
            _context.RegistryItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnBuyItem(string buyerName, string recipientName, int productId)
        {
            //check if users exist
            var buyer = await _userManager.FindByNameAsync(buyerName);
            var recipient = await _userManager.FindByNameAsync(recipientName);
            if (buyer == null || recipient == null)
            {
                return false;
            }
            //check if product is on list
            var item = await _context.RegistryItems.Where(ri => ri.ProductId == productId && ri.RecipientName.Equals(recipientName)).SingleOrDefaultAsync();
            if (item == null)
            {
                return false;
            }
            //check if bought and by buyer
            if(item.BuyerName == null || !item.BuyerName.Equals(buyerName)) {
                return false;
            }
            item.BuyerName = null;
            item.Buyer = null;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}