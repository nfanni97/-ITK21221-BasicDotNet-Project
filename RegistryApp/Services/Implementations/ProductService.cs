using System.Linq.Expressions;
using AutoMapper;
using RegistryApp.Dtos;
using RegistryApp.Filters;
using RegistryApp.Models;
using RegistryApp.Services.Interfaces;
using RegistryApp.Vms;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace RegistryApp.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly RegistryContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(RegistryContext context, IMapper mapper, ILogger<ProductService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        private void mapCategories(Product p, ProductVM pvm)
        {
            pvm.CategoryNames = new List<string>();
            foreach (var cp in p.CategoryProducts)
            {
                pvm.CategoryNames.Add(cp.Category.Name);
            }
        }
        public async Task<bool> AddCategoryToProduct(int productId, int categoryId)
        {
            // check if category exists
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                _logger.LogWarning("Category with id " + categoryId + " not found");
                return false;
            }
            // check if product exists
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                _logger.LogWarning("Product with id " + productId + " not found");
                return false;
            }
            // check if product has that category already
            var cp = new CategoryProduct
            {
                ProductId = productId,
                CategoryId = categoryId
            };
            if (product.CategoryProducts.Contains(cp))
            {
                _logger.LogWarning("Product " + productId + " already has category " + categoryId);
                return false;
            }
            product.CategoryProducts.Add(cp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductVM> CreateProduct(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            var productVM = _mapper.Map<ProductVM>(product);
            mapCategories(product, productVM);
            return productVM;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductVM>> GetAllProducts(GenericQueryOption<ProductFilter> option)
        {
            var q = _context.Products as IQueryable<Product>;
            //filter
            if (option.Filter != null)
            {
                _logger.LogDebug($"filter nameterm: {option.Filter.NameTerm}, descr: {option.Filter.InDescription}, minprice: {option.Filter.MinPriceHuf}, maxprice: {option.Filter.MaxPriceHuf}");
            }
            if (!String.IsNullOrEmpty(option.Filter?.NameTerm))
            {
                _logger.LogDebug("nameterm: " + option.Filter.NameTerm);
                q = q.Where(x => x.Name.Contains(option.Filter.NameTerm));
            }
            if (option.Filter?.MinPriceHuf != null)
            {
                q = q.Where(x => x.PriceHuf >= option.Filter.MinPriceHuf);
            }
            if (option.Filter?.MaxPriceHuf != null)
            {
                q = q.Where(x => x.PriceHuf <= option.Filter.MaxPriceHuf);
            }
            if (!String.IsNullOrEmpty(option.Filter?.InDescription))
            {
                q = q.Where(x => x.Description.Contains(option.Filter.InDescription));
            }
            //sort
            q = option.SortOrder == SortOrder.Ascending ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name);
            //page
            var products = await q
                    .Skip((option.Page - 1) * option.PageSize)
                    .Take(option.PageSize)
                    .ToListAsync();
            var productVMs = new List<ProductVM>();
            foreach (var product in products)
            {
                var pvm = _mapper.Map<ProductVM>(product);
                _logger.LogDebug("categories of product " + product.Id + ": " + product.CategoryProducts);
                mapCategories(product, pvm);
                _logger.LogDebug("categories of productvm " + product.Id + ": " + pvm.CategoryNames);
                productVMs.Add(pvm);
            }
            return productVMs;
        }

        public async Task<ProductVM?> GetProductById(int id)
        {
            var p = await _context.Products
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            if (p == null)
            {
                return null;
            }
            var pvm = _mapper.Map<ProductVM>(p);
            mapCategories(p, pvm);
            return pvm;
        }

        public async Task<bool> RemoveCategoryFromProduct(int productId, int categoryId)
        {
            //check if category exists
            var category = await _context.Products.FindAsync(categoryId);
            if (category == null)
            {
                _logger.LogWarning("Category with id " + categoryId + " not found");
                return false;
            }
            // check if product exists
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                _logger.LogWarning("Product with id " + productId + " not found");
                return false;
            }
            // check if product does not have that category already
            var cp = new CategoryProduct
            {
                ProductId = productId,
                CategoryId = categoryId
            };
            if (!product.CategoryProducts.Contains(cp))
            {
                _logger.LogWarning("Product " + productId + " already doesn't have category " + categoryId);
                return false;
            }
            product.CategoryProducts.Remove(cp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProduct(int id, ProductDto r)
        {
            var updatedProduct = _mapper.Map<Product>(r);
            //need to save categories before overwriting!
            var oldProduct = await _context.Products.FindAsync(id);
            if (oldProduct == null)
            {
                return false;
            }
            updatedProduct.CategoryProducts = oldProduct.CategoryProducts;
            _context.Entry(oldProduct).State = EntityState.Detached;
            updatedProduct.Id = id;
            _context.Entry(updatedProduct).State = EntityState.Modified;
            var n = await _context.SaveChangesAsync();
            return n == 1;
        }
    }
}