using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistryApp.Dtos;
using RegistryApp.Filters;
using RegistryApp.Models.Authentication;
using RegistryApp.Services.Interfaces;
using RegistryApp.Vms;

namespace RegistryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductVM>> GetAll([FromQuery] GenericQueryOption<ProductFilter> option)
        {
            return await _productService.GetAllProducts(option);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var p = _productService.GetProductById(id);
            if (p == null)
            {
                return NotFound();
            }
            return Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductDto productDto)
        {
            var newProduct = await _productService.CreateProduct(productDto);
            if (newProduct == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int id) {
            return await _productService.DeleteProduct(id) ? NoContent() : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductDto productDto) {
            return await _productService.UpdateProduct(id,productDto) ? NoContent() : NotFound();
        }

        [HttpPost("{productId}/addCategory/{categoryId}")]
        public async Task<IActionResult> AddCategory(int productId, int categoryId) {
            return await _productService.AddCategoryToProduct(productId,categoryId) ? NoContent() : NotFound();
        }

        [HttpPost("{productId}/removeCategory/{categoryId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> RemoveCategory(int productId, int categoryId) {
            return await _productService.RemoveCategoryFromProduct(productId,categoryId) ? NoContent() : NotFound();
        }
    }
}