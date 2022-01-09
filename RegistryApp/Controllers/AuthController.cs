using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RegistryApp.Dtos.Authentication;
using RegistryApp.Models.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using RegistryApp.Vms.Authentication;

namespace RegistryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _conf;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration conf,ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _conf = conf;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto) {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            // pwd is hashed!
            if(user != null && await _userManager.CheckPasswordAsync(user,loginDto.Password)) {
                // get roles
                var userRoles = await _userManager.GetRolesAsync(user);
                _logger.LogDebug($"roles: {userRoles[0]}, {userRoles[1]}");
                // get claims
                var authClaims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name,user.UserName)
                };
                // role-claims
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role,role));
                }
                // create token
                var token = new JwtSecurityToken(
                    issuer: _conf["JWT:validIssuer"],
                    audience: _conf["JWT:validAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(
                        key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["JWT:secret"])),
                        algorithm: SecurityAlgorithms.HmacSha256
                    )
                );
                return Ok(new LoginResult{
                    Token = new JwtSecurityTokenHandler().WriteToken(token).ToString(),
                    Expires = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto) {
            // check if user exists
            var existingUser = await _userManager.FindByNameAsync(registerDto.Username);
            if(existingUser != null) {
                throw new HttpResponseException("user already exists") {
                    Status = StatusCodes.Status500InternalServerError
                };
            }
            var newUser = new ApplicationUser {
                Email = registerDto.Email,
                UserName = registerDto.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            //add roles
            var result = await _userManager.CreateAsync(newUser,registerDto.Password);
            await _userManager.AddToRolesAsync(newUser,registerDto.UserRoles);
            if(!result.Succeeded) {
                throw new HttpResponseException("something wrong during user creation");
            }
            return Ok();
        }
    }
}