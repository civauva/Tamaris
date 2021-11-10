using Microsoft.AspNetCore.Identity;
using Tamaris.Entities.Admin;

namespace Tamaris.API.Configuration
{
    public class DataSeederConfiguration
    {
        private readonly RoleManager<Role> _roleManager;

        public DataSeederConfiguration(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task CheckDefaultRoles()
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new Role { Name = "Administrators", IsAdmin = true });
                await _roleManager.CreateAsync(new Role { Name = "Standard users", IsAdmin = false });
            }
        }
    }
}
