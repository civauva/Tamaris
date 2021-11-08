using Microsoft.AspNetCore.Identity;

namespace Tamaris.Entities.Admin
{
    public class Role : IdentityRole
    {
        /// <summary>
        /// Admin flag
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}