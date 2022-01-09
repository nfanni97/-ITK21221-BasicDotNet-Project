using System;

namespace RegistryApp.Vms.Authentication
{
    public class LoginResult
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}