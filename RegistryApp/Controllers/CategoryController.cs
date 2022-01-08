using Microsoft.AspNetCore.Mvc;
using RegistryApp.Services.Interfaces;
using RegistryApp.Vms;

namespace CategoryController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<CategoryVM>> GetAll() {
            return await _categoryService.GetAll();
        }
    }
}