using Microsoft.AspNetCore.Identity;

namespace RegistryApp.Models.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public virtual List<RegistryItem> ForMe { get; set; }
        public virtual List<RegistryItem> ByMe { get; set; }
    }
}