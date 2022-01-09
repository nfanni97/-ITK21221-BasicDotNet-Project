using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class RegistryController : ControllerBase
    {
        private readonly IRegistryService _registryService;
        private readonly ILogger<RegistryController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public RegistryController(IRegistryService registryService, ILogger<RegistryController> logger, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _registryService = registryService;
            _logger = logger;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        private string getUserName() {
            return _contextAccessor.HttpContext?.User?.Identity?.Name;
        }

        [HttpPost("addItem")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> AddItemToRegistry([FromBody] AddRegistryItemDto addRegistryItemDto) {
            var userName = getUserName();
            return await _registryService.AddItemToRegistry(userName,addRegistryItemDto.ProductId) ? NoContent() : NotFound();
        }

        [HttpPost("buyItem")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> BuyItem([FromBody] BuyRegistryItemDto buyRegistryItemDto) {
            var userName = getUserName();
            return await _registryService.BuyItem(userName,buyRegistryItemDto.RecipientName,buyRegistryItemDto.ProductId) ? NoContent() : NotFound();
        }

        [HttpGet("boughtBy/{userName}")]
        [Authorize(Roles = $"{UserRoles.User},{UserRoles.Admin}")]
        public async Task<IActionResult> GetProductsBoughtByUser([FromRoute] string userName) {
            var currentUser = getUserName();
            _logger.LogDebug($"username: {userName}");
            if(!currentUser.Equals(userName) && !User.IsInRole(UserRoles.Admin)) {
                return Unauthorized();
            }
            return Ok(await _registryService.GetProductsBoughtByUser(userName));
        }

        [HttpGet("{userName}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IEnumerable<RegistryItemVM>> GetRegistryOfUser(string userName, [FromQuery] GenericQueryOption<RegistryFilter> option) {
            return await _registryService.GetRegistryOfUser(userName, option);
        }

        [HttpDelete("removeItem")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> RemoveItemFromRegistry([FromBody] AddRegistryItemDto removeRegistryItemDto) {
            var userName = getUserName();
            return await _registryService.RemoveItemFromRegistry(userName,removeRegistryItemDto.ProductId) ? NoContent() : NotFound();
        }

        [HttpPost("unBuyItem")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> UnBuyItem([FromBody] BuyRegistryItemDto unBuyRegistryItemDto) {
            var userName = getUserName();
            return await _registryService.UnBuyItem(userName,unBuyRegistryItemDto.RecipientName,unBuyRegistryItemDto.ProductId) ? NoContent() : NotFound();
        }
    }
}