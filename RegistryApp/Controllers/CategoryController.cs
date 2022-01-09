using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistryApp.Dtos;
using RegistryApp.Models.Authentication;
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
        public async Task<IEnumerable<CategoryVM>> GetAll()
        {
            return await _categoryService.GetAllCategories();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var c = await _categoryService.GetCategoryById(id);
            if (c == null)
            {
                return NotFound();
            }
            return Ok(c);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CategoryDto categoryDto)
        {
            var newCat = await _categoryService.CreateCategory(categoryDto);
            if (newCat == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(Get), new { id = newCat.Id }, newCat);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            return await _categoryService.DeleteCategory(id) ? NoContent() : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CategoryDto categoryDto)
        {
            return await _categoryService.UpdateCategory(id, categoryDto) ? NoContent() : NotFound();
        }
    }
}