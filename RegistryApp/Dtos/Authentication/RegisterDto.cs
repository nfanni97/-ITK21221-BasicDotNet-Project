using RegistryApp.Models.Authentication;

namespace RegistryApp.Dtos.Authentication
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public List<string> UserRoles { get; set; }
    }
}